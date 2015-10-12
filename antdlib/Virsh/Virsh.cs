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
            public static void attach_device(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void attach_disk(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void attach_interface(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void autostart(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void blkdeviotune(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void blkiotune(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void blockcommit(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void blockcopy(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void blockjob(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void blockpull(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void blockresize(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void change_media(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void console(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void cpu_baseline(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void cpu_compare(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void cpu_stats(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void create(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void define(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void desc(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void destroy(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void detach_device(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void detach_disk(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void detach_interface(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void domdisplay(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void domfsfreeze(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void domfsthaw(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void domfsinfo(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void domfstrim(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void domhostname(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void domid(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void domif_setlink(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void domiftune(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void domjobabort(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void domjobinfo(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void domname(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void domrename(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void dompmsuspend(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void dompmwakeup(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void domuuid(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void domxml_from_native(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void domxml_to_native(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void dump(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void dumpxml(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void edit(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void @event(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void inject_nmi(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void iothreadinfo(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void iothreadpin(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void iothreadadd(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void iothreaddel(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void send_key(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void send_process_signal(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void lxc_enter_namespace(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void managedsave(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void managedsave_remove(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void memtune(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void metadata(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void migrate(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void migrate_setmaxdowntime(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void migrate_compcache(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void migrate_setspeed(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void migrate_getspeed(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void numatune(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void qemu_attach(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void qemu_monitor_command(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void qemu_monitor_event(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void qemu_agent_command(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void reboot(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void reset(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void restore(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void resume(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void save(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void save_image_define(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void save_image_dumpxml(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void save_image_edit(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void schedinfo(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void screenshot(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void set_user_password(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void setmaxmem(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void setmem(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void setvcpus(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void shutdown(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void start(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void suspend(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void ttyconsole(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void undefine(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void update_device(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void vcpucount(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void vcpuinfo(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void vcpupin(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void emulatorpin(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void vncdisplay(string options) { Terminal.Execute($"virsh command {options}"); }
        }

        public class Monitor {
            public static void domblkerror(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void domblkinfo(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void domblklist(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void domblkstat(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void domcontrol(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void domif_getlink(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void domifaddr(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void domiflist(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void domifstat(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void dominfo(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void dommemstat(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void domstate(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void domstats(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void domtime(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void list(string options) { Terminal.Execute($"virsh command {options}"); }
        }

        public class Host {
            public static void allocpages(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void capabilities(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void cpu_models(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void domcapabilities(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void freecell(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void freepages(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void hostname(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void maxvcpus(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void node_memory_tune(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void nodecpumap(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void nodecpustats(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void nodeinfo(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void nodememstats(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void nodesuspend(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void sysinfo(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void uri(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void version(string options) { Terminal.Execute($"virsh command {options}"); }
        }

        public class Interface {
            public static void iface_begin(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void iface_bridge(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void iface_commit(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void iface_define(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void iface_destroy(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void iface_dumpxml(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void iface_edit(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void iface_list(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void iface_mac(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void iface_name(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void iface_rollback(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void iface_start(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void iface_unbridge(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void iface_undefine(string options) { Terminal.Execute($"virsh command {options}"); }
        }

        public class Filter {
            public static void nwfilter_define(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void nwfilter_dumpxml(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void nwfilter_edit(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void nwfilter_list(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void nwfilter_undefine(string options) { Terminal.Execute($"virsh command {options}"); }
        }

        public class Network {
            public static void net_autostart(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void net_create(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void net_define(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void net_destroy(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void net_dhcp_leases(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void net_dumpxml(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void net_edit(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void net_event(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void net_info(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void net_list(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void net_name(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void net_start(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void net_undefine(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void net_update(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void net_uuid(string options) { Terminal.Execute($"virsh command {options}"); }
        }

        public class Nodedev {
            public static void nodedev_create(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void nodedev_destroy(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void nodedev_detach(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void nodedev_dumpxml(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void nodedev_list(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void nodedev_reattach(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void nodedev_reset(string options) { Terminal.Execute($"virsh command {options}"); }
        }

        public class Secret {
            public static void secret_define(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void secret_dumpxml(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void secret_get_value(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void secret_list(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void secret_set_value(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void secret_undefine(string options) { Terminal.Execute($"virsh command {options}"); }
        }

        public class Snapshot {
            public static void snapshot_create(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void snapshot_create_as(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void snapshot_current(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void snapshot_delete(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void snapshot_dumpxml(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void snapshot_edit(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void snapshot_info(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void snapshot_list(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void snapshot_parent(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void snapshot_revert(string options) { Terminal.Execute($"virsh command {options}"); }
        }

        public class Pool {
            public static void find_storage_pool_sources_as(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void find_storage_pool_sources(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void pool_autostart(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void pool_build(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void pool_create_as(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void pool_create(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void pool_define_as(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void pool_define(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void pool_delete(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void pool_destroy(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void pool_dumpxml(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void pool_edit(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void pool_info(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void pool_list(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void pool_name(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void pool_refresh(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void pool_start(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void pool_undefine(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void pool_uuid(string options) { Terminal.Execute($"virsh command {options}"); }
        }

        public class Volume {
            public static void vol_clone(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void vol_create_as(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void vol_create(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void vol_create_from(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void vol_delete(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void vol_download(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void vol_dumpxml(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void vol_info(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void vol_key(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void vol_list(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void vol_name(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void vol_path(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void vol_pool(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void vol_resize(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void vol_upload(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void vol_wipe(string options) { Terminal.Execute($"virsh command {options}"); }
        }

        public class Self {
            public static void cd(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void echo(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void exit(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void help(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void pwd(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void quit(string options) { Terminal.Execute($"virsh command {options}"); }
            public static void connect(string options) { Terminal.Execute($"virsh command {options}"); }
        }
    }
}
