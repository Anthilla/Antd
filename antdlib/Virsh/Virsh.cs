//-------------------------------------------------------------------------------------
//     Copyright (c) 2014, Anthilla S.r.l. (http://www.anthilla.com)
//     All rights reserved.
//
//     Redistribution and use in source and binary forms, with or without
//     modification, are permitted provided that the following conditions are met:
//         * Redistributions of source code must retain the above copyright
//           notice, this list of conditions and the following disclaimer.
//         * Redistributions in binary form must reproduce the above copyright
//           notice, this list of conditions and the following disclaimer in the
//           documentation and/or other materials provided with the distribution.
//         * Neither the name of the Anthilla S.r.l. nor the
//           names of its contributors may be used to endorse or promote products
//           derived from this software without specific prior written permission.
//
//     THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
//     ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
//     WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
//     DISCLAIMED. IN NO EVENT SHALL ANTHILLA S.R.L. BE LIABLE FOR ANY
//     DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
//     (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
//     LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
//     ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
//     (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
//     SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//
//     20141110
//-------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using antdlib.common;

namespace antdlib.Virsh {
    public class Virsh {

        public class VM {
            public string Id { get; set; }
            public string Domain { get; set; }
            public string State { get; set; }
            public string VncIp { get; set; }
            public string VncPort { get; set; }
        }

        public static IEnumerable<VM> GetVmList() {
            var vms = new List<VM>();
            var res = Bash.Execute("virsh list --all | sed '1,2d'");
            if (res.Length < 1) {
                return vms;
            }
            var virshVms = res.Split(new[] { Environment.NewLine }, 3, StringSplitOptions.RemoveEmptyEntries);
            foreach (var i in virshVms) {
                var info = i.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                var vm = new VM {
                    Id = info[0],
                    Domain = info[1],
                    State = info[2],
                };
                var vnc = GetVmVncAddress(vm.Domain);
                vm.VncIp = vnc.Key;
                vm.VncPort = vnc.Value;
                vms.Add(vm);
            }
            return vms;
        }

        private static KeyValuePair<string, string> GetVmVncAddress(string domain) {
            //virsh dumpxml 004_NET_HWK_AnthillaOS | grep "graphics type='vnc'"
            //<graphics type='vnc' port='22004' autoport='no' websocket='23004' listen='10.1.19.1' keymap='it' sharePolicy='force-shared'>
            var res = Bash.Execute($"virsh dumpxml {domain} | grep \"graphics type='vnc'\"");
            if (res.Length < 1 || !res.Contains("port=") || !res.Contains("listen=")) {
                return new KeyValuePair<string, string>(null, null);
            }
            var portRegex = "port='([\\d]*)'";
            var ipRegex = "listen='([\\d. ]*)'";
            var port = new Regex(portRegex, RegexOptions.Multiline).Matches(res)[0].Value;
            var ip = new Regex(ipRegex, RegexOptions.Multiline).Matches(res)[0].Value;
            return new KeyValuePair<string, string>(ip, port);
        }

        public class Domain {
            public static string Attachdevice(string options) { return Bash.Execute($"virsh attach_device {options}"); }
            public static string Attachdisk(string options) { return Bash.Execute($"virsh attach_disk {options}"); }
            public static string Attachinterface(string options) { return Bash.Execute($"virsh attach_interface {options}"); }

            /// <summary>
            /// TODO
            /// </summary>
            /// <param name="options"></param>
            /// <returns></returns>
            public static string Autostart(string options) { return Bash.Execute($"virsh autostart {options}"); }

