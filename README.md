Antd
====

About
-----

Antd aims to be an application & configuration init and a configuration daemon.
Thinking about a Linux/Unix system as "applicative appliance system", Antd can be invoked by the base init system just after the base boot of the system into a known minimal runlevel
and execute base and applicative processes, settings, and check the configuration. It can work both locally and in distributed environments and uses and internal nosql database for complex data management and 
to start, control, check, monitor of the system and application stack.
 
Antd can work with systemd or other init systems.

- Antd is written in C#
- Antd is based on [Mono](http://www.mono-project.com/) and [Nancy](https://github.com/NancyFx/Nancy) frameworks
- Antd is implemented as web service with REST API service and rendered web interface
- Antd is released under the BSD 3 clause license
- Antd is self hosted, no external webserver is needed

Antd would be a single daemon to maintain the linux/unix appliance system (processes, clustering, configuration, configuration store, configuration dispatching, monitoring)
using, whenever possible, shell commands, kernel related userspace commands or direct access to pseudo filesystems (linux) like /proc /sys and dbus.

### Target Achievements
- total administration, easy to use for inspecting journals and starting and stopping services.
- swiss knife for administration, configuration and monitoring
- single machine to big clustered enviroments or distributed installations scalability
- de-structured clustering (orchestrated executions and management of processes and application on distributed enviroments
- capable of full machine control and check
- unique interface, from low level to highest application level
- system independent / system architecture independent

### Antd is inspired by:
- [Webmin](http://www.webmin.com/)
- [LuCI OpenWRT](http://wiki.openwrt.org/doc/howto/luci.essentials)
- [Cockipt Project](http://cockipt-project.org)
- [CoreOS](https://coreos.com/using-coreos/) [fleet and etcd](https://github.com/coreos/fleet/blob/master)

### References:
- [C5](https://github.com/sestoft/C5/)
- [DensoDb](https://github.com/ppossanzini/DensoDB)
- [jQuery](https://github.com/jquery/jquery)
- [Owin](https://github.com/owin/owin)
- [Nancy](https://github.com/NancyFx/Nancy)
- [Newtonsoft Json](https://github.com/JamesNK/Newtonsoft.Json)
- [Metro UI](https://github.com/olton/Metro-UI-CSS)

Getting Started
---------------

<b>Windows</b>

1. download the source files
2. import nuget packages necessary to build with "Restore nuget packages" available both in Mono Develop + nuget and Visual Studio.
3. run it!
 
<b>Gentoo Linux</b>

1. add dotnet repository (`layman -a dotnet`)
2. `emerge antd`
3. run it! (`antd` or `sudo antd` for root permissions)

<b>Any Linux</b>

1. install mono (3.2.1 or higher)
2. build it with xbuild or from the IDE
3. run it!
 
The Antd API uses JSON over HTTP

How to Contribute
-----------------

The project is in a very early stage.
Help us sharing ideas and ways to organize and improve the project!

### Communicate by Email

Email: osdev@anthilla.com

### Getting Started

Fork the repository on GitHub
Play with the project, submit bugs, submit patches, submit extensions, submit ideas (!!!),

### Contribution Flow

This is a rough outline of what a contributor's workflow looks like:

Create a topic branch from where you want to base your work (usually master).
Make commits of logical units.
Make sure your commit messages are in the proper format (see below).
Push your changes to a topic branch in your fork of the repository.
Make sure the tests pass, and add any new tests as appropriate.
Submit a pull request to the original repository.

Thanks for your contributions!

Antd is released under the BSD 3 clause license. See the LICENSE file for details.

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
