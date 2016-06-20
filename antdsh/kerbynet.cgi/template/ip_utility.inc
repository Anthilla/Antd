function ip2num(IP) {
  if (!isValidIP(IP)) return 0;
  filter=/^ *(\d{1,3})\.(\d{1,3})\.(\d{1,3})\.(\d{1,3}) *$/
  comp=filter.exec(IP);
  if (comp[1]>255 || comp[2]>255 || comp[3]>255 || comp[4]>255) {
     return 0;
  }
  return parseInt(comp[4])+parseInt(comp[3])*256+parseInt(comp[2])*256*256+parseInt(comp[1])*256*256*256;
}
function isValidIP (IP) {
  filter=/^ *(\d{1,3})\.(\d{1,3})\.(\d{1,3})\.(\d{1,3}) *$/
  comp=filter.exec(IP);
  if (comp==undefined) {
     return false;
  }
  if (comp.length<4) {
    return false;
  }
  if (comp[1]>255 || comp[2]>255 || comp[3]>255 || comp[4]>255) {
     return false;
  }
  return true;
}
function isValidPort(port) {
  port.value=port.replace(/[ ]+/g,"");
  if (port=="") return false;
  if (port.replace(/[^0123456789:]+/g,"")!=port) {
    port="100000";
  }
  if (port>=0 && port<=65535) {
    NoValid=false;
  } else {
    NoValid=true;
  }
  return !NoValid;
}
function isValidMAC (MAC) {
  MAC=MAC.toUpperCase();
  filter=/^ *([0123456789ABCDEF]{1,2})\:([0123456789ABCDEF]{1,2})\:([0123456789ABCDEF]{1,2})\:([0123456789ABCDEF]{1,2})\:([0123456789ABCDEF]{1,2})\:([0123456789ABCDEF]{1,2}) *$/
  comp=filter.exec(MAC);
  if (comp==undefined) {
     return false;
  }
  if (comp[1]==undefined || comp[2]==undefined || comp[3]==undefined || comp[4]==undefined || comp[5]==undefined || comp[6]==undefined) {
     return false;
  }
  return true;
}
function formatMAC(MAC) {
  if (!isValidMAC(MAC)) {
    return MAC;
  }
  MAC=MAC.toUpperCase();
  filter=/^ *([0123456789ABCDEF]{1,2})\:([0123456789ABCDEF]{1,2})\:([0123456789ABCDEF]{1,2})\:([0123456789ABCDEF]{1,2})\:([0123456789ABCDEF]{1,2})\:([0123456789ABCDEF]{1,2}) *$/
  comp=filter.exec(MAC);
  if (comp[1].length<2) {
    comp[1]="0"+comp[1];  
  }
  if (comp[2].length<2) {
    comp[2]="0"+comp[2];  
  }
  if (comp[3].length<2) {
    comp[3]="0"+comp[3];  
  }
  if (comp[4].length<2) {
    comp[4]="0"+comp[4];  
  }
  if (comp[5].length<2) {
    comp[5]="0"+comp[5];  
  }
  if (comp[6].length<2) {
    comp[6]="0"+comp[6];  
  }
  return (comp[1]+":"+comp[2]+":"+comp[3]+":"+comp[4]+":"+comp[5]+":"+comp[6]);
}
function isValidNetmask (Netmask) {
  return isValidIP(Netmask);
}
function standardNetmask(IP) {
  if (isValidIP(IP)) {
    filter=/^ *(\d{1,3})\.(\d{1,3})\.(\d{1,3})\.(\d{1,3}) *$/
    comp=filter.exec(IP);
    if (comp[1]==0 && comp[2]==0 && comp[3]==0 && comp[4]==0) {
      return "0.0.0.0";
    }
    if (comp[1]<128) {
       return "255.0.0.0";
    } else {
       if (comp[1]<192) {
          return "255.255.0.0";
       } else {
          return "255.255.255.0";
       }
    }
  } else {
     return "";
  }
}
function NetmaskFromNetwork(Network) {
  Netmask=standardNetmask(Network);
  if (isValidNetwork(Network,Netmask)) {
     return Netmask;
  }
  filter=/^ *(\d{1,3})\.(\d{1,3})\.(\d{1,3})\.(\d{1,3}) *$/
  comp=filter.exec(Network);
  if (comp==undefined) {
     return "";
  }
  if (comp.length<4) {
    return "";
  }
  Netmask="255.0.0.0";
  if (comp[2]!=0) Netmask="255.255.0.0";
  if (comp[3]!=0) Netmask="255.255.255.0";
  if (isValidNetwork(Network,Netmask)) {
     return Netmask;
  }
  return "";
}
function isValidNetwork (Network,Netmask) {
  filter=/^ *(\d{1,3})\.(\d{1,3})\.(\d{1,3})\.(\d{1,3}) *$/
  comp=filter.exec(Network);
  if (comp==undefined) {
     return false;
  }
  if (comp.length<4) {
    return false;
  }
  if (comp[1]>255 || comp[2]>255 || comp[3]>255 || comp[4]>255) {
     return false;
  }
  if (Netmask!="") {
    NetmaskComp=filter.exec(Netmask);
    if (NetmaskComp==undefined) {
     return false;
    }
    if (NetmaskComp.length<4) {
      return false;
    }
    if (((comp[1] & NetmaskComp[1])!=comp[1]) || ((comp[2] & NetmaskComp[2])!=comp[2]) || ((comp[3] & NetmaskComp[3])!=comp[3]) || ((comp[4] & NetmaskComp[4])!=comp[4])) {
       return false;
    }
  }
  return true;
}
function isValidNetworkCIDR (Network,Netmask) {
  if (isValidNetwork(Network,Netmask)) return true;
  if (Netmask.indexOf('.')==-1 && /[0123456789]{1,2}/.test(Netmask) && isValidIP(Network) && Netmask>=0 && Netmask<=32) return true;
  return false;
}
function ip_belongs_network (IP,Network,Netmask) {
  if (!isValidIP(IP)) return false;
  if (!isValidNetwork(Network,Netmask)) return false;   
  filter=/^ *(\d{1,3})\.(\d{1,3})\.(\d{1,3})\.(\d{1,3}) *$/
  NetworkComp=filter.exec(Network);
  NetmaskComp=filter.exec(Netmask);
  IPComp=filter.exec(IP);
  if (comp[1]>255 || comp[2]>255 || comp[3]>255 || comp[4]>255) {
     return false;
  }
  if ((IPComp[1] & NetmaskComp[1])==(NetworkComp[1] & NetmaskComp[1]) && (IPComp[2] & NetmaskComp[2])==(NetworkComp[2] & NetmaskComp[2]) && (IPComp[3] & NetmaskComp[3])==(NetworkComp[3] & NetmaskComp[3]) && (IPComp[4] & NetmaskComp[4])==(NetworkComp[4] & NetmaskComp[4])) {
    return true;
  } else {
    return false;
  }
}
function NetworkFromIP(IP,Netmask) {
  filter=/^ *(\d{1,3})\.(\d{1,3})\.(\d{1,3})\.(\d{1,3}) *$/
  IPComp=filter.exec(IP);
  NetmaskComp=filter.exec(Netmask);
  return (IPComp[1] & NetmaskComp[1])+"."+(IPComp[2] & NetmaskComp[2])+"."+(IPComp[3] & NetmaskComp[3])+"."+(IPComp[4] & NetmaskComp[4]);
  


}
