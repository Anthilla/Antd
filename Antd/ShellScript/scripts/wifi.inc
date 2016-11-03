[ -z "$SCRIPTS" ] && . /etc/kerbynet.conf
WIFIPATH=/root/kerbynet.cgi
MADWIFI_MAXVAPS=4
DRIVER=`cat $REGISTER/system/net/WiFi/module 2>/dev/null`
if ! echo "$PATH" | grep -q "$WIFIPATH" ; then
  export PATH=$WIFIPATH/scripts/:$PATH
fi
function wifi-listvapsbymac {
  eths=`( cd $REGISTER/system/net/interfaces/ ; ls -d WLAN* 2>/dev/null )`
  for e in $eths ; do
    if [ "`cat $REGISTER/system/net/interfaces/$e/WiFi/DevID 2>/dev/null`" = "$1" ] ; then
      echo $e
    fi
  done
}
function getAllowedPower {
  # BROKEN WITH NEWER KERNEL
  return 0
  ALLOWEDPWR=""
  for P in `seq 0 100` ; do
    iwconfig $1 txpower $P
    TXPW=`iwconfig $1 |grep Tx-Power= | sed -r 's/.*Tx-Power=(.*) dBm.*/\1/'`
    echo $P $TXPW
    if [ "$P" = "$TXPW" ] ; then
      ALLOWEDPWR="$ALLOWEDPWR $P"
      MAXTXPOWER=$P
    fi
  done
  iwconfig $1 txpower `cat $REGISTER//system/net/WiFi/DEVS/$1/TXPOWER 2>/dev/null`
  echo $MAXTXPOWER $ALLOWEDPWR
}
function setregdomain {
   REGDOMAIN=`cat $REGISTER/system/net/WiFi/REG 2>/dev/null`
   if [ -n "$REGDOMAIN" ] ; then
     iw reg set $REGDOMAIN   
   else
     echo
     echo "WARNING: Regulatory Domain has not be set by using wifi-manager"
     echo
   fi
}
function readvalue {
# legge il valore dato dall'utente,
# controlla che questo sia nel range richiesto,
# da in uscita il valore 
# in tutto maiuscolo (up)
# in tutto minuscolo (lo)
# oppure unvaloe numerico.

  jmsg="$1"
  jdefault="$2"
  jallowed="$3"
  jmode="$4"
  jnum="$5"
  jopt="$6"
  jmin="$7"
  jmax="$8"
  jrepeat=yes
  while [ "$jrepeat" == yes ] ; do
    if [ -z "$jdefault" ] ; then
      echo -n "$jmsg: "
    else
      echo -n "$jmsg [$jdefault]: "
    fi
    read jvalue
    if [ -z "$jvalue" ] ; then
      jvalue="$jdefault"
      jrepeat=no
    else
      if [ -n "$jallowed" ] ; then
        if echo "$jallowed" | grep -q -i -w "$jvalue" ; then
          jrepeat=no
        fi
      else
        jrepeat=no
      fi
    fi
    if [ -z "$jvalue" ] && echo "$jopt" | grep -i -w -q RejectNull ; then
       jrepeat=yes
    fi
    if [ -n "$jmin" -a -n "$jmax" ] ; then
      jlen=` echo -n "$jvalue" |wc -m`
      if ! [  $jlen -ge $jmin -a $jlen -le $jmax ] ; then
        echo "The size of this value must be between $jmin and $jmax."
        echo
        jrepeat=yes
      fi
    fi
  done
  [ "$jmode" == up ] && jvalue=`echo $jvalue | awk '{print toupper($0)}'`
  [ "$jmode" == lo ] && jvalue=`echo $jvalue | awk '{print tolower($0)}'`
  [ "$jmode" == num ] && jvalue=`echo $jnum | cut -f $jvalue -d ' '`
  READVALUE="$jvalue"
}

