echo "Check machine status"
echo "  machine system:"
echo "  $(uname -a)"
echo ""
echo "Check pkg: mono"
echo "  $(whereis mono)"
echo "Check pkg: dotnet"
echo "  $(whereis dotnet)"
echo ""
echo "Check network"
ping -c 8.8.8.8
echo "Check network - dns"
ping -c google.com
