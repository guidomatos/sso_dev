var po = document.createElement('script'); po.type = 'text/javascript'; po.async = true;
po.src = 'https://apis.google.com/js/client:platform.js?onload=handleClientLoad';
var s = document.getElementsByTagName('script')[0]; s.parentNode.insertBefore(po, s);

//INICIO cuenta:"idm.ssoupc@gmail.com"
  var clientId = '955171238109-sn4h8vpfvnu7549luu6hi2r2j98voqco.apps.googleusercontent.com';
  var apiKey   = 'AIzaSyBIpeIXoKHiyZVJgTWOnywxrzgaX7fgmc4';
  //var clientId = '266458480876-furp0ebl8d3okhf8l3cmc83v0nl8nuj1.apps.googleusercontent.com';
  //var apiKey   = 'AIzaSyCu8O2XBuCU_aeIz-WXq2hay0gnpBExcgQ';
//FIN


function googleLogin(callback) {
    gapi.client.setApiKey(apiKey);
    gapi.auth2.authorize({
        client_id: clientId,
        scope: 'email profile openid',
        response_type: 'id_token permission'
    }, callback);
}