function wifi-mac2vap {
# risale al nome del virtual ap dal mac
   pmac="`echo $1 | sed 's/://g' 2>/dev/null | awk '{print toupper($0)}'`"
   cvaps=`( cd /sys/class/net/ ; ls -d WLAN[0123456789][0123456789] )`
   for cvap in $cvaps ; do
     cmac="`sed 's/://g' < /sys/class/net/$cvap/address 2>/dev/null | awk '{print toupper($0)}'`"
     if [ "$cmac" == "$pmac" ] ; then
        echo $cvap
        return
     fi
   done
}
function wifi-mac2dev {
# risale al nome del device fisico partendo dal mac del virtual ap
   pmac="`echo $1 | sed 's/://g' 2>/dev/null | awk '{print toupper($0)}'`"
   cdevs=`( cd /sys/class/net/ ; ls -d wlan* )`
   for cdev in $cdevs ; do
     cmac="`sed 's/://g' < /sys/class/net/$cdev/phy80211/addresses 2>/dev/null | awk '{print toupper($0)}'`"
     if [ "$cmac" == "$pmac" ] ; then
        echo $cdev
     fi
   done
}
function wifi-create-vaps {
# crea tutti i virtual ap
  echo "Creating virtual WiFi interfaces:"
  wifi-destroy-all
  USEDDEVS=""
  cvSTA=""
  APCREATED=no
  cvINTERFACES=`(cd "$REGISTER/system/net/interfaces" ; ls -d WLAN[0123456789][0123456789] 2>/dev/null)`
  for cvI in $cvINTERFACES ; do
     set-wifi $cvI
  done
  if [ "$1" != norestart ] ; then
     $SCRIPTS/dhcp_start noconfig
     $SCRIPTS/dns_hup
  fi 
}
function driver-load {
  if ! ls /sys/class/net/wlan? 2>/dev/null >/dev/null ; then
    echo "PROCESS ABORTED:"
    echo "        No Wi-Fi compatible chipset has been found."
    echo
    return 2
  else
    ldevs=`wifi-list`
    if [ -n "$ldevs" ] ; then
      echo -n "Configuring physical WiFi devices ( "
      for ldev in $ldevs ; do
        echo -n "$ldev "
        mkdir -p $REGISTER/system/net/WiFi/DEVS/$ldev
        if ! [ -f "$REGISTER/system/net/WiFi/DEVS/$ldev/ID" ] ; then
          sed 's/://g' < /sys/class/net/$ldev/address 2>/dev/null | awk '{print toupper($0)}' > "$REGISTER/system/net/WiFi/DEVS/$ldev/ID"
        fi
        if ! [ -f "$REGISTER/system/net/WiFi/DEVS/$ldev/CHANNEL" ] ; then
          wifiN=`echo $ldev |sed -r "s/wlan(.*)/\1/"`
          echo "$(($wifiN%3*5+1))" > "$REGISTER/system/net/WiFi/DEVS/$ldev/CHANNEL"
        fi
      done 
      echo ")"
    fi
    return
  fi 
}
function driver-unload {
#  rmmod $DRIVER
   :
}
function wifi-createdefaultconfig {
# funzione di creazione del configurazione standard del ap
  [ -z "$1" ] && return
  CONFIG="$REGISTER/system/net/interfaces/$1/WiFi"
  if ! [ -d "$CONFIG" ] ; then
    echo "Creating default WiFi configuration for the interface $1 ..."
    mkdir $REGISTER/system/net/interfaces/$1/WiFi
    echo sta > $REGISTER/system/net/interfaces/$1/WiFi/Mode
    echo ANY > $REGISTER/system/net/interfaces/$1/WiFi/SSID
  fi
}