            public static string Blkdeviotune(string options) { return Bash.Execute($"virsh blkdeviotune {options}"); }
            public static string Blkiotune(string options) { return Bash.Execute($"virsh blkiotune {options}"); }
            public static string Blockcommit(string options) { return Bash.Execute($"virsh blockcommit {options}"); }
            public static string Blockcopy(string options) { return Bash.Execute($"virsh blockcopy {options}"); }
            public static string Blockjob(string options) { return Bash.Execute($"virsh blockjob {options}"); }
            public static string Blockpull(string options) { return Bash.Execute($"virsh blockpull {options}"); }
            public static string Blockresize(string options) { return Bash.Execute($"virsh blockresize {options}"); }
            public static string Changemedia(string options) { return Bash.Execute($"virsh change_media {options}"); }
            public static string Console(string options) { return Bash.Execute($"virsh console {options}"); }
            public static string Cpubaseline(string options) { return Bash.Execute($"virsh cpu_baseline {options}"); }
            public static string Cpucompare(string options) { return Bash.Execute($"virsh cpu_compare {options}"); }
            public static string Cpustats(string options) { return Bash.Execute($"virsh cpu_stats {options}"); }
            public static string Create(string options) { return Bash.Execute($"virsh create {options}"); }
            public static string Define(string options) { return Bash.Execute($"virsh define {options}"); }
            public static string Desc(string options) { return Bash.Execute($"virsh desc {options}"); }

            /// <summary>
            /// TODO
            /// </summary>
            /// <param name="options"></param>
            /// <returns></returns>
            public static string Destroy(string options) { return Bash.Execute($"virsh destroy {options}"); }

            public static string detach_device(string options) { return Bash.Execute($"virsh detach_device {options}"); }
            public static string detach_disk(string options) { return Bash.Execute($"virsh detach_disk {options}"); }
            public static string detach_interface(string options) { return Bash.Execute($"virsh detach_interface {options}"); }
            public static string Domdisplay(string options) { return Bash.Execute($"virsh domdisplay {options}"); }
            public static string Domfsfreeze(string options) { return Bash.Execute($"virsh domfsfreeze {options}"); }
            public static string Domfsthaw(string options) { return Bash.Execute($"virsh domfsthaw {options}"); }
            public static string Domfsinfo(string options) { return Bash.Execute($"virsh domfsinfo {options}"); }
            public static string Domfstrim(string options) { return Bash.Execute($"virsh domfstrim {options}"); }
            public static string Domhostname(string options) { return Bash.Execute($"virsh domhostname {options}"); }
            public static string Domid(string options) { return Bash.Execute($"virsh domid {options}"); }
            public static string domif_setlink(string options) { return Bash.Execute($"virsh domif_setlink {options}"); }
            public static string Domiftune(string options) { return Bash.Execute($"virsh domiftune {options}"); }
            public static string Domjobabort(string options) { return Bash.Execute($"virsh domjobabort {options}"); }
            public static string Domjobinfo(string options) { return Bash.Execute($"virsh domjobinfo {options}"); }
            public static string Domname(string options) { return Bash.Execute($"virsh domname {options}"); }
            public static string Domrename(string options) { return Bash.Execute($"virsh domrename {options}"); }

            /// <summary>
            /// TODO - low
            /// </summary>
            /// <param name="options"></param>
            /// <returns></returns>
            public static string Dompmsuspend(string options) { return Bash.Execute($"virsh dompmsuspend {options}"); }

            /// <summary>
            /// TODO - low
            /// </summary>
            /// <param name="options"></param>
            /// <returns></returns>
            public static string Dompmwakeup(string options) { return Bash.Execute($"virsh dompmwakeup {options}"); }

