#!/bin/bash
OPTIONS="$1"
TMP=/tmp/cntop.$$

X=''
while [ "$X" != Q -a "$X" != q  ] ; do 
  echo > $TMP
  echo -e "Load Avg:`uptime |awk -F'average:' '{print $2}'`  Tot.Connections: `cat /proc/sys/net/netfilter/nf_conntrack_count`" >> $TMP
  echo "----------------------------------------------------" >> $TMP
  echo -e "IP ADDRESS\t\t\t         CONNECTIONS" >> $TMP
  echo "----------------------------------------------------" >> $TMP
  conntrack -L 2>/dev/null |awk -F"src=" '{print $2}' | \
    awk '{print $1}' |sort | \
      awk '{if (IP==$0) {N=N+1} else {if (IP!="") printf ("%s\t\t\t\t %11d\n",IP,N);N=1;IP=$0;} }' | sort -n -k 2 | tac | head -15 >> $TMP
  echo "----------------------------------------------------" >> $TMP
  if [ "$OPTIONS" != web ] ; then
     echo "Press Q to exit" >> $TMP
     clear
  fi
  cat $TMP
  rm "$TMP"
  if [ "$OPTIONS" != web ] ; then
    read -n1 -t5 X
  else
    X=Q
  fi
done
