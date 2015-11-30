### pre execute start
echo Cleaning old tmp env...
umount -t tmpfs /mnt/cdrom/Apps/tmp
rm -fR /mnt/cdrom/Apps/tmp

echo Prepare tmp env for Antdsh update...
mkdir -p /mnt/cdrom/Apps/tmp
mount -t tmpfs tmpfs /mnt/cdrom/Apps/tmp
mkdir -p /framework/antdsh

echo verify Antdsh version
wget "http://srv.anthilla.com:8081/update.antdsh/update.txt" -O /mnt/cdrom/Apps/tmp/update.txt
SQUASH=$(head -1 /mnt/cdrom/Apps/tmp/update.txt)
echo "version found: $SQUASH"

echo Download Antdsh
mkdir -p /mnt/cdrom/Apps/Anthilla_antdsh
wget "http://srv.anthilla.com:8081/update.antdsh/$SQUASH" -O "/mnt/cdrom/Apps/Anthilla_antdsh/$SQUASH"

echo kill active antdsh
killall antdsh.exe

echo unmount old antdsh
umount /framework/antdsh

echo Linking newest version
rm /mnt/cdrom/Apps/Anthilla_antdsh/active-version
ln -s /mnt/cdrom/Apps/Anthilla_antdsh/$SQUASH /mnt/cdrom/Apps/Anthilla_antdsh/active-version

echo cleanup
umount -t tmpfs /mnt/cdrom/Apps/tmp
rm -fR /mnt/cdrom/Apps/tmp
### pre execute end

### execute start
echo Mounting antdsh
mount /mnt/cdrom/Apps/Anthilla_antdsh/active-version /framework/antdsh
mono /framework/antdsh/antdsh.exe update

echo Antd Installed
echo I have to wait some seconds...
sleep 10
mono /framework/antdsh/antdsh.exe status

echo Finished!
### execute end