            public static string Domuuid(string options) { return Bash.Execute($"virsh domuuid {options}"); }
            public static string domxml_from_native(string options) { return Bash.Execute($"virsh domxml_from_native {options}"); }
            public static string domxml_to_native(string options) { return Bash.Execute($"virsh domxml_to_native {options}"); }
            public static string Dump(string options) { return Bash.Execute($"virsh dump {options}"); }
            public static string Dumpxml(string options) { return Bash.Execute($"virsh dumpxml {options}"); }
            public static string Edit(string options) { return Bash.Execute($"virsh edit {options}"); }
            public static string Event(string options) { return Bash.Execute($"virsh event {options}"); }
            public static string inject_nmi(string options) { return Bash.Execute($"virsh inject_nmi {options}"); }
            public static string Iothreadinfo(string options) { return Bash.Execute($"virsh iothreadinfo {options}"); }
            public static string Iothreadpin(string options) { return Bash.Execute($"virsh iothreadpin {options}"); }
            public static string Iothreadadd(string options) { return Bash.Execute($"virsh iothreadadd {options}"); }
            public static string Iothreaddel(string options) { return Bash.Execute($"virsh iothreaddel {options}"); }
            public static string send_key(string options) { return Bash.Execute($"virsh send_key {options}"); }
            public static string send_process_signal(string options) { return Bash.Execute($"virsh send_process_signal {options}"); }
            public static string lxc_enter_namespace(string options) { return Bash.Execute($"virsh lxc_enter_namespace {options}"); }
            public static string Managedsave(string options) { return Bash.Execute($"virsh managedsave {options}"); }
            public static string managedsave_remove(string options) { return Bash.Execute($"virsh managedsave_remove {options}"); }
            public static string Memtune(string options) { return Bash.Execute($"virsh memtune {options}"); }
            public static string Metadata(string options) { return Bash.Execute($"virsh metadata {options}"); }
            public static string Migrate(string options) { return Bash.Execute($"virsh migrate {options}"); }
            public static string migrate_setmaxdowntime(string options) { return Bash.Execute($"virsh migrate_setmaxdowntime {options}"); }
            public static string migrate_compcache(string options) { return Bash.Execute($"virsh migrate_compcache {options}"); }
            public static string migrate_setspeed(string options) { return Bash.Execute($"virsh migrate_setspeed {options}"); }
            public static string migrate_getspeed(string options) { return Bash.Execute($"virsh migrate_getspeed {options}"); }
            public static string Numatune(string options) { return Bash.Execute($"virsh numatune {options}"); }
            public static string qemu_attach(string options) { return Bash.Execute($"virsh qemu_attach {options}"); }
            public static string qemu_monitor_command(string options) { return Bash.Execute($"virsh qemu_monitor_command {options}"); }
            public static string qemu_monitor_event(string options) { return Bash.Execute($"virsh qemu_monitor_event {options}"); }
            public static string qemu_agent_command(string options) { return Bash.Execute($"virsh qemu_agent_command {options}"); }

            /// <summary>
            /// TODO
            /// </summary>
            /// <param name="options"></param>
            /// <returns></returns>
            public static string Reboot(string options) { return Bash.Execute($"virsh reboot {options}"); }

            /// <summary>
            /// TODO
            /// </summary>
            /// <param name="options"></param>
            /// <returns></returns>
            public static string Reset(string options) { return Bash.Execute($"virsh reset {options}"); }

            /// <summary>
            /// TODO
            /// </summary>
            /// <param name="options"></param>
            /// <returns></returns>
            public static string Restore(string options) { return Bash.Execute($"virsh restore {options}"); }

            /// <summary>
            /// TODO
            /// </summary>
            /// <param name="options"></param>
            /// <returns></returns>
            public static string Resume(string options) { return Bash.Execute($"virsh resume {options}"); }

            public static string Save(string options) { return Bash.Execute($"virsh save {options}"); }
            public static string save_image_define(string options) { return Bash.Execute($"virsh save_image_define {options}"); }
            public static string save_image_dumpxml(string options) { return Bash.Execute($"virsh save_image_dumpxml {options}"); }
            public static string save_image_edit(string options) { return Bash.Execute($"virsh save_image_edit {options}"); }
            public static string Schedinfo(string options) { return Bash.Execute($"virsh schedinfo {options}"); }
            public static string Screenshot(string options) { return Bash.Execute($"virsh screenshot {options}"); }
            public static string set_user_password(string options) { return Bash.Execute($"virsh set_user_password {options}"); }
            public static string Setmaxmem(string options) { return Bash.Execute($"virsh setmaxmem {options}"); }
            public static string Setmem(string options) { return Bash.Execute($"virsh setmem {options}"); }
            public static string Setvcpus(string options) { return Bash.Execute($"virsh setvcpus {options}"); }

