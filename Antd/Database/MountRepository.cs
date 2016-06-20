using System;
using System.Collections.Generic;
using System.Linq;
using antdlib.common;
using antdlib.common.Helpers;
using antdlib.views;
using antdlib.views.Repo;

namespace Antd.Database {
    public class MountRepository {
        private const string ViewName = "Mount";

        public IEnumerable<MountSchema> GetAll() {
            var result = DatabaseRepository.Query<MountSchema>(AntdApplication.Database, ViewName);
            return result;
        }

        public MountSchema GetByGuid(string guid) {
            var result = DatabaseRepository.Query<MountSchema>(AntdApplication.Database, ViewName, schema => schema.Id == guid || schema.Guid == guid);
            return result.FirstOrDefault();
        }

        public bool Create(IDictionary<string, string> dict) {
            var mountContext = dict["MountContext"];
            var mountEntity = dict["MountEntity"];
            var path = dict["Path"];
            var obj = new MountModel {
                DfpTimestamp = Timestamp.Now,
                MountContext = (MountContext)Enum.Parse(typeof(MountContext), mountContext),
                MountEntity = (MountEntity)Enum.Parse(typeof(MountEntity), mountEntity),
                MountStatus = MountStatus.Mounted,
                DirsPath = Mounts.SetDirsPath(path),
                Path = path
            };
            obj.HtmlStatusIcon = AssignHtmlStatusIcon(obj.MountStatus);
            var result = DatabaseRepository.Save(AntdApplication.Database, obj, true);
            return result;
        }

        public bool Edit(IDictionary<string, string> dict) {
            var id = dict["Id"];
            var mountContext = dict["MountContext"];
            var mountEntity = dict["MountEntity"];
            var path = dict["Path"];
            var mountedPath = dict["MountedPath"];
            var mountStatus = dict["MountStatus"];
            var units = dict["Units"];
            var objUpdate = new MountModel {
                Id = id.ToGuid(),
                DfpTimestamp = Timestamp.Now,
                MountContext = (MountContext)Enum.Parse(typeof(MountContext), mountContext),
                MountEntity = (MountEntity)Enum.Parse(typeof(MountEntity), mountEntity),
                MountStatus = (MountStatus)Enum.Parse(typeof(MountStatus), mountStatus),
                DirsPath = path.IsNullOrEmpty() ? null : Mounts.SetDirsPath(path),
                Path = path.IsNullOrEmpty() ? null : path,
                MountedPath = mountedPath.IsNullOrEmpty() ? null : mountedPath,
                AssociatedUnits = units.SplitToList()
            };
            objUpdate.HtmlStatusIcon = AssignHtmlStatusIcon(objUpdate.MountStatus);
            var result = DatabaseRepository.Edit(AntdApplication.Database, objUpdate, true);
            return result;
        }

        private static string AssignHtmlStatusIcon(MountStatus status) {
            switch (status) {
                case MountStatus.Mounted:
                    return "icon-record fg-green";
                case MountStatus.Unmounted:
                    return "icon-record fg-red";
                case MountStatus.MountedTmp:
                    return "icon-record fg-orange";
                case MountStatus.DifferentMount:
                    return "icon-stop-2 fg-orange";
                case MountStatus.MountedReadOnly:
                    return "icon-stop-2 fg-red";
                case MountStatus.MountedReadWrite:
                    return "icon-stop-2 fg-red";
                case MountStatus.Error:
                    return "icon-stop-2 fg-red";
                default:
                    return "icon-stop-2 fg-red";
            }
        }

        public bool Delete(string guid) {
            var result = DatabaseRepository.Delete<MountModel>(AntdApplication.Database, Guid.Parse(guid));
            return result;
        }

        public IEnumerable<MountSchema> GetByUnit(string unit) {
            var result = GetAll().Where(_ => _.AssociatedUnits.Contains(unit));
            return result;
        }

        public IEnumerable<string> GetListByUnit(string unit) {
            var result = GetByUnit(unit).Select(_ => _.MountedPath);
            return result;
        }

        public MountSchema GetByPath(string path) {
            var result = DatabaseRepository.Query<MountSchema>(AntdApplication.Database, ViewName, schema => schema.Path == path || schema.DirsPath == path || schema.MountedPath == path);
            return result.FirstOrDefault();
        }

        public void SetAsMounted(string path, string mountedPath) {
            var m = GetByPath(path);
            Edit(new Dictionary<string, string> {
                {"Id", m.Guid},
                {"Path", path},
                {"MountedPath", mountedPath},
                {"MountStatus", MountStatus.Mounted.ToString()}
            });
        }

        public void SetAsNotMounted(string path) {
            var m = GetByPath(path);
            Edit(new Dictionary<string, string> {
                {"Id", m.Guid},
                {"Path", path},
                {"MountStatus", MountStatus.Unmounted.ToString()}
            });
        }

        public void SetAsTmpMounted(string path) {
            var m = GetByPath(path);
            Edit(new Dictionary<string, string> {
                {"Id", m.Guid},
                {"Path", path},
                {"MountStatus", MountStatus.MountedTmp.ToString()}
            });
        }

        public void SetAsMountedReadOnly(string path) {
            var m = GetByPath(path);
            Edit(new Dictionary<string, string> {
                {"Id", m.Guid},
                {"Path", path},
                {"MountStatus", MountStatus.MountedReadOnly.ToString()}
            });
        }

        public void SetAsDifferentMounted(string path) {
            var m = GetByPath(path);
            Edit(new Dictionary<string, string> {
                {"Id", m.Guid},
                {"Path", path},
                {"MountStatus", MountStatus.DifferentMount.ToString()}
            });
        }

        public void SetAsError(string path) {
            var m = GetByPath(path);
            Edit(new Dictionary<string, string> {
                {"Id", m.Guid},
                {"Path", path},
                {"MountStatus", MountStatus.Error.ToString()}
            });
        }

        public void SetUnit(string path, IEnumerable<string> units) {
            var m = GetByPath(path);
            Edit(new Dictionary<string, string> {
                {"Id", m.Guid},
                {"Units", units.JoinToString()}
            });
        }
    }
}
