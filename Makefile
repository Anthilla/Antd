all: antd

antd:
	xbuild Antd/Antd.csproj

clean:
	rm -rf Antd/bin