            /// <summary>
            /// TODO
            /// </summary>
            /// <param name="options"></param>
            /// <returns></returns>
            public static string Shutdown(string options) { return Bash.Execute($"virsh shutdown {options}"); }

            /// <summary>
            /// TODO
            /// </summary>
            /// <param name="options"></param>
            /// <returns></returns>
            public static string Start(string options) { return Bash.Execute($"virsh start {options}"); }

            /// <summary>
            /// TODO
            /// </summary>
            /// <param name="options"></param>
            /// <returns></returns>
            public static string Suspend(string options) { return Bash.Execute($"virsh suspend {options}"); }

            /// <summary>
            /// TODO - low
            /// </summary>
            /// <param name="options"></param>
            /// <returns></returns>
            public static string Ttyconsole(string options) { return Bash.Execute($"virsh ttyconsole {options}"); }

            public static string Undefine(string options) { return Bash.Execute($"virsh undefine {options}"); }
            public static string update_device(string options) { return Bash.Execute($"virsh update_device {options}"); }
            public static string Vcpucount(string options) { return Bash.Execute($"virsh vcpucount {options}"); }
            public static string Vcpuinfo(string options) { return Bash.Execute($"virsh vcpuinfo {options}"); }
            public static string Vcpupin(string options) { return Bash.Execute($"virsh vcpupin {options}"); }
            public static string Emulatorpin(string options) { return Bash.Execute($"virsh emulatorpin {options}"); }

            /// <summary>
            /// TODO
            /// </summary>
            /// <param name="options"></param>
            /// <returns></returns>
            public static string Vncdisplay(string options) { return Bash.Execute($"virsh vncdisplay {options}"); }
        }

        public class Monitor {
            public static string Domblkerror(string options) { return Bash.Execute($"virsh domblkerror {options}"); }
            public static string Domblkinfo(string options) { return Bash.Execute($"virsh domblkinfo {options}"); }
            public static string Domblklist(string options) { return Bash.Execute($"virsh domblklist {options}"); }
            public static string Domblkstat(string options) { return Bash.Execute($"virsh domblkstat {options}"); }
            public static string Domcontrol(string options) { return Bash.Execute($"virsh domcontrol {options}"); }
            public static string domif_getlink(string options) { return Bash.Execute($"virsh domif_getlink {options}"); }
            public static string Domifaddr(string options) { return Bash.Execute($"virsh domifaddr {options}"); }
            public static string Domiflist(string options) { return Bash.Execute($"virsh domiflist {options}"); }
            public static string Domifstat(string options) { return Bash.Execute($"virsh domifstat {options}"); }
            public static string Dominfo(string options) { return Bash.Execute($"virsh dominfo {options}"); }
            public static string Dommemstat(string options) { return Bash.Execute($"virsh dommemstat {options}"); }
            public static string Domstate(string options) { return Bash.Execute($"virsh domstate {options}"); }
            public static string Domstats(string options) { return Bash.Execute($"virsh domstats {options}"); }
            public static string Domtime(string options) { return Bash.Execute($"virsh domtime {options}"); }
            public static string List(string options) { return Bash.Execute($"virsh list {options}"); }
        }

