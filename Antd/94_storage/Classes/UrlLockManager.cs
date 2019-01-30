using anthilla.core;
using System;
using System.Collections.Generic;

namespace Kvpbase {
    public class UrlLockManager {
        #region Public-Members

        #endregion

        #region Private-Members

        private Dictionary<string, Tuple<int?, string, string, DateTime>> LockedObjects;            // UserMasterId, SourceIp, verb, established
        private List<string> ReadObjects;                                                           // URL
        private readonly object MainLock;

        #endregion

        #region Constructors-and-Factories

        public UrlLockManager() {
            LockedObjects = new Dictionary<string, Tuple<int?, string, string, DateTime>>();
            ReadObjects = new List<string>();
            MainLock = new object();
        }

        #endregion

        #region Public-Methods

        public Dictionary<string, Tuple<int?, string, string, DateTime>> GetLockedUrls() {
            lock(MainLock) {
                return LockedObjects;
            }
        }

        public List<string> GetReadUrls() {
            lock(MainLock) {
                return ReadObjects;
            }
        }

        public bool LockUrl(RequestMetadata md) {
            lock(MainLock) {
                if(LockedObjects.ContainsKey(md.CurrentObj.DiskPath)) {
                    ConsoleLogger.Warn("LockUrl resource currently locked: " + md.CurrentObj.DiskPath);
                    return false;
                }

                if(ReadObjects.Contains(md.CurrentObj.DiskPath)) {
                    ConsoleLogger.Warn("LockUrl resource currently being read: " + md.CurrentObj.DiskPath);
                    return false;
                }

                LockedObjects.Add(md.CurrentObj.DiskPath,
                    new Tuple<int?, string, string, DateTime>(
                        md.CurrentUserMaster.UserMasterId,
                        md.CurrentHttpRequest.SourceIp,
                        md.CurrentHttpRequest.Method,
                        DateTime.Now));
            }

            return true;
        }

        public bool LockResource(RequestMetadata md, string resource) {
            lock(MainLock) {
                if(LockedObjects.ContainsKey(md.CurrentObj.DiskPath)) {
                    ConsoleLogger.Warn("LockResource resource currently locked: " + md.CurrentObj.DiskPath);
                    return false;
                }

                if(ReadObjects.Contains(md.CurrentObj.DiskPath)) {
                    ConsoleLogger.Warn("LockResource resource currently being read: " + md.CurrentObj.DiskPath);
                    return false;
                }

                LockedObjects.Add(resource,
                    new Tuple<int?, string, string, DateTime>(
                        md.CurrentUserMaster.UserMasterId,
                        md.CurrentHttpRequest.SourceIp,
                        md.CurrentHttpRequest.Method,
                        DateTime.Now));
            }

            return true;
        }

        public bool UnlockUrl(RequestMetadata md) {
            lock(MainLock) {
                if(LockedObjects.ContainsKey(md.CurrentObj.DiskPath)) {
                    LockedObjects.Remove(md.CurrentObj.DiskPath);
                }
            }

            return true;
        }

        public bool UnlockResource(RequestMetadata md, string resource) {
            lock(MainLock) {
                if(LockedObjects.ContainsKey(resource)) {
                    LockedObjects.Remove(resource);
                }
            }

            return true;
        }

        public bool AddReadResource(string resource) {
            lock(MainLock) {
                if(LockedObjects.ContainsKey(resource)) {
                    ConsoleLogger.Warn("AddReadResource resource currently being read: " + resource);
                    return false;
                }

                ReadObjects.Add(resource);
            }

            return true;
        }

        public bool RemoveReadResource(string resource) {
            lock(MainLock) {
                if(ReadObjects.Contains(resource)) {
                    ReadObjects.Remove(resource);
                }
            }

            return true;
        }

        #endregion
    }
}
