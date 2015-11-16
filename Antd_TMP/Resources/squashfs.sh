echo Removing existing squashes
rm *.squashfs.xz
ANTD_Y=`date +%Y`
ANTD_m=`date +%m`
ANTD_d=`date +%d`
echo Create squashfs for Antd ${ANTD_Y}${ANTD_m}${ANTD_d}
mksquashfs antd DIR_framework_antd-${ANTD_Y}${ANTD_m}${ANTD_d}.squashfs.xz -comp xz -Xbcj x86 -Xdict-size 75%
sleep 10
echo Create squashfs for antdsh
mksquashfs antdsh DIR_framework_antdsh.squashfs.xz -comp xz -Xbcj x86 -Xdict-size 75%
unset ANTD_VERSION
