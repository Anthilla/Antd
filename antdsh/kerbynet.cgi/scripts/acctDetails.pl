#!/usr/bin/perl
use strict;
use warnings;
exists($ARGV[0]) and my $USER=$ARGV[0] or die 1;
chdir "/var/register/system/acct/entries/$USER/sessions" or die 2;
open FILE,"/var/register/system/acct/Currency";my $CURRENCY=<FILE>; chomp($CURRENCY); close FILE;
open FILE,"/var/register/system/acct/Decimals";my $DECIMALS=<FILE>; chomp($DECIMALS); close FILE;
my $FLOAT="%.${DECIMALS}f";
my $LST="ls -dt *";
my $OUT=`$LST`;
my @SESSIONS=split("\n",$OUT);
my $S;
my $IP;
my $MAC;
my $CLIENT;
my $NAS;
my $START;
my $STOP;
my $LAST;
my $RX;
my $TX;
my $COST;
my $TRAFFIC;
my $TIME;
my $NUMCONN=@SESSIONS;
my $SUMTRAFFIC;
my $SUMTIME;
my $SUMCOST;
open FILE,"../MB" and $SUMTRAFFIC=<FILE> and close FILE or $SUMTRAFFIC=0;
open FILE,"../Time" and $SUMTIME=<FILE> and close FILE or $SUMTIME=0;
open FILE,"../Cost" and $SUMCOST=<FILE> and close FILE or $SUMCOST=0;
foreach $S (@SESSIONS) {
  open FILE,"$S/IP" and $IP=<FILE> and close FILE and chomp($IP)or $IP="";
  open FILE,"$S/MAC" and $MAC=<FILE> and close FILE and chomp($MAC) or $MAC="";
  if ($IP eq "") {
    $CLIENT=$MAC;
  } else {
    $CLIENT=$IP;
    $CLIENT="$IP / $MAC" unless ($MAC eq "");
  }
  open FILE,"$S/NAS" and $NAS=<FILE> and close FILE and chomp($NAS) or $NAS="";
  open FILE,"$S/start" and $START=<FILE> and close FILE and chomp ($START) or $START="";
  $START=~/[0123456789]+/ or $START="";
  unless ($START eq "") {
    my ($sec,$min,$hour,$mday,$mon,$year,$wday,$yday,$isdst)=localtime($START);
    $START=sprintf("%d-%02d-%02d %02d:%02d:%02d",$year-100,$mon+1,$mday,$hour,$min,$sec);
  }
  open FILE,"$S/stop" and $STOP=<FILE> and close FILE and chomp($STOP) or $STOP="";
  $STOP=~/[0123456789]+/ or $STOP="";
  unless ($STOP eq "") {
    my ($sec,$min,$hour,$mday,$mon,$year,$wday,$yday,$isdst)=localtime($STOP);
    $STOP=sprintf("%d-%02d-%02d %02d:%02d:%02d",$year-100,$mon+1,$mday,$hour,$min,$sec);
  }
  open FILE,"$S/Last" and $LAST=<FILE> and close FILE and chomp($LAST) or $LAST="";
  $LAST=~/[0123456789]+/ or $LAST="";
  unless ($LAST eq "") {
    my ($sec,$min,$hour,$mday,$mon,$year,$wday,$yday,$isdst)=localtime($LAST);
    $LAST=sprintf("%d-%02d-%02d %02d:%02d",$year-100,$mon+1,$mday,$hour,$min);
  }
  open FILE,"$S/RX" and $RX=<FILE> and close FILE and chomp($RX) or $RX=0; 
  $RX=sprintf("%.2f",$RX/1048576);
  open FILE,"$S/TX" and $TX=<FILE> and close FILE and chomp($TX) or $TX=0; 
  $TX=sprintf("%.2f",$TX/1048576);
  open FILE,"$S/Traffic" and $TRAFFIC=<FILE> and close FILE and chomp($TRAFFIC) or $TRAFFIC=0; 
  $TRAFFIC=sprintf("%.2f",$TRAFFIC/1048576);
  open FILE,"$S/Time" and $TIME=<FILE> and close FILE and chomp($TIME) or $TIME=0; 
  $TIME=~/[0123456789]+/ or $TIME=0;
  $TIME=sprintf("%2d:%02d:%02d",$TIME/3600,($TIME/60)%60,$TIME%60);
  open FILE,"$S/Cost" and $COST=<FILE> and close FILE and chomp($COST) or $COST=0;
  $COST=sprintf($FLOAT,$COST);

  print "<tr align=center style='color: #404040;'><td><input type=radio name=CLT value='$S'></td><td class=Smaller1>$CLIENT</td><td class=Smaller1>$NAS</td><td class=Smaller1>$START</td><td class=Smaller1>$STOP</td><td class=Smaller1>$RX</td><td class=Smaller1>$TX</td><td class=Smaller1>$TRAFFIC</td><td class=Smaller1>$TIME</td><td class=Smaller1>$COST</td><td class=Smaller1>$LAST</td></tr>\n"
}
print "<script>parent.document.getElementById('nSession').innerHTML='$NUMCONN';</script>\n";

$SUMTRAFFIC=sprintf("%.2f",$SUMTRAFFIC/1048576);
print "<script>parent.document.getElementById('SumTraffic').innerHTML='$SUMTRAFFIC';</script>\n";
$SUMTIME/=60;
$SUMTIME=sprintf("%2d:%02d",$SUMTIME/60,$SUMTIME%60);
print "<script>parent.document.getElementById('SumTime').innerHTML='$SUMTIME';</script>\n";
$SUMCOST=sprintf("$FLOAT",$SUMCOST);
print "<script>parent.document.getElementById('SumCost').innerHTML='$SUMCOST';document.getElementById('CurrencySymbol').innerHTML='$CURRENCY';</script>\n";

