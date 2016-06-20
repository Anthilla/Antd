#!/bin/bash
CONFIG=/etc/usb_modeswitch.conf
cp /etc/usb_modeswitch.conf /tmp/usb_modeswitch.conf
rm -rf /tmp/usb_modeswitch
mkdir /tmp/usb_modeswitch

echo "### LAST LINE ###" >> /tmp/usb_modeswitch.conf
awk '/;DefaultVendor/ {
   I++;
   FL="/tmp/usb_modeswitch/" I
   printf("%s\n",substr($0,2)) > FL
   getline L
   while (substr(L,1,3)!="###") {
     if (substr(L,1,1)==";") L=substr(L,2) 
     printf("%s\n",L) > FL
     getline L
   }
}' < /tmp/usb_modeswitch.conf

for C in /tmp/usb_modeswitch/* ; do
  echo $C
  usb_modeswitch -c $C 
done
exit 1