function wifi-list {
# funzione che fa la lista dei device fisici
  ( cd /sys/class/net/ ; ls -d wlan* 2>/dev/null )
}
function wifi-listvaps {
# funzione che da la lista di tutti i virtual ap
  eths=`( cd $REGISTER/system/net/interfaces/ ; ls -d WLAN* 2>/dev/null )`
  if [ -z "$1" ] ; then
    for e in $eths ; do
      echo $e
    done
  else
# lista di tutti i virtual ap che condividono la scheda fisica con quello dati in $i
    devmac=`sed 's/://g' < /sys/class/net/$1/phy80211/addresses 2>/dev/null | awk '{print toupper($0)}'`
    for e in $eths ; do
      if [ -d "$REGISTER/system/net/interfaces/$e/WiFi/" ] ; then
        vmac=`(cat $REGISTER/system/net/interfaces/$e/WiFi/DevID 2>/dev/null)`
        if [ "$vmac" == "$devmac" ] ; then
          echo $e
        fi
      fi
    done
  fi
}
function wifi-liststa {
# lista di tutti i virtual ap che sono in modalità  sta
  lsVAPS=`wifi-listvaps $1`
  lsSTAS=""
  for lsV in $lsVAPS ; do
    if [ "`cat $REGISTER/system/net/interfaces/$lsV/WiFi/Mode 2>/dev/null`" == sta ] ; then
      lsSTAS="$lsSTAS $lsV"
    fi
  done
  echo $lsSTAS
}
function wifi-ssid-rnd {
# crea random ssid
  echo "SSID-`rand`"
}
function disassociate {
# funzione che disassocia la scheda data in $1 dal suo ap
  disINTERFACE="$1"
  if [ "`cat $REGISTER/system/net/interfaces/$disINTERFACE/WiFi/Mode 2>/dev/null`" = sta ] ; then
     disAP="`iwconfig $disINTERFACE 2>/dev/null | grep 'Access Point:' |sed -r 's/.*Access Point: (..:..:..:..:..:..)/\1/' |egrep '..:..:..:..:..:..' | awk '{print $1}'`"
     if [ -n "$disAP" ] ; then
       echo "Trying to disassociate from the Access Point ($disAP) ..."
       ifconfig $disINTERFACE down
       sleep 2
     fi
  fi
}
function wifi-destroy-interface {
# funzione per la distruzione di una interfaccia
  name="$1"
  if [ -z "$name" ] ; then
    return
  fi
  for p in hostapd wpa_supplicant ; do
    PID=`ps -ef | grep $p |grep $name |awk '{print $2}'`
    while [ -n "$PID" ] ; do
      kill $PID 2>/dev/null
      PID=`ps -ef | grep $p |grep $name |awk '{print $2}'`
    done
  done
  disassociate $name
#  wlanconfig $name destroy 2>/dev/null
  if [ -d /sys/class/net/$name ] ; then
    iw $name del 2>/dev/null 
  fi
}
function wifi-phy-dev {
# funzione per la ricerca del device a cui appartiene $1
 PDEV=$1
 echo "`ls /sys/class/net/$PDEV/phy80211/device/ieee80211`"
}
function wifi-create-localmac {
# funzione per la creazione di un mac address locale 
  pmac=$1
  lname=$2
  tmpmac="0x"$pmac
  lmac=`printf "%X" $(($tmpmac + 1))`
  lenmac=${#lmac}
  ncycle=$((12-$lenmac))
  for (( i=0 ; i<ncycle ; i=$(($i+1)) )); do
    lmac="0"$lmac
  done
  for (( ; ; )) ; do
    if [ `ls $REGISTER/system/net/interfaces/*/MAC/$lmac 2>/dev/null | wc -l ` -gt 0 ] ; then
      tmpmac="0x"$lmac
      lmac=`printf "%X" $(($tmpmac + 1))`
      lenmac=${#lmac}
      ncycle=$((12-$lenmac))
      for (( i=0 ; i<ncycle ; i=$(($i+1)) )); do
        lmac="0"$lmac
      done
    else
      break
    fi
  done
  echo $lmac
}
function wifi-create-interface {
# funzione per la creazione di una interfaccia
  name="$1"
  mode="$2"
  wifialias=$3
  beacon="$4"
  phyname=`wifi-phy-dev $name`
  if [ -z "$name" ] ; then
    return
  fi
  if [ -z "$mode" ] ; then
    mode=sta
    [ -z "$beacon" ] && beacon=nosbeacon
  fi
  if [ -z $wifialias ] ; then
    #Number of wireless interfaces used in the system
    NUM=`ls -d $REGISTER/system/net/interfaces/WLAN* 2>/dev/null | wc -l`
    i=0
    while [ $i -lt `echo $(($NUM + 1))`  ] ; do
      if [ $i -lt 10 ] ; then
        wifialias="WLAN0"$i
      else
        wifialias="WLAN"$i
      fi
      if ! [ -d /sys/class/net/$wifialias ] ; then
        break
      fi
      let i=i+1
    done
    PhyMac=`sed 's/://g' < /sys/class/net/$name/phy80211/addresses 2>/dev/null | awk '{print toupper($0)}'`
    SMAC=`wifi-create-localmac $PhyMac $name`
  else
    SMAC=`(cd $REGISTER/system/net/interfaces/$wifialias/MAC/; ls * 2>/dev/null)`
  fi
  if ! `iw phy $phyname interface add $wifialias type managed ` ; then
    return
  fi
  if ! `ifconfig $wifialias hw ether $SMAC ` ; then
    iw $wifialias del
    return
  fi
  mkdir -p $REGISTER/system/net/interfaces/$wifialias/MAC
  touch $REGISTER/system/net/interfaces/$wifialias/MAC/$SMAC
  if ! [ -f $REGISTER/system/net/interfaces/$wifialias/STATUS  ] ; then
    echo "up" > $REGISTER/system/net/interfaces/$wifialias/STATUS
  fi
  echo $SMAC
}
function showconfig {
# funzione che da la lista dei virtual ap presenti  per ogni device fisico
  wl=`wifi-list`
  if [ -z "$wl" ] ; then
    echo "WARNING: No Wi-Fi compatible chipset has been found."
    sleep 2
    exit
  fi
  echo
  echo "REGULATORY DOMAIN: `$SCRIPTS/wifi-get-regdomain short` (`$SCRIPTS/wifi-get-regdomain long`)"
  [ -z "`cat $REGISTER/system/net/WiFi/REG 2>/dev/null`" ] && echo "WARNING          : Regulatory Domain has not be set by using wifi-manager"
  for wdev in $wl ; do
    echo
    echo -n "[$wdev] "
    $SCRIPTS/getintdesc $wdev | sed -r 's/(.*)Atheros Communications, Inc.(.*)/Chipset\1\2/'
    vaps1=`wifi-listvaps $wdev`
    echo "-- If --- Mode -- SSID -------------------------- Hide -- WDS -- Security -" 
    if [ -n "$vaps1" ] ; then
      for vap1 in $vaps1 ; do
        gmode=`cat $REGISTER/system/net/interfaces/$vap1/WiFi/Mode 2>/dev/null`
        [ "$gmode" == ap ] && gmode=" AP  "
        [ "$gmode" == sta ] && gmode="STA  "
        [ "$gmode" == adhoc ] && gmode="AdHoc"
        gssid="`cat $REGISTER/system/net/interfaces/$vap1/WiFi/SSID 2>/dev/null`                                       "
        gssid=${gssid:0:32} 
        ghide=`cat $REGISTER/system/net/interfaces/$vap1/WiFi/Hide 2>/dev/null`
        gwds=`cat $REGISTER/system/net/interfaces/$vap1/WiFi/WDS 2>/dev/null`
        [ "$gwds" = no -o -z "$gwds" ] && gwds="no "
#        gmac=`( cd $REGISTER/system/net/interfaces/$vap1/MAC ; ls -d * )`
        if [ "$ghide" == 1 ] ; then
           ghide=yes
        else
           ghide="no "
        fi
        if [ "$vap1" == "$SELECTED" ] ; then
          gsel=">>"
        else
          gsel="  "
        fi
        genc=`cat $REGISTER/system/net/interfaces/$vap1/WiFi/Encryption 2>/dev/null`
        [ -z "$genc" ] && genc=plaintext
        [ "$genc" == plaintext ] && genc="Plaintext"
        [ "$genc" == wpa-eap ] && genc="WPA-EAP  "
        [ "$genc" == wpa-psk ] && genc="WPA-PSK  "
        if [ "$genc" == wep ] ; then
          LN=`cat $REGISTER/system/net/interfaces/$vap1/WiFi/WEPKEY 2>/dev/null | wc -c`
          [ $LN -eq 5 -o $LN -eq 10 ] && genc="WEP64"
          [ $LN -eq 13 -o $LN -eq 26 ] && genc="WEP128"
        fi
        echo "$gsel $vap1 $gmode   $gssid  $ghide    $gwds    $genc"
      done
    fi 
  done 
}
function wifi-getdev {
  gdVAP="$1"
  gdDEVID=`cat $REGISTER/system/net/interfaces/$gdVAP/WiFi/DevID 2>/dev/null`
  [ -z "$gdDEVID" ] && return 
  gdDEVS=`wifi-list`
  for gdDEV in $gdDEVS ; do
    if [ "`cat $REGISTER/system/net/WiFi/DEVS/$gdDEV/ID`" == "$gdDEVID" ] ; then
      if [ -d "/sys/class/net/$gdDEV" ] ; then
        echo $gdDEV
      fi
    fi
  done
 
}
function wifi-getname {
#funzione che restituisce il nome del device fisico a partire dal virtual ap
  name="$1"
  if [ -z "$name" ] ; then
    return
  fi
  WL=`wifi-list`
  for i in $WL ; do
    pmac=`cat /sys/class/net/$i/address`
    if [ `(cat $REGISTER/system/net/interfaces/$name/WiFi/DevID 2>/dev/null )` == $pmac ] ; then
      echo $i
    fi
  done
}
function wifi-getaliasname {
  name="$1"
  if [ -z "$name" ] ; then
    return
  fi
  ( cd /sys/class/net/$name/device/net/ ; ls -d WLAN* 2>/dev/null|grep -v mon )
}
function wifi-destroy-all {
#funzione che distrugge tutti i virtual ap
  allwifi="$1"
  [ -z "$allwifi" ] && allwifi=`wifi-list`
  [ -z "$allwifi" ] && return
  for i in $allwifi ; do
    aliasnames=`wifi-getaliasname $i`
    if [ -n "$aliasnames" ] ; then  
      for j in $aliasnames ; do
        wifi-destroy-interface "$j"
      done
    fi
  done
}
function wifi-create-all {
  allwifi=`wifi-list`
  [ -z "$allwifi" ] && return
#  for i in $allwifi ; do
#    wifi-create-interface $i
# done
  eths=`( cd $REGISTER/system/net/interfaces ; ls -d WLAN[0123456789][0123456789] )`
  n=20
  for i in $eths ; do
    if [ -f "$REGISTER/system/net/interfaces/$i/WiFi/MasterVAP" ] ; then
      MasterVAP=`cat $REGISTER/system/net/interfaces/$i/WiFi/MasterVAP`
      wdevice=$MasterDEV
      mode=`cat $REGISTER/system/net/interfaces/$i/WiFi/Mode`
      if  wifi-create-interface $wdevice eth$n $mode bssid ; then
        wmac="`sed 's/://g' < /sys/class/net/eth$n/address 2>/dev/null | awk '{print toupper($0)}'`"
        rm -f $REGISTER/system/net/interfaces/$i/MAC/*
        touch $REGISTER/system/net/interfaces/$i/MAC/$wmac
      fi
      n=$((n+1))   
    fi
  done
}
