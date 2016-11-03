#!/usr/bin/perl
use strict;
use warnings;
my $FILTER="";
exists($ARGV[0]) and $FILTER=$ARGV[0];
my $CONFIG='/var/register/system/acct/entries';
chdir $CONFIG;
open FILE,"/var/register/system/acct/Decimals"; ;my $DECIMALS=<FILE>; chomp($DECIMALS); close FILE;
my $FLOAT="%.${DECIMALS}f";
my $LST="ls -d *$FILTER*";
my $OUT=`$LST`;
my @ENTRIES=split("\n",$OUT);
my $E;
my $TIME;
my $MB;
my $COST;
my $CREDIT;
my $LAST;
foreach $E (@ENTRIES) {
  open FILE,"$E/Time" and $TIME=<FILE> and close FILE or $TIME=0;
  $TIME=~/[0123456789]+/ or $TIME=0;
  $TIME/=60; 
  $TIME=sprintf("%2d:%02d",$TIME/60,$TIME%60);
  open FILE,"$E/MB" and $MB=<FILE> and close FILE or $MB=0;
  $MB=~/[0123456789]+/ or $MB=0;
  $MB=sprintf("%.2f",$MB/1048576);
  open FILE,"$E/Cost" and $COST=<FILE> and close FILE or $COST=0;
  $COST=sprintf($FLOAT,$COST);
  open FILE,"/var/register/system/acct/credits/$E/Credit" and $CREDIT=<FILE> and close FILE or $CREDIT=0;
  $CREDIT=sprintf($FLOAT,$CREDIT);
  open FILE,"$E/Last" and $LAST=<FILE> and close FILE or $LAST=0;
  my ($sec,$min,$hour,$mday,$mon,$year,$wday,$yday,$isdst)=localtime($LAST);
  $LAST=sprintf("%d-%02d-%02d&nbsp;&nbsp;%02d:%02d",$year+1900,$mon+1,$mday,$hour,$min);
  print "<tr align=center style='color: #404040;'><td><input type=radio name=CLT value='$E'></td><td class=Smaller1 nowrap><a href='#' onclick='OpenDetails(\"$E\")'>$E</a></td><td class=Smaller1 nowrap>$MB</td><td class=Smaller1 nowrap>$TIME</td><td class=Smaller1 nowrap>$COST</td><td class=Smaller1 nowrap>$CREDIT</td><td class=Smaller1 nowrap>&nbsp;$LAST&nbsp;</td></tr>\n"
}
my $NUMCONN=@ENTRIES;
print "<script>parent.document.getElementById('NumConn').innerHTML=$NUMCONN;</script>\n";

