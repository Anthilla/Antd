using Antd.MDns;
using System;

namespace ServiceFinder {
    class Program {
        private static object s_gate = new object();

        static void Main(string[] args) {
            string serviceType = "_workstation._tcp";
            if (args.Length >= 1) {
                serviceType = args[0];
            }

            ServiceBrowser serviceBrowser = new ServiceBrowser();
            serviceBrowser.ServiceAdded += onServiceAdded;
            serviceBrowser.ServiceRemoved += onServiceRemoved;
            serviceBrowser.ServiceChanged += onServiceChanged;

            Console.WriteLine("Browsing for type: {0}", serviceType);
            serviceBrowser.StartBrowse(serviceType);
            Console.ReadLine();
        }

        static void onServiceChanged(object sender, ServiceAnnouncementEventArgs e) {
            printService('~', e.Announcement);
        }

        static void onServiceRemoved(object sender, ServiceAnnouncementEventArgs e) {
            printService('-', e.Announcement);
        }

        static void onServiceAdded(object sender, ServiceAnnouncementEventArgs e) {
            printService('+', e.Announcement);
        }

        static void printService(char startChar, ServiceAnnouncement service) {
            lock (s_gate) {
                Console.WriteLine("{0} '{1}' on {2}", startChar, service.Instance, service.NetworkInterface.Name);
                Console.WriteLine("\tHost: {0} ({1})", service.Hostname, string.Join(", ", service.Addresses));
                Console.WriteLine("\tPort: {0}", service.Port);
                Console.WriteLine("\tTxt : [{0}]", string.Join(", ", service.Txt));
            }
        }
    }
}
