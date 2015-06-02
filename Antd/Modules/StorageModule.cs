using Nancy;
using Nancy.Security;

namespace Antd {

    public class StorageModule : NancyModule {

        public StorageModule()
            : base("/storage") {
            this.RequiresAuthentication();

            Get["/home"] = x => {
                return View["page-storage-home"];
            };

            Get["/manage/disks"] = x => {
                return View["page-storage-disk"];
            };

            Get["/manage/iscsi"] = x => {
                return View["page-storage-iscsi"];
            };

            Get["/sharing"] = x => {
                return View["page-storage-share"];
            };

            Get["/user/acl"] = x => {
                return View["page-storage-user"];
            };

            Get["/backup"] = x => {
                return View["page-storage-backup"];
            };

            Get["/mail"] = x => {
                return View["page-storage-mail"];
            };

            Get["/mail/domains"] = x => {
                return View["page-storage-mldmns"];
            };

            Get["/log"] = x => {
                return View["page-storage-log"];
            };

            Get["/network"] = x => {
                return View["page-storage-net"];
            };

            Get["/network/server"] = x => {
                return View["page-storage-netsrv"];
            };

            Get["/setup"] = x => {
                return View["page-storage-setup"];
            };
        }
    }
}