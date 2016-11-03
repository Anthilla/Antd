function KADMIN {
   QUERY="$1"
   kadmin.local -q "$QUERY" |grep -v "Authenticating as principal" 


}