        public class Host {
            public static string Allocpages(string options) { return Bash.Execute($"virsh allocpages {options}"); }
            public static string Capabilities(string options) { return Bash.Execute($"virsh capabilities {options}"); }
            public static string cpu_models(string options) { return Bash.Execute($"virsh cpu_models {options}"); }
            public static string Domcapabilities(string options) { return Bash.Execute($"virsh domcapabilities {options}"); }
            public static string Freecell(string options) { return Bash.Execute($"virsh freecell {options}"); }
            public static string Freepages(string options) { return Bash.Execute($"virsh freepages {options}"); }
            public static string Hostname(string options) { return Bash.Execute($"virsh hostname {options}"); }
            public static string Maxvcpus(string options) { return Bash.Execute($"virsh maxvcpus {options}"); }
            public static string node_memory_tune(string options) { return Bash.Execute($"virsh node_memory_tune {options}"); }
            public static string Nodecpumap(string options) { return Bash.Execute($"virsh nodecpumap {options}"); }
            public static string Nodecpustats(string options) { return Bash.Execute($"virsh nodecpustats {options}"); }
            public static string Nodeinfo(string options) { return Bash.Execute($"virsh nodecpustats {options}"); }
            public static string Nodememstats(string options) { return Bash.Execute($"virsh nodememstats {options}"); }
            public static string Nodesuspend(string options) { return Bash.Execute($"virsh nodesuspend {options}"); }
            public static string Sysinfo(string options) { return Bash.Execute($"virsh sysinfo {options}"); }
            public static string Uri(string options) { return Bash.Execute($"virsh uri {options}"); }
            public static string Version(string options) { return Bash.Execute($"virsh version {options}"); }
        }

        public class Interface {
            public static string iface_begin(string options) { return Bash.Execute($"virsh iface_begin {options}"); }
            public static string iface_bridge(string options) { return Bash.Execute($"virsh iface_bridge {options}"); }
            public static string iface_commit(string options) { return Bash.Execute($"virsh iface_commit {options}"); }
            public static string iface_define(string options) { return Bash.Execute($"virsh iface_define {options}"); }
            public static string iface_destroy(string options) { return Bash.Execute($"virsh iface_destroy {options}"); }
            public static string iface_dumpxml(string options) { return Bash.Execute($"virsh iface_dumpxml {options}"); }
            public static string iface_edit(string options) { return Bash.Execute($"virsh iface_edit {options}"); }
            public static string iface_list(string options) { return Bash.Execute($"virsh iface_list {options}"); }
            public static string iface_mac(string options) { return Bash.Execute($"virsh iface_mac {options}"); }
            public static string iface_name(string options) { return Bash.Execute($"virsh iface_name {options}"); }
            public static string iface_rollback(string options) { return Bash.Execute($"virsh iface_rollback {options}"); }
            public static string iface_start(string options) { return Bash.Execute($"virsh iface_start {options}"); }
            public static string iface_unbridge(string options) { return Bash.Execute($"virsh iface_unbridge {options}"); }
            public static string iface_undefine(string options) { return Bash.Execute($"virsh iface_undefine {options}"); }
        }

        public class Filter {
            public static string nwfilter_define(string options) { return Bash.Execute($"virsh nwfilter_define {options}"); }
            public static string nwfilter_dumpxml(string options) { return Bash.Execute($"virsh nwfilter_dumpxml {options}"); }
            public static string nwfilter_edit(string options) { return Bash.Execute($"virsh nwfilter_edit {options}"); }
            public static string nwfilter_list(string options) { return Bash.Execute($"virsh nwfilter_list {options}"); }
            public static string nwfilter_undefine(string options) { return Bash.Execute($"virsh nwfilter_undefine {options}"); }
        }

