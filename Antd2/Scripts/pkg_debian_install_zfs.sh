#apt-get update
#apt-get --yes install linux-headers-`uname -r`
#apt-get --yes install buster-backports
#apt-get --yes install dkms
#apt-get --yes install spl-dkms
#apt-get --yes install buster-backports
#apt-get --yes install zfs-dkms
#apt-get --yes install zfsutils-linux

sed -i 's/main/main contrib non-free/g' /etc/apt/sources.list
apt-get update
apt -y install linux-headers-$(uname -r)
ln -s /bin/rm /usr/bin/rm
apt-get -y install zfs-dkms
