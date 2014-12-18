all: antd

antd:
	xbuild Antd/Antd.csproj

clean:
	rm -rf Antd/bin

run: antd
	mono Antd/bin/Debug/Antd.exe
