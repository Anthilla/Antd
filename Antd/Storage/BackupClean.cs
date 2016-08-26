using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using antdlib.common;

namespace Antd.Storage {
    public class BackupClean {

        public class Model {
            public int Index { get; set; }
            public string Name { get; set; }
            public DateTime Created { get; set; }
            public long Dimension { get; set; }
        }

        public BackupClean() {
            RemovableSnapshots = GetRemovableSnapshots();
        }

        /// <summary>
        /// Lista effettiva degli snapshot da rimuovere
        /// Viene generata partendo dalla lista completa degli snapshot dopo essere stata filtrata secondo le Regole definite sotto
        /// </summary>
        public HashSet<string> RemovableSnapshots { get; private set; }

        public HashSet<string> GetRemovableSnapshots() {
            var snapshots = new List<Model>();
            var text = Terminal.Execute("zfs list -t snap -o name,used -p");
            var lines = text.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).Skip(1);
            foreach (var line in lines) {
                var snap = new Model();
                var attr = line.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                snap.Name = attr[0];
                snap.Dimension = Convert.ToInt64(attr[1]);
                snap.Created = GetSnapshotDate(snap.Name);
                snapshots.Add(snap);
            }
            snapshots = Rule00(snapshots);
            snapshots = Rule01(snapshots.ToArray());
            snapshots = Rule02(snapshots);
            snapshots = Rule03(snapshots);
            return snapshots.Select(_ => _.Name).ToHashSet();
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
        public void Launch() {
            var dtn = DateTime.Now;
            var removableSnapshots = GetRemovableSnapshots().ToList();
            //divido in liste più piccole e più facili da gestire
            var chunks = ChunkList(removableSnapshots, 25);
            foreach (var chunk in chunks) {
                var s = Stopwatch.StartNew();
                foreach (var snapshot in chunk) {
                    DestroySnapshot(snapshot);
                }
                Thread.Sleep(1000 * 60 * 2 - Convert.ToInt32(s.ElapsedMilliseconds));
            }
            ConsoleLogger.Log($"{DateTime.Now.ToString("yyyyMMdd")} - snapshot cleanup done, time elapsed: {DateTime.Now - dtn}");
        }

        public bool DestroySnapshot(string snapshotName) {
            if (snapshotName.Contains("@")) {
                Terminal.Execute($"zfs destroy {snapshotName}");
            }
            else {
                Terminal.Execute($"zfs destroy @{snapshotName}");
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

            for (int i = 0; i < elements.Count; i += chunkSize) {
                list.Add(elements.GetRange(i, Math.Min(chunkSize, elements.Count - i)));
            }

            return list;
        }



        //Giorno
        private const int Range00 = 1;
        //Settinama
        private const int Range01 = 7;
        //Mese
        private const int Range02 = Range01 * 4;
        //Anno
        private static readonly int Range03 = Range02 * 12;
        //Dimensioni
        private const int RangeDim = 0;

        /// <summary>
        /// Regola: per la giornata corrente e per quella di ieri non faccio niente
        /// Ignora la giornata di oggi - Range00
        /// Ottendo quindi la lista degli snapshot rimanenti
        /// </summary>
        /// <param name="snapshots"></param>
        public List<Model> Rule00(List<Model> snapshots) {
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
        public List<Model> Rule01(Model[] snapshots) {
            //per trovare precedente e successivo creo una lista degli indici degli snapshot
            var indexes = new HashSet<int>();
            foreach (var snapshot in snapshots.Where(snapshot => snapshot.Dimension > RangeDim)) {
                indexes.Add(snapshot.Index);
                indexes.Add(snapshot.Index - 1);
                indexes.Add(snapshot.Index + 1);
            }
            return indexes.Select(i => snapshots[i]).ToList();
        }

        /// <summary>
        /// Regola: dal mese precedente al corrente tengo uno snapshot a settimana
        /// nell'anno in corso tengo i settimanali
        /// Pulizia degli snapshot più vecchi all'interno del range Range02 \ x \ Range03
        /// Quelli più vecchi al momento li tengo tutti e li filtro nel passaggio successivo
        /// </summary>
        /// <param name="snapshots"></param>
        /// <returns></returns>
        public List<Model> Rule02(List<Model> snapshots) {
            var minRange = DateTime.Now.AddDays(-Range02);
            var maxRange = DateTime.Now.AddDays(-Range03);
            var olderSnapshots = snapshots.Where(_ => _.Created > maxRange).OrderBy(_ => _.Name).ToList();
            var pastMonthSnapshots = snapshots.Where(_ => _.Created < minRange && _.Created > maxRange).OrderBy(_ => _.Name).ToList();

            var weeksOfPastMonth = PartitionList(pastMonthSnapshots, 4);

            foreach (var elementsOfWeek in weeksOfPastMonth) {
                var keepThis = elementsOfWeek.OrderBy(_ => _.Dimension).LastOrDefault();
                olderSnapshots.Add(keepThis);
            }

            return olderSnapshots;
        }

        /// <summary>
        /// Regola: nell'anno precedente al corrente tengo uno snapshot al mese
        /// </summary>
        /// <param name="snapshots"></param>
        /// <returns></returns>
        public List<Model> Rule03(List<Model> snapshots) {
            var range = DateTime.Now.AddDays(-Range03);
            var olderSnapshots = snapshots.Where(_ => _.Created < range).OrderBy(_ => _.Name).ToList();
            var monthsOfPastYear = PartitionList(olderSnapshots, 4);
            var keepThese = new List<Model>();

            foreach (var elementsOfMonth in monthsOfPastYear) {
                var keepThis = elementsOfMonth.OrderBy(_ => _.Dimension).LastOrDefault();
                keepThese.Add(keepThis);
            }

            return snapshots;
        }
    }
}
