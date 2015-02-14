#                 Ant System Daemon
#  Copyright (C) Copyright (c) 2014-2015, Anthilla S.r.l.
#

.PHONY : clean

all: antd

nuget:
	wget http://nuget.org/nuget.exe
	cp nuget.exe ./.nuget/NuGet.exe
	chmod a+x ./.nuget/NuGet.exe

antd: nuget
	xbuild Antd/Antd.csproj /p:Configuration=Release

clean:
	rm -rf Antd/bin

run: antd
	mono Antd/bin/Release/Antd.exe
