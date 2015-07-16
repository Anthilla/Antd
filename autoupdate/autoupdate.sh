echo Cleaning old files...
rm -fR /mnt/cdrom/Scripts/Autoupdate/tmp/*
echo Installing Antd...
mkdir -p /mnt/cdrom/Scripts/Autoupdate/tmp
cd /mnt/cdrom/Scripts/Autoupdate/tmp
wget "https://github.com/Anthilla/Antd/raw/master/autoupdate/DIR_framework_antdsh.squashfs.xz"
mkdir -p /framework/antdsh
mkdir -p /mnt/cdrom/Apps
mv /mnt/cdrom/Scripts/Autoupdate/tmp/DIR_framework_antdsh.squashfs.xz /mnt/cdrom/Apps/DIR_framework_antdsh.squashfs.xz
mount /mnt/cdrom/Apps/DIR_framework_antdsh.squashfs.xz /framework/antdsh
mono /framework/antdsh/antdsh.exe update-url
echo Antd Installed
echo I have to wait some seconds...
sleep 30
mono /framework/antdsh/antdsh.exe status



echo Finished
