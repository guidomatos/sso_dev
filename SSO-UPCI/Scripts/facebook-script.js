
window.fbAsyncInit = function () {
    // FB JavaScript SDK configuration and setup
    FB.init({
        //INICIO cuenta:"idm.ssoupc@gmail.com" (JCRodriguez 12-10-2020)
        appId: '757153728614172',
          //appId: '2372062989753787', // FB App ID
        //FIN
        cookie: true,   // enable cookies to allow the server to access the session
        xfbml: true,    // parse social plugins on this page
        //INICIO cuenta:"idm.ssoupc@gmail.com" (JCRodriguez 12-10-2020)
          version: 'v14.0'
          //version: 'v7.0' // use graph api version 2.8
        //FIN
    });
};

// Load the JavaScript SDK asynchronously
(function (d, s, id) {
    var js, fjs = d.getElementsByTagName(s)[0];
    if (d.getElementById(id)) return;
    js = d.createElement(s); js.id = id;
    js.src = "//connect.facebook.net/en_US/sdk.js";
    fjs.parentNode.insertBefore(js, fjs);
}(document, 'script', 'facebook-jssdk'));


// Facebook login with JavaScript SDK
function fbLogin(callback, callbackFail) {
    FB.login(function (response) {
        if (response.authResponse) {
            getFbUserData(callback); // Get and display the user profile data
        } else {
            callbackFail();
            return;
        }
    }, { scope: 'email' });
}

// Facebook logout
function fbLogout() {
    FB.getLoginStatus(function(response) {
        if (response && response.status === 'connected') {
            FB.logout(function(response) {
                document.location.reload();
            });
        }
    });
}

// Fetch the user profile data from facebook
function getFbUserData(callback) {
    FB.api('/me', { locale: 'en_US', fields: 'id,first_name,last_name,email,link,gender,locale,picture' }, callback);
}