        public class Network {
            public static string net_autostart(string options) { return Bash.Execute($"virsh net_autostart {options}"); }
            public static string net_create(string options) { return Bash.Execute($"virsh net_create {options}"); }
            public static string net_define(string options) { return Bash.Execute($"virsh net_define {options}"); }
            public static string net_destroy(string options) { return Bash.Execute($"virsh net_destroy {options}"); }
            public static string net_dhcp_leases(string options) { return Bash.Execute($"virsh net_dhcp_leases {options}"); }
            public static string net_dumpxml(string options) { return Bash.Execute($"virsh net_dumpxml {options}"); }
            public static string net_edit(string options) { return Bash.Execute($"virsh net_edit {options}"); }
            public static string net_event(string options) { return Bash.Execute($"virsh net_event {options}"); }
            public static string net_info(string options) { return Bash.Execute($"virsh net_info {options}"); }
            public static string net_list(string options) { return Bash.Execute($"virsh net_list {options}"); }
            public static string net_name(string options) { return Bash.Execute($"virsh net_name {options}"); }
            public static string net_start(string options) { return Bash.Execute($"virsh net_start {options}"); }
            public static string net_undefine(string options) { return Bash.Execute($"virsh net_undefine {options}"); }
            public static string net_update(string options) { return Bash.Execute($"virsh net_update {options}"); }
            public static string net_uuid(string options) { return Bash.Execute($"virsh net_uuid {options}"); }
        }

        public class Nodedev {
            public static string nodedev_create(string options) { return Bash.Execute($"virsh nodedev_create {options}"); }
            public static string nodedev_destroy(string options) { return Bash.Execute($"virsh nodedev_destroy {options}"); }
            public static string nodedev_detach(string options) { return Bash.Execute($"virsh nodedev_detach {options}"); }
            public static string nodedev_dumpxml(string options) { return Bash.Execute($"virsh nodedev_dumpxml {options}"); }
            public static string nodedev_list(string options) { return Bash.Execute($"virsh nodedev_list {options}"); }
            public static string nodedev_reattach(string options) { return Bash.Execute($"virsh nodedev_reattach {options}"); }
            public static string nodedev_reset(string options) { return Bash.Execute($"virsh nodedev_reset {options}"); }
        }

        public class Secret {
            public static string secret_define(string options) { return Bash.Execute($"virsh secret_define {options}"); }
            public static string secret_dumpxml(string options) { return Bash.Execute($"virsh secret_dumpxml {options}"); }
            public static string secret_get_value(string options) { return Bash.Execute($"virsh secret_get_value {options}"); }
            public static string secret_list(string options) { return Bash.Execute($"virsh secret_list {options}"); }
            public static string secret_set_value(string options) { return Bash.Execute($"virsh secret_set_value {options}"); }
            public static string secret_undefine(string options) { return Bash.Execute($"virsh secret_undefine {options}"); }
        }

        public class Snapshot {
            public static string snapshot_create(string options) { return Bash.Execute($"virsh snapshot_create {options}"); }
            public static string snapshot_create_as(string options) { return Bash.Execute($"virsh snapshot_create_as {options}"); }
            public static string snapshot_current(string options) { return Bash.Execute($"virsh snapshot_current {options}"); }
            public static string snapshot_delete(string options) { return Bash.Execute($"virsh snapshot_delete {options}"); }
            public static string snapshot_dumpxml(string options) { return Bash.Execute($"virsh snapshot_dumpxml {options}"); }
            public static string snapshot_edit(string options) { return Bash.Execute($"virsh snapshot_edit {options}"); }
            public static string snapshot_info(string options) { return Bash.Execute($"virsh snapshot_info {options}"); }
            public static string snapshot_list(string options) { return Bash.Execute($"virsh snapshot_list {options}"); }
            public static string snapshot_parent(string options) { return Bash.Execute($"virsh snapshot_parent {options}"); }
            public static string snapshot_revert(string options) { return Bash.Execute($"virsh snapshot_revert {options}"); }
        }

