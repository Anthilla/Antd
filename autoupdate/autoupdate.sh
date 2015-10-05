
echo Cleaning old files...
umount -t tmpfs /mnt/cdrom/Apps/tmp
rm -fR /mnt/cdrom/Apps/tmp
rm /mnt/cdrom/Apps/Anthilla_antdsh/active-version

echo Installing Antd...
mkdir -p /mnt/cdrom/Apps/tmp
mount -t tmpfs tmpfs /mnt/cdrom/Apps/tmp

wget "http://srv.anthilla.com:8081/antdsh-update/update.txt" -O /mnt/cdrom/Apps/tmp/update.txt
SQUASH=$(tail -1 /mnt/cdrom/Apps/tmp/update.txt)
echo "version found: $SQUASH"
wget "http://srv.anthilla.com:8081/antdsh-update/$SQUASH" -O "/mnt/cdrom/Apps/Anthilla_antdsh/$SQUASH"
mkdir -p /framework/antdsh
mkdir -p /mnt/cdrom/Apps/Anthilla_antdsh
ls -la /mnt/cdrom/Apps/Anthilla_antdsh
umount -t tmpfs /mnt/cdrom/Apps/tmp
rm -fR /mnt/cdrom/Apps/tmp

echo Linking newest version
ln -s /mnt/cdrom/Apps/Anthilla_antdsh/$SQUASH /mnt/cdrom/Apps/Anthilla_antdsh/active-version

echo Mounting antdsh
mount /mnt/cdrom/Apps/Anthilla_antdsh/active-version /framework/antdsh
mono /framework/antdsh/antdsh.exe update

echo Antd Installed

echo I have to wait some seconds...
sleep 30
mono /framework/antdsh/antdsh.exe status



echo Finished!
