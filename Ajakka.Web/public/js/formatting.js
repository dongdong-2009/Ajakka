
function formatMac(mac){
    if(mac.length != 12)
        return mac;
    return mac.substring(0,2) +':'+mac.substring(2,4)+':'+mac.substring(4,6)+':'+mac.substring(6,8)+':'+mac.substring(8,10)+':'+mac.substring(10)
    
}