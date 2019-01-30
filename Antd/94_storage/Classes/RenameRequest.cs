using anthilla.core;
using System;
using System.Collections.Generic;

namespace Kvpbase {
    public class RenameRequest {
        #region Public-Members

        public string UserGuid;
        public List<string> ContainerPath;
        public string RenameFrom;
        public string RenameTo;

        #endregion

        #region Constructors-and-Factories

        public RenameRequest() {

        }

        #endregion

        #region Public-Static-Methods

        public static string BuildDiskPath(RenameRequest req, bool useRenameFrom, UserManager users, Settings settings) {
            #region Check-for-Null-Values

            if(req == null) {
                ConsoleLogger.Warn("BuildDiskPath null rename object supplied");
                return null;
            }

            #endregion

            #region Variables

            UserMaster currUser = new UserMaster();
            string homeDirectory = "";
            string fullPath = "";

            #endregion

            #region Get-User-Master-and-Home-Directory

            currUser = users.GetUserByGuid(req.UserGuid);
            if(currUser == null) {
                ConsoleLogger.Warn("BuildDiskPath unable to retrieve user object from GUID " + req.UserGuid);
                return null;
            }

            if(String.IsNullOrEmpty(currUser.HomeDirectory)) {
                // global directory
                homeDirectory = String.Copy(settings.Storage.Directory);
                if(!homeDirectory.EndsWith(Common.GetPathSeparator(settings.Environment)))
                    homeDirectory += Common.GetPathSeparator(settings.Environment);
                homeDirectory += currUser.Guid;
                homeDirectory += Common.GetPathSeparator(settings.Environment);
            }
            else {
                // user-specific home directory
                homeDirectory = String.Copy(currUser.HomeDirectory);
                if(!homeDirectory.EndsWith(Common.GetPathSeparator(settings.Environment)))
                    homeDirectory += Common.GetPathSeparator(settings.Environment);
            }

            #endregion

            #region Process

            fullPath = String.Copy(homeDirectory);

            if(req.ContainerPath != null) {
                if(req.ContainerPath.Count > 0) {
                    foreach(string currContainer in req.ContainerPath) {
                        if(String.IsNullOrEmpty(currContainer))
                            continue;

                        if(Common.ContainsUnsafeCharacters(currContainer)) {
                            ConsoleLogger.Warn("BuildDiskPath unsafe characters detected: " + currContainer);
                            return null;
                        }

                        fullPath += currContainer + Common.GetPathSeparator(settings.Environment);
                    }
                }
            }

            if(useRenameFrom) {
                if(!String.IsNullOrEmpty(req.RenameFrom))
                    fullPath += req.RenameFrom;
            }
            else {
                if(!String.IsNullOrEmpty(req.RenameTo))
                    fullPath += req.RenameTo;
            }

            // ConsoleLogger.Warn( "BuildDiskPath_RenameFrom returning full_path " + full_path);
            return fullPath;

            #endregion
        }

        public static bool UnsafeFsChars(RenameRequest currRename) {
            if(currRename == null)
                return true;
            if(Common.ContainsUnsafeCharacters(currRename.ContainerPath))
                return true;
            if(Common.ContainsUnsafeCharacters(currRename.RenameFrom))
                return true;
            if(Common.ContainsUnsafeCharacters(currRename.RenameTo))
                return true;
            return false;
        }

        #endregion
    }
}
