using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Antd.UnitFiles {
    public class CreateUnit {
        public static void ForAntd() {
            UnitFile.Write("antd.service");
        }

        public static void ForAnthillaAS() {
            UnitFile.Write("anthillaas.service");
        }

        public static void ForAnthillaFirewall() {
            UnitFile.Write("anthillafirewall.service");
        }

        public static void ForAnthillaStorage() {
            UnitFile.Write("anthillastorage.service");
        }

        public static void ForAnthillaSP() {
            UnitFile.Write("anthillasp.service");
        }

        public static void ForAnthillaServer() {
            UnitFile.Write("anthillaserver.service");
        }
    }
}
