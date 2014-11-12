ANTD
====

About
-----

Antd would be a appication and configuration init and configuration daemon.
thinking about a linux/unix system as "applicative appliance system", Antd can be invoked by the base init system after the base boot of the system to a known minimal runlevel
and execute base and applicative processes, settingd, and check the configuration, both locally or distributed and use and internal nosql database for complex elaborations
to start, control, check, monitor of the system and application stack.
 
Antd can work with systemd or other init systems.

- Antd is wrote in c#
- Antd is based on [Mono](http://www.mono-project.com/) and [Nancy](https://github.com/NancyFx/Nancy/wiki/Introduction) framework
- Antd is wrote as web service with API REST service and Rendered web interface.
- Antd is relead under BSD-3 license
- Antd is self hosted, no external webserver is required.

Antd would be a single daemon to maintain the linux/unix appliance system (processes, clustering, configuration, configuration store, configuration dispatching, monitoring)
using, when is possible direct command or kernel related userspace commands or direct access to pseudo filesystems (linux) like /proc /sys and dbus implementatons.

### Target Achievement
- total administration, easy to use to inspecting journals and starting and stopping services.
- swiss knife for administration configuration and monitoring
- single machine to big clustered enviroments or distributed installations
- de-structured clustering (orchestrated executions and management of processes and application on distributed enviroments
- capable of full machine control and check
- unique interface, from low level to highest application level
- system independent / system architecture independent

### Antd is inspired by:
- [Webmin](http://www.webmin.com/)
- [LuCI OpenWRT](http://wiki.openwrt.org/doc/howto/luci.essentials)
- [Cockipt Project](http://cockipt-project.org)
- [CoreOS](https://coreos.com/using-coreos/) [fleet and etcd](https://github.com/coreos/fleet/blob/master)

Getting Started
---------------

1. download the source files
2. import nuget packages necessary to build with "Restore nuget packages" available both in Mono Develop + nuget and Visual Studio.
3. in a linux installation install mono (minimal 3.2.1)
4. build it with xbuild or from ide
5. Execute it.
 
The Antd API uses JSON over HTTP

How to Contribute
-----------------

The projects is in very early stage.
Help us and propose ideas and ways how organize the project.

### Communicate by Email

    Email: osdev@anthilla.com

### Getting Started

    Fork the repository on GitHub
    Play with the project, submit bugs, submit patches, Submit extensions, Submit Ideas(!!!),

### Contribution Flow

This is a rough outline of what a contributor's workflow looks like:

    Create a topic branch from where you want to base your work (usually master).
    Make commits of logical units.
    Make sure your commit messages are in the proper format (see below).
    Push your changes to a topic branch in your fork of the repository.
    Make sure the tests pass, and add any new tests as appropriate.
    Submit a pull request to the original repository.

Thanks for your contributions!

Antd is released under the BSD-3 license. See the LICENSE file for details.

Specific components of Antd use code derivative from software distributed under other licenses; in those cases the appropriate licenses are stipulated alongside the code.

>
>
> Copyright (c) 2014, [Anthilla S.r.l.] (http://www.anthilla.com)
>
> All rights reserved.
>
> Redistribution and use in source and binary forms, with or without
> modification, are permitted provided that the following conditions are met:
>     * Redistributions of source code must retain the above copyright
>       notice, this list of conditions and the following disclaimer.
>     * Redistributions in binary form must reproduce the above copyright
>       notice, this list of conditions and the following disclaimer in the
>       documentation and/or other materials provided with the distribution.
>     * Neither the name of the Anthilla S.r.l. nor the
>       names of its contributors may be used to endorse or promote products
>       derived from this software without specific prior written permission.
>
> THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
> ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
> WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
> DISCLAIMED. IN NO EVENT SHALL ANTHILLA S.R.L. BE LIABLE FOR ANY
> DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
> (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
> LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
> ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
> (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
> SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
>
>
>