# Network routines
. /etc/kerbynet.conf
function getDEV () {
  IP="$1"
  DEV=`ip route get "$IP" |sed -r  's/.*[ ]dev[ ](.*[! ])[ ].*/\1/' |head -1`
  if [ -z "$DEV" ] ; then
    exit 2 
  else  
    echo $DEV
  fi 
}
function gethostbyname () {
  export Hname="$1"
  if ! perl -e 'use Socket;use Net::hostent;$h=gethostbyname($ENV{'Hname'});printf "%s\n",inet_ntoa($h->addr)' 2>/dev/null ; then
    echo "DNS lookup error." 1>&2
  fi
}
function getbroadcast () {
  ipcalc -b -n $1/$2 |grep Broadcast: |awk '{print $2}'
}
function canonicMac () {
   echo $1 |awk -F: '{printf "%02s%02s%02s%02s%02s%02s",toupper($1),toupper($2),toupper($3),toupper($4),toupper($5),toupper($6)}'
}
function getmac () {
   if MAC=`cat /sys/class/net/$1/address 2>/dev/null` ; then
     canonicMac "$MAC" 
   else
     echo 000000000000
   fi
}
function doBackup() {
#   /usr/local/bin/sudo tar -C "$REGISTER/system/net" -c -j -f "$REGISTER/system/net.bck" . || rm -f "$REGISTER/system/net.bck" 
 : 
}
function setallbridgedinterface() {
  BR=$1
  if [ "${BR:0:6}" == BRIDGE ] ; then
    for I in `$SCRIPTS/net_interfacesbridged $BR` ; do
      $SCRIPTS/setinterfaceforbridge $I
    done
  fi
}
function setallbondedinterface() {
  BD=$1
  if [ "${BD:0:4}" == BOND ] ; then
    for I in `$SCRIPTS/net_interfacesbonded $BD` ; do
      $SCRIPTS/setinterfaceforbond $I
    done
  fi
}
function removezombievlan () {
  INTER=$1
  CONFIG="$REGISTER/system/net/interfaces/"
  if [ -r $CONFIG/$INTER/Bridge/Name ] ; then
    VDIR=$CONFIG/`cat $CONFIG/$INTER/Bridge/Name`/VLAN
  else
    if [ -r $CONFIG/$INTER/Bond/Name ] ; then
      BONDNAME=`cat $CONFIG/$INTER/Bond/Name`
      if [ -r $CONFIG/$BONDNAME/Bridge/Name ] ; then
        VDIR=$CONFIG/`cat $CONFIG/$BONDNAME/Bridge/Name`/VLAN
      else
        VDIR=$CONFIG/$BONDNAME/VLAN
      fi
    else
      VDIR=$CONFIG/$INTER/VLAN
    fi
  fi
  ACTIVEVLAN=`ip link |grep $INTER | sed -r "s/.*$INTER\.([0123456789]*):.*/\1/" |grep -v $INTER`
  for V in $ACTIVEVLAN ; do
    if ! [ -d $VDIR/$V ] ; then
       ifconfig $INTER.$V down
       vconfig rem $INTER.$V
    fi
  done
}
function ext_bridgename() {
  BR=$1
  BRLIST=''
  if [ "${BR:0:6}" == BRIDGE ] ; then
    for I in `cat "$REGISTER/system/net/interfaces/$BR/BridgeList"` ; do
      BRLIST="$BRLIST,$I"
    done
    echo ${BRLIST:1}
  fi
}
function ext_bondname() {
  BD=$1
  BDLIST=''
  if [ "${BD:0:4}" == BOND ] ; then
    for I in `cat "$REGISTER/system/net/interfaces/$BD/BondList"` ; do
      BDLIST="$BDLIST,$I"
    done
    echo ${BDLIST:1}
  fi
}
function IsValidIPAddr () {
  IP="$1"
  if ! echo "$IP" | egrep -q "^[0123456789]{1,3}\.[0123456789]{1,3}\.[0123456789]{1,3}\.[0123456789]{1,3}$" ; then
    return 1
  fi
  N=`echo "$IP" | awk -F. '{print $1}'`
  [ "$N" -ge 0 -a "$N" -le 255 ] || return 1
  N=`echo "$IP" | awk -F. '{print $2}'`
  [ "$N" -ge 0 -a "$N" -le 255 ] || return 1
  N=`echo "$IP" | awk -F. '{print $3}'`
  [ "$N" -ge 0 -a "$N" -le 255 ] || return 1
  N=`echo "$IP" | awk -F. '{print $4}'`
  [ "$N" -ge 0 -a "$N" -le 255 ] || return 1
  return 0
}
