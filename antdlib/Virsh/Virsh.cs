///-------------------------------------------------------------------------------------
///     Copyright (c) 2014, Anthilla S.r.l. (http://www.anthilla.com)
///     All rights reserved.
///
///     Redistribution and use in source and binary forms, with or without
///     modification, are permitted provided that the following conditions are met:
///         * Redistributions of source code must retain the above copyright
///           notice, this list of conditions and the following disclaimer.
///         * Redistributions in binary form must reproduce the above copyright
///           notice, this list of conditions and the following disclaimer in the
///           documentation and/or other materials provided with the distribution.
///         * Neither the name of the Anthilla S.r.l. nor the
///           names of its contributors may be used to endorse or promote products
///           derived from this software without specific prior written permission.
///
///     THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
///     ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
///     WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
///     DISCLAIMED. IN NO EVENT SHALL ANTHILLA S.R.L. BE LIABLE FOR ANY
///     DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
///     (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
///     LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
///     ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
///     (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
///     SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
///
///     20141110
///-------------------------------------------------------------------------------------

using antdlib.Scheduler;

namespace antdlib.Virsh {
    public class Virsh {
        public class Domain {
            public static string attach_device(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string attach_disk(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string attach_interface(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string autostart(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string blkdeviotune(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string blkiotune(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string blockcommit(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string blockcopy(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string blockjob(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string blockpull(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string blockresize(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string change_media(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string console(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string cpu_baseline(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string cpu_compare(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string cpu_stats(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string create(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string define(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string desc(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string destroy(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string detach_device(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string detach_disk(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string detach_interface(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string domdisplay(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string domfsfreeze(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string domfsthaw(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string domfsinfo(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string domfstrim(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string domhostname(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string domid(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string domif_setlink(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string domiftune(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string domjobabort(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string domjobinfo(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string domname(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string domrename(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string dompmsuspend(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string dompmwakeup(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string domuuid(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string domxml_from_native(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string domxml_to_native(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string dump(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string dumpxml(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string edit(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string @event(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string inject_nmi(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string iothreadinfo(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string iothreadpin(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string iothreadadd(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string iothreaddel(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string send_key(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string send_process_signal(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string lxc_enter_namespace(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string managedsave(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string managedsave_remove(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string memtune(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string metadata(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string migrate(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string migrate_setmaxdowntime(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string migrate_compcache(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string migrate_setspeed(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string migrate_getspeed(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string numatune(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string qemu_attach(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string qemu_monitor_command(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string qemu_monitor_event(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string qemu_agent_command(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string reboot(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string reset(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string restore(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string resume(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string save(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string save_image_define(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string save_image_dumpxml(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string save_image_edit(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string schedinfo(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string screenshot(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string set_user_password(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string setmaxmem(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string setmem(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string setvcpus(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string shutdown(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string start(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string suspend(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string ttyconsole(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string undefine(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string update_device(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string vcpucount(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string vcpuinfo(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string vcpupin(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string emulatorpin(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string vncdisplay(string options) { return Terminal.Execute($"virsh command {options}"); }
        }

        public class Monitor {
            public static string domblkerror(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string domblkinfo(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string domblklist(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string domblkstat(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string domcontrol(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string domif_getlink(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string domifaddr(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string domiflist(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string domifstat(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string dominfo(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string dommemstat(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string domstate(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string domstats(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string domtime(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string list(string options) { return Terminal.Execute($"virsh command {options}"); }
        }

        public class Host {
            public static string allocpages(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string capabilities(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string cpu_models(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string domcapabilities(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string freecell(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string freepages(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string hostname(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string maxvcpus(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string node_memory_tune(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string nodecpumap(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string nodecpustats(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string nodeinfo(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string nodememstats(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string nodesuspend(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string sysinfo(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string uri(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string version(string options) { return Terminal.Execute($"virsh command {options}"); }
        }

        public class Interface {
            public static string iface_begin(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string iface_bridge(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string iface_commit(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string iface_define(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string iface_destroy(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string iface_dumpxml(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string iface_edit(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string iface_list(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string iface_mac(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string iface_name(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string iface_rollback(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string iface_start(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string iface_unbridge(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string iface_undefine(string options) { return Terminal.Execute($"virsh command {options}"); }
        }

        public class Filter {
            public static string nwfilter_define(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string nwfilter_dumpxml(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string nwfilter_edit(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string nwfilter_list(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string nwfilter_undefine(string options) { return Terminal.Execute($"virsh command {options}"); }
        }

        public class Network {
            public static string net_autostart(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string net_create(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string net_define(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string net_destroy(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string net_dhcp_leases(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string net_dumpxml(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string net_edit(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string net_event(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string net_info(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string net_list(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string net_name(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string net_start(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string net_undefine(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string net_update(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string net_uuid(string options) { return Terminal.Execute($"virsh command {options}"); }
        }

        public class Nodedev {
            public static string nodedev_create(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string nodedev_destroy(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string nodedev_detach(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string nodedev_dumpxml(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string nodedev_list(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string nodedev_reattach(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string nodedev_reset(string options) { return Terminal.Execute($"virsh command {options}"); }
        }

        public class Secret {
            public static string secret_define(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string secret_dumpxml(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string secret_get_value(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string secret_list(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string secret_set_value(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string secret_undefine(string options) { return Terminal.Execute($"virsh command {options}"); }
        }

        public class Snapshot {
            public static string snapshot_create(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string snapshot_create_as(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string snapshot_current(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string snapshot_delete(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string snapshot_dumpxml(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string snapshot_edit(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string snapshot_info(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string snapshot_list(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string snapshot_parent(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string snapshot_revert(string options) { return Terminal.Execute($"virsh command {options}"); }
        }

        public class Pool {
            public static string find_storage_pool_sources_as(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string find_storage_pool_sources(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string pool_autostart(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string pool_build(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string pool_create_as(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string pool_create(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string pool_define_as(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string pool_define(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string pool_delete(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string pool_destroy(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string pool_dumpxml(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string pool_edit(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string pool_info(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string pool_list(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string pool_name(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string pool_refresh(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string pool_start(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string pool_undefine(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string pool_uuid(string options) { return Terminal.Execute($"virsh command {options}"); }
        }

        public class Volume {
            public static string vol_clone(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string vol_create_as(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string vol_create(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string vol_create_from(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string vol_delete(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string vol_download(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string vol_dumpxml(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string vol_info(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string vol_key(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string vol_list(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string vol_name(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string vol_path(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string vol_pool(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string vol_resize(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string vol_upload(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string vol_wipe(string options) { return Terminal.Execute($"virsh command {options}"); }
        }

        public class Self {
            public static string cd(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string echo(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string exit(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string help(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string pwd(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string quit(string options) { return Terminal.Execute($"virsh command {options}"); }
            public static string connect(string options) { return Terminal.Execute($"virsh command {options}"); }
        }
    }
}
