namespace Antd.UnitFiles {
    public class ServiceUnitInfo {
        public static void SetDefaultUnitInfo() {
            SetAntdUnitsInfo();
            SetAnthillaASUnitsInfo();
            SetAnthillaSPUnitsInfo();
        }

        public static void SetAntdUnitsInfo() {
            UnitRepo.SetInfo("C2BCCED9-42D1-4075-9036-3BD4A8C1355E", new string[] {
                "antd.service",
                "/usr/bin/mono /antd/Antd/start.antd.sh"
            });
        }

        public static void SetAnthillaASUnitsInfo() {
            UnitRepo.SetInfo("0EC23814-92EA-4EB3-91D8-8B9BAECFD733", new string[] {
                "anthillaas.service",
                "/usr/bin/mono /framework/Anthilla/AnthillaAS/AnthillaAS/AnthillaAS.exe"
            });
            UnitRepo.SetInfo("BE9BBA57-B551-43B5-965B-6F9E0DFE7CF5", new string[] {
                "anthillafirewall.service",
                "/usr/bin/mono /framework/Anthilla/AnthillaAS/AnthillaFirewall/AnthillaFirewall.exe"
            });
            UnitRepo.SetInfo("613B8C07-1F7E-4048-8BB7-A5FC47E64D00", new string[] {
                "anthillastorage.service",
                "/usr/bin/mono /framework/Anthilla/AnthillaAS/AnthillaStorage/AnthillaStorage.exe"
            });
        }

        public static void SetAnthillaSPUnitsInfo() {
            UnitRepo.SetInfo("5A7D2098-0B99-44F3-AE52-DBA90666703A", new string[] {
                "anthillasp.service",
                "/usr/bin/mono /framework/Anthilla/AnthillaSP/AnthillaSP/AnthillaSP.exe"
            });
            UnitRepo.SetInfo("A9A9F1C7-1885-4640-85D1-0D9195952621", new string[] {
                "anthillaserver.service",
                "/usr/bin/mono /framework/Anthilla/AnthillaSP/AnthillaServer/AnthillaServer.exe"
            });
        }
    }
}
