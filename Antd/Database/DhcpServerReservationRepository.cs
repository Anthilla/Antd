using System;
using System.Collections.Generic;
using System.Linq;
using antdlib.common;
using antdlib.views;
using antdlib.views.Repo;

namespace Antd.Database {
    public class DhcpServerReservationRepository {

        private const string ViewName = "DhcpServerReservation";

        public IEnumerable<DhcpServerReservationSchema> GetAll() {
            var result = DatabaseRepository.Query<DhcpServerReservationSchema>(AntdApplication.Database, ViewName);
            return result;
        }

        public DhcpServerReservationSchema GetByGuid(string guid) {
            var result = DatabaseRepository.Query<DhcpServerReservationSchema>(AntdApplication.Database, ViewName, schema => schema.Id == guid || schema.Guid == guid);
            return result.FirstOrDefault();
        }

        public bool Create(string hostName, string macAddress, string ipAddress) {
            var obj = new DhcpServerReservationModel {
                HostName = hostName,
                MacAddress = macAddress,
                IpAddress = ipAddress,
            };
            var result = DatabaseRepository.Save(AntdApplication.Database, obj, true);
            return result;
        }

        public bool Edit(string id, string hostName, string macAddress, string ipAddress) {
            var objUpdate = new DhcpServerReservationModel {
                Id = id.ToGuid(),
                HostName = hostName,
                MacAddress = macAddress,
                IpAddress = ipAddress
            };
            var result = DatabaseRepository.Edit(AntdApplication.Database, objUpdate, true);
            return result;
        }

        public bool Delete(string guid) {
            var result = DatabaseRepository.Delete<DhcpServerReservationModel>(AntdApplication.Database, Guid.Parse(guid));
            return result;
        }
    }
}
