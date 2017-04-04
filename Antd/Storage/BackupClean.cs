using antdlib.common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

namespace Antd.Storage {
    public class BackupClean {

        public class Model {
            public int Index { get; set; }
            public string Name { get; set; }
            public string PoolName { get; set; }
            public DateTime Created { get; set; }
            public long Dimension { get; set; }
        }

        private readonly Bash _bash = new Bash();

        /// <summary>
        /// Lista effettiva degli snapshot da rimuovere
        /// Viene generata partendo dalla lista completa degli snapshot dopo essere stata filtrata secondo le Regole definite sotto
        /// </summary>
        public HashSet<string> GetRemovableSnapshots(IEnumerable<string> t = null) {
            var snapshots = new List<Model>();
            var text = _bash.Execute("zfs list -t snap -o name,used -p");
            var lines = t ?? text.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).Skip(1);
            foreach (var line in lines) {
                var snap = new Model();
                var attr = line.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                try {
                    snap.Name = attr[0];
                    snap.PoolName = attr[0].Split('@').FirstOrDefault();
                    snap.Dimension = Convert.ToInt64(attr[1]);
                    snap.Created = GetSnapshotDate(snap.Name);
                    snapshots.Add(snap);
                }
                catch (Exception ex) {
                    Console.WriteLine(ex);
                }
            }

            var list = new HashSet<string>();
            var snapshotGroups = snapshots.GroupBy(_ => _.PoolName);
            foreach (var snpgrp in snapshotGroups) {
                var filter = Filter(snpgrp);
                foreach (var snp in filter) {
                    list.Add(snp.Name);
                }
            }
            return list;
        }

        /// <summary>
        /// Avvia l'eliminazione degli snapshot
        /// per non appensantire il lavoro l'eliminazione verrà fatta:
        ///     - in orari notturni (?)
        ///     - una per volta in modo da non sovrapporre più azioni di zfs
        /// Controllo che l'eliminazione venga fatta
        /// 
        /// Questa azione viene scatenata una volta al giorno
        /// Questa azione schedula / gestisce le cancellazioni in maniera controllata
        /// </summary>
        public void Launch(IEnumerable<string> t = null) {
            var dtn = DateTime.Now;
            var removableSnapshots = GetRemovableSnapshots(t).ToList();
            //divido in liste più piccole e più facili da gestire
            var chunks = ChunkList(removableSnapshots, 25);
            foreach (var chunk in chunks) {
                var s = Stopwatch.StartNew();
                foreach (var snapshot in chunk) {
                    DestroySnapshot(snapshot);
                }
                Thread.Sleep(1000 * 60 * 2 - Convert.ToInt32(s.ElapsedMilliseconds));
            }
            ConsoleLogger.Log($"{DateTime.Now:yyyyMMdd} - snapshot cleanup done, time elapsed: {DateTime.Now - dtn}");
        }

        public bool DestroySnapshot(string snapshotName) {
            if (snapshotName.Contains("@")) {
                _bash.Execute($"zfs destroy {snapshotName}", false);
            }
            else {
                _bash.Execute($"zfs destroy @{snapshotName}", false);
            }
            return true;
        }

        /// <summary>
        /// Prende il nome dello snapshot, estrae la stringa corrispondente alla data (il formato è noto: yyyyMMdd)
        /// Converte la stringa in DateTime
        /// </summary>
        /// <param name="snapshotName"></param>
        /// <returns></returns>
        private static DateTime GetSnapshotDate(string snapshotName) {
            var regex = new Regex("([\\d]{8})");
            var match = regex.Match(snapshotName);
            return string.IsNullOrEmpty(match.Value) ? DateTime.Now : DateTime.ParseExact(match.Value, "yyyyMMdd", CultureInfo.InvariantCulture);
        }

        private static List<T>[] PartitionList<T>(List<T> elements, int totalChunks = 2) {
            var partitions = new List<T>[totalChunks];
            var maxSize = (int)Math.Ceiling(elements.Count / (double)totalChunks);
            var k = 0;
            for (var i = 0; i < partitions.Length; i++) {
                partitions[i] = new List<T>();
                for (var j = k; j < k + maxSize; j++) {
                    if (j >= elements.Count)
                        break;
                    partitions[i].Add(elements[j]);
                }
                k += maxSize;
            }
            return partitions;
        }

        private static List<List<T>> ChunkList<T>(List<T> elements, int chunkSize = 30) {
            var list = new List<List<T>>();
            for (var i = 0; i < elements.Count; i += chunkSize) {
                list.Add(elements.GetRange(i, Math.Min(chunkSize, elements.Count - i)));
            }
            return list;
        }

