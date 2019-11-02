using Antd.models;
using anthilla.core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

namespace Antd2.cmds {
    public class Zfs {

        private const string zfsCommand = "zfs";

        private const string zfsEmptyMessage = "no datasets available";

        public static ZfsDatasetModel[] GetDatasets() {
            var result = Bash.Execute($"{zfsCommand} list").ToArray();
            if (result.Length < 1) {
                return new ZfsDatasetModel[0];
            }
            if (result[0].Contains(zfsEmptyMessage)) {
                return new ZfsDatasetModel[0];
            }
            var dataset = new ZfsDatasetModel[result.Length];
            for (var i = 0; i < dataset.Length; i++) {
                var currentData = result[i].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                dataset[i] = new ZfsDatasetModel {
                    Name = currentData[0],
                    Used = currentData[1],
                    Available = currentData[2],
                    Refer = currentData[3],
                    Mountpoint = currentData[4]
                };
            }
            return dataset;
        }

        public static ZfsSnapshotModel[] GetSnapshots() {
            var result = Bash.Execute($"{zfsCommand} list -t snap").ToArray();
            if (result.Length < 1) {
                return new ZfsSnapshotModel[0];
            }
            if (result[0].Contains(zfsEmptyMessage)) {
                return new ZfsSnapshotModel[0];
            }
            var snap = new ZfsSnapshotModel[result.Length];
            for (var i = 0; i < snap.Length; i++) {
                var currentData = result[i].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                snap[i] = new ZfsSnapshotModel {
                    Name = currentData[0],
                    Used = currentData[1],
                    Available = currentData[2],
                    Refer = currentData[3],
                    Mountpoint = currentData[4]
                };
            }
            return snap;
        }

        public class Backup {

            public class Model {
                public int Index { get; set; }
                public string Name { get; set; }
                public string PoolName { get; set; }
                public DateTime Created { get; set; }
                public long Dimension { get; set; }
            }

            private const string zfsCommand = "zfs";
            private const string snapshotCleanupArgs = "list -t snap -o name,used -p";
            private const string snapshotLaunchArgs = "snap -r ";

            /// <summary>
            /// Lancia il comando per creare uno snapshot
            /// </summary>
            public static void Launch() {
                var result = Zpool.GetPools();
                if (result.Length < 1) {
                    return;
                }
                var dateArgument = DateTime.Now.ToString("yyyyMMdd-HHmmss");
                for (var i = 0; i < result.Length; i++) {
                    var currentPool = result[i];
                    Bash.Do($"{zfsCommand} {CommonString.Append(snapshotLaunchArgs, currentPool.Name, "@", dateArgument)}");
                    ConsoleLogger.Log($"[zfs] snapshot create for '{currentPool.Name}'");
                }
            }

            public static void Launch(string pool) {
                var dateArgument = DateTime.Now.ToString("yyyyMMdd-HHmmss");
                Bash.Do($"{zfsCommand} {CommonString.Append(snapshotLaunchArgs, pool, "@", dateArgument)}");
                ConsoleLogger.Log($"[zfs] snapshot create for '{pool}'");
            }

            /// <summary>
            /// Lista effettiva degli snapshot da rimuovere
            /// Viene generata partendo dalla lista completa degli snapshot dopo essere stata filtrata secondo le Regole definite sotto
            /// </summary>
            public static HashSet<string> GetRemovableZfsSnapshotModels() {
                var result = Bash.Execute($"{zfsCommand} {snapshotCleanupArgs}").Skip(1).ToArray();
                var snapshots = new List<Model>();
                foreach (var line in result) {
                    var attr = line.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                    if (attr.Length < 1) {
                        continue;
                    }
                    var snap = new Model();
                    snap.Name = attr[0];
                    snap.PoolName = attr[0].Split('@').FirstOrDefault();
                    snap.Dimension = Convert.ToInt64(attr[1]);
                    snap.Created = GetZfsSnapshotModelDate(snap.Name);
                    snapshots.Add(snap);
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
            public static void Cleanup() {
                var removableZfsSnapshotModels = GetRemovableZfsSnapshotModels().ToList();
                if (!removableZfsSnapshotModels.Any()) {
                    return;
                }
                //divido in liste più piccole e più facili da gestire
                var chunks = ChunkList(removableZfsSnapshotModels, 25);
                foreach (var chunk in chunks) {
                    var s = Stopwatch.StartNew();
                    foreach (var snapshot in chunk) {
                        DestroyZfsSnapshotModel(snapshot);
                    }
                    Thread.Sleep(1000 * 60 * 5 - Convert.ToInt32(s.ElapsedMilliseconds));
                }
                ConsoleLogger.Log($"{DateTime.Now:yyyyMMdd} - snapshot cleanup done");
            }

            private static bool DestroyZfsSnapshotModel(string snapshotName) {
                Bash.Execute(snapshotName.Contains("@") ? $"zfs destroy {snapshotName}" : $"zfs destroy @{snapshotName}");
                return true;
            }

            /// <summary>
            /// Prende il nome dello snapshot, estrae la stringa corrispondente alla data (il formato è noto: yyyyMMdd)
            /// Converte la stringa in DateTime
            /// </summary>
            /// <param name="snapshotName"></param>
            /// <returns></returns>
            private static DateTime GetZfsSnapshotModelDate(string snapshotName) {
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
                var olderZfsSnapshotModels = snapshots.Where(_ => _.Created > maxRange).OrderBy(_ => _.Name).ToList();
                var pastMonthZfsSnapshotModels = snapshots.Where(_ => _.Created < minRange && _.Created > maxRange).OrderBy(_ => _.Name).ToList();

                var weeksOfPastMonth = PartitionList(pastMonthZfsSnapshotModels, 4);

                foreach (var elementsOfWeek in weeksOfPastMonth) {
                    var keepThis = elementsOfWeek.OrderBy(_ => _.Dimension).LastOrDefault();
                    olderZfsSnapshotModels.Add(keepThis);
                }

                var result = olderZfsSnapshotModels.Where(_ => _ != null).ToList();
                return result;
            }

            /// <summary>
            /// Regola: nell'anno precedente al corrente tengo uno snapshot al mese
            /// </summary>
            /// <param name="snapshots"></param>
            /// <returns></returns>
            private static List<Model> Rule03(List<Model> snapshots) {
                var range = DateTime.Now.AddDays(-Range03);
                var olderZfsSnapshotModels = snapshots.Where(_ => _ != null && _.Created < range).ToList();
                var monthsOfPastYear = PartitionList(olderZfsSnapshotModels.OrderBy(_ => _.Name).ToList(), 4);
                var keepThese = new List<Model>();

                foreach (var elementsOfMonth in monthsOfPastYear) {
                    var keepThis = elementsOfMonth.OrderBy(_ => _.Dimension).LastOrDefault();
                    keepThese.Add(keepThis);
                }

                return keepThese;
            }
        }
    }
}