        public class Pool {
            public static string find_storage_pool_sources_as(string options) { return Bash.Execute($"virsh find_storage_pool_sources_as {options}"); }
            public static string find_storage_pool_sources(string options) { return Bash.Execute($"virsh find_storage_pool_sources {options}"); }
            public static string pool_autostart(string options) { return Bash.Execute($"virsh pool_autostart {options}"); }
            public static string pool_build(string options) { return Bash.Execute($"virsh pool_build {options}"); }
            public static string pool_create_as(string options) { return Bash.Execute($"virsh pool_create_as {options}"); }
            public static string pool_create(string options) { return Bash.Execute($"virsh pool_create {options}"); }
            public static string pool_define_as(string options) { return Bash.Execute($"virsh pool_define_as {options}"); }
            public static string pool_define(string options) { return Bash.Execute($"virsh pool_define {options}"); }
            public static string pool_delete(string options) { return Bash.Execute($"virsh pool_delete {options}"); }
            public static string pool_destroy(string options) { return Bash.Execute($"virsh pool_destroy {options}"); }
            public static string pool_dumpxml(string options) { return Bash.Execute($"virsh pool_dumpxml {options}"); }
            public static string pool_edit(string options) { return Bash.Execute($"virsh pool_edit {options}"); }
            public static string pool_info(string options) { return Bash.Execute($"virsh pool_info {options}"); }
            public static string pool_list(string options) { return Bash.Execute($"virsh pool_list {options}"); }
            public static string pool_name(string options) { return Bash.Execute($"virsh pool_name {options}"); }
            public static string pool_refresh(string options) { return Bash.Execute($"virsh pool_refresh {options}"); }
            public static string pool_start(string options) { return Bash.Execute($"virsh pool_start {options}"); }
            public static string pool_undefine(string options) { return Bash.Execute($"virsh pool_undefine {options}"); }
            public static string pool_uuid(string options) { return Bash.Execute($"virsh pool_uuid {options}"); }
        }

        public class Volume {
            public static string vol_clone(string options) { return Bash.Execute($"virsh vol_clone {options}"); }
            public static string vol_create_as(string options) { return Bash.Execute($"virsh vol_create_as {options}"); }
            public static string vol_create(string options) { return Bash.Execute($"virsh vol_create {options}"); }
            public static string vol_create_from(string options) { return Bash.Execute($"virsh vol_create_from {options}"); }
            public static string vol_delete(string options) { return Bash.Execute($"virsh vol_delete {options}"); }
            public static string vol_download(string options) { return Bash.Execute($"virsh vol_download {options}"); }
            public static string vol_dumpxml(string options) { return Bash.Execute($"virsh vol_dumpxml {options}"); }
            public static string vol_info(string options) { return Bash.Execute($"virsh vol_info {options}"); }
            public static string vol_key(string options) { return Bash.Execute($"virsh vol_key {options}"); }
            public static string vol_list(string options) { return Bash.Execute($"virsh vol_list {options}"); }
            public static string vol_name(string options) { return Bash.Execute($"virsh vol_name {options}"); }
            public static string vol_path(string options) { return Bash.Execute($"virsh vol_path {options}"); }
            public static string vol_pool(string options) { return Bash.Execute($"virsh vol_pool {options}"); }
            public static string vol_resize(string options) { return Bash.Execute($"virsh vol_resize {options}"); }
            public static string vol_upload(string options) { return Bash.Execute($"virsh vol_upload {options}"); }
            public static string vol_wipe(string options) { return Bash.Execute($"virsh vol_wipe {options}"); }
        }

        public class Self {
            public static string Cd(string options) { return Bash.Execute($"virsh cd {options}"); }
            public static string Echo(string options) { return Bash.Execute($"virsh echo {options}"); }
            public static string Exit(string options) { return Bash.Execute($"virsh exit {options}"); }
            public static string Help(string options) { return Bash.Execute($"virsh help {options}"); }
            public static string Pwd(string options) { return Bash.Execute($"virsh pwd {options}"); }
            public static string Quit(string options) { return Bash.Execute($"virsh quit {options}"); }
            public static string Connect(string options) { return Bash.Execute($"virsh connect {options}"); }
        }
    }
}