        #region Consts
        //Giorno
        private const int Range00 = 1;
        //Settinama
        private const int Range01 = 7;
        //Mese
        private const int Range02 = Range01 * 4;
        //Anno
        private const int Range03 = Range02 * 12;
        //Dimensioni
        private const int RangeDim = 0;
        #endregion

        private static IEnumerable<Model> Filter(IEnumerable<Model> list) {
            var rule00List = Rule00(list);
            var rule01List = Rule01(rule00List.ToArray());
            var rule02List = Rule02(rule01List);
            var rule03List = Rule03(rule02List);
            return rule03List;
        }

        /// <summary>
        /// Regola: per la giornata corrente e per quella di ieri non faccio niente
        /// Ignora la giornata di oggi - Range00
        /// Ottendo quindi la lista degli snapshot rimanenti
        /// </summary>
        /// <param name="snapshots"></param>
        private static List<Model> Rule00(IEnumerable<Model> snapshots) {
            var range = DateTime.Now.AddDays(-Range00);
            return snapshots.Where(_ => _.Created < range).ToList();
        }

        /// <summary>
        /// Regola: per tutti i giorni prima di ieri tengo tutti gli snaphot con dimensioni positive, più il suo precedente e il suo successivo
        /// Regola generale / prima scrematura
        /// prende tutti gli snapshot con dimensioni > RangeDim
        /// di questi si aggiunge il precedente e il successivo
        /// Questa regola viene applicata dopo Rule00
        /// Ottendo quindi la lista degli snapshot rimanenti
        /// </summary>
        /// <param name="snapshots"></param>
        /// <returns></returns>
        private static List<Model> Rule01(Model[] snapshots) {
            //per trovare precedente e successivo creo una lista degli indici degli snapshot
            //questi oggetti saranno quelli da TENERE
            var indexes = new HashSet<int>();
            var snpList = snapshots.Where(snapshot => snapshot.Dimension > RangeDim);
            foreach (var snapshot in snpList) {
                indexes.Add(snapshot.Index);
                var prevIndex = snapshot.Index - 1;
                if (prevIndex > 0) {
                    indexes.Add(prevIndex);
                }
                var nextIndex = snapshot.Index + 1;
                if (nextIndex < snapshots.Length) {
                    indexes.Add(nextIndex);
                }
            }
            var result = snapshots.Where(_ => !indexes.Contains(_.Index)).ToList();
            //var result = indexes.Select(i => snapshots[i]).ToList();
            return result;
        }

        /// <summary>
        /// Regola: dal mese precedente al corrente tengo uno snapshot a settimana
        /// nell'anno in corso tengo i settimanali
        /// Pulizia degli snapshot più vecchi all'interno del range Range02 \ x \ Range03
        /// Quelli più vecchi al momento li tengo tutti e li filtro nel passaggio successivo
        /// </summary>
        /// <param name="snapshots"></param>
        /// <returns></returns>
        private static List<Model> Rule02(List<Model> snapshots) {
            var minRange = DateTime.Now.AddDays(-Range02);
            var maxRange = DateTime.Now.AddDays(-Range03);
            var olderSnapshots = snapshots.Where(_ => _.Created > maxRange).OrderBy(_ => _.Name).ToList();
            var pastMonthSnapshots = snapshots.Where(_ => _.Created < minRange && _.Created > maxRange).OrderBy(_ => _.Name).ToList();

            var weeksOfPastMonth = PartitionList(pastMonthSnapshots, 4);

            foreach (var elementsOfWeek in weeksOfPastMonth) {
                var keepThis = elementsOfWeek.OrderBy(_ => _.Dimension).LastOrDefault();
                olderSnapshots.Add(keepThis);
            }

            var result = olderSnapshots.Where(_ => _ != null).ToList();
            return result;
        }

        /// <summary>
        /// Regola: nell'anno precedente al corrente tengo uno snapshot al mese
        /// </summary>
        /// <param name="snapshots"></param>
        /// <returns></returns>
        private static List<Model> Rule03(List<Model> snapshots) {
            var range = DateTime.Now.AddDays(-Range03);
            var olderSnapshots = snapshots.Where(_ => _ != null && _.Created < range).ToList();
            var monthsOfPastYear = PartitionList(olderSnapshots.OrderBy(_ => _.Name).ToList(), 4);
            var keepThese = new List<Model>();

            foreach (var elementsOfMonth in monthsOfPastYear) {
                var keepThis = elementsOfMonth.OrderBy(_ => _.Dimension).LastOrDefault();
                keepThese.Add(keepThis);
            }

            return snapshots;
        }
    }
}
