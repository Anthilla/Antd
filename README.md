Antd
====

[![Join the chat at https://gitter.im/Anthilla/Antd](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/Anthilla/Antd?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

<a href="https://scan.coverity.com/projects/antd">
  <img alt="Coverity Scan Build Status"
       src="https://scan.coverity.com/projects/6211/badge.svg"/>
</a>

About
-----
 
Antd aims to be an application/configuration init and a configuration daemon. Antd can be invoked by the base init system just after entering a known minimum runlevel during boot. Antd will then execute base and applicative processes, settings, and then check the configuration. Antd can work both locally and in distributed environments. Antd uses noSQL for complex data management to start, control, and mmonitor parts of the system and application stack. Antd can also work with systemd or other init systems.

Antd is:
- written in C#
- based on [Mono](http://www.mono-project.com/) and [Nancy](https://github.com/NancyFx/Nancy) frameworks
- implemented as web service with REST API service and rendered web interface
- released under the BSD 3 license
- self hosted, no external webserver is needed

Antd is a single daemon to maintain the entire Linux/Unix appliance system (processes, clustering, configuration, and monitoring) using, whenever possible, shell commands, kernel related userspace commands, or direct access to Unix pseudo filesystems like <code>/proc</code>, <code>/sys</code>, and dbus.

### Project Goals
- Complete administration capabilities
- Easy to use for inspecting logs and managing services
- Swiss Knife all-in-one for administration, configuration and monitoring
- Scales from one machine to big cluster environments.
- Orchestrated execution and management of processes and applications in distributed environments
- Full machine administration and monitoring
- Unique interface throughout the application
- Hardware/OS/architecture independent

### Antd is inspired by:
- [Webmin](http://www.webmin.com/)
- [LuCI OpenWRT](http://wiki.openwrt.org/doc/howto/luci.essentials)
- [Cockipt Project](http://cockipt-project.org)
- [CoreOS](https://coreos.com/using-coreos/) [fleet and etcd](https://github.com/coreos/fleet/blob/master)
- [Salt Stack](http://saltstack.com/community/) and its sub project [Salt Virt](http://docs.saltstack.com/en/latest/topics/virt/)
- [Witsbits](http://witsbits.com/)
- Any other kind of distributed configuration manager and system synchronizer out there

### Dependencies:
- [C5](https://github.com/sestoft/C5/)
- [DensoDb](https://github.com/ppossanzini/DensoDB) v3
- [jQuery](https://github.com/jquery/jquery)
- [Owin](https://github.com/owin/owin)
- [Nancy](https://github.com/NancyFx/Nancy)
- [Newtonsoft Json](https://github.com/JamesNK/Newtonsoft.Json)
- [Metro UI](https://github.com/olton/Metro-UI-CSS) 

Getting Started
---------------

<b>Windows</b>

1. Download the source files from GitHub, or use `git clone https://github.com/Anthilla/Antd.git`

2. Import nuget packages necessary to build with "Restore nuget packages" available both in Mono Develop + nuget and Visual Studio.
3. Run it!
 
<b>Gentoo Linux</b>

1. Add the dotnet repository (`layman -a dotnet`)
2. `emerge antd`
3. Run it! (`antd` or `sudo antd` for root permissions)

<b>Other Linux</b>

1. Install mono (3.2.1 or higher)
2. Build it with xbuild or from an IDE
3. Run it!
 
The Antd API uses JSON over HTTP.

How to Contribute
-----------------

This project is still in its early stages of development. Please help us by sharing ideas to help organize and improve the project!

### Communicate by Email

Email: osdev@anthilla.com

### IM

Freenode IRC channel: #anthilla

Gitter: [![Join the chat at https://gitter.im/Anthilla/Antd](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/Anthilla/Antd?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

### Getting Started

Fork the repository on GitHub using `git clone https://github.com/Anthilla/Antd.git`. 
Play with the code, submit patches, and file bugs when you come across them. Don't forget to send your ideas and feature requests too!

### Contribution Flow

This is an outline of what a contributor's workflow should look like:

- Create a new branch or fork the repository from where you want to start your work (usually Anthilla/Antd:master)
- Edit the code, testing your changes as you go
- Create a commit, making sure that your commit messages follow **OUR** standards (see below)
- Push your changes to your own branch/forked repository
- Ensure your code is stable and free of any application-breaking bugs
- Submit a pull request

Please be patient as it may take some time for us to review your code. If we have any questions, we will notify you through the active pull request.

Thank you for your contributions!

Antd is released under the BSD 3 clause license. See the LICENSE file for details (included with the application).

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
