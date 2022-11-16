$(document).ready(function () {
    const indexUI = (function () {
        let ui = {};

        const init = function () {
            const $form = $('form');
            ui = {
                buttons: {
                    facebook: $form.find('#btn-facebook'),
                    google: $form.find('#btn-google'),
                },
                links: {
                    loginUPC: $('#link-login')
                },
                toast: $form.find('.toast'),
            };

            ui.links.loginUPC.prop('href', UI_URLS.LOGIN);
            addEventListeners();
        };

        const uiFlags = {
            toast: false,
        };

        const uiActions = {
            showToast: function() {
                if (!uiFlags.toast) {
                    uiFlags.toast = true;
                    ui.toast.addClass('visible');
                }
            },
            hideToast: function() {
                if (uiFlags.toast) {
                    uiFlags.toast = false;
                    ui.toast.removeClass('visible');
                }
            },
            setToastMessage: function(toastMessage) {
                ui.toast.find('.toast-title').text(toastMessage.title);
                ui.toast.find('.toast-message').text(toastMessage.message);
            },
        };

        const callbacks = {
            facebookLogin: function (fbResponse) {
                services.processFacebookLogin(fbResponse, function (response){
                    if (response.ok) {
                        localStorage.user = response.obj.usuario_login;
                        //localStorage.username    = response.obj.usuario_nombre;
                        //localStorage.email       = response.obj.Usuario_correoPersonal;
                        //localStorage.cel         = response.obj.usuario_telefono;
                        localStorage.urlFederada   = response.federada;

                        UI_URLS.NEXT_PAGE = response.federada;

                        services.dontShow(callbacks.dontShow);
                    }
                    else {
                        uiActions.setToastMessage({
                            title: 'Error',
                            message: response.mensaje,
                        });
                        uiActions.showToast();
                        console.error(response.mensaje);
                    };
                });
            },
            googleLogin: function (googleResponse) {
                services.processGoogleLogin(googleResponse, function (response){
                    if (response.ok) {
                        localStorage.user = response.obj.usuario_login;
                        //localStorage.username    = response.obj.usuario_nombre;
                        //localStorage.email       = response.obj.Usuario_correoPersonal;
                        //localStorage.cel         = response.obj.usuario_telefono;
                        localStorage.urlFederada   = response.federada;

                        UI_URLS.NEXT_PAGE = response.federada;

                        services.dontShow(callbacks.dontShow);
                    }
                    else {
                        uiActions.setToastMessage({
                            title: 'Error',
                            message: response.mensaje,
                        });
                        uiActions.showToast();
                        console.error(response.mensaje);
                    };
                });
            },
            dontShow: function (response) {
                if (response.ok) {
                    let redirectUrl;
                    if (!response.noMostrar) {
                        redirectUrl = location.protocol + '//' + API_URL_BASE + UI_URLS.VINCULATE_ACCOUNTS;
                    } else {
                        redirectUrl = UI_URLS.NEXT_PAGE;
                    }

                    window.location.href = redirectUrl;
                }
            },
        };

        const services = {
            processFacebookLogin: function (fbResponse, callback) {
                const data = { redSocial_email: btoa(fbResponse.email) };
                $.post(API_URLS.EVALUATE_FB, data).done(callback);
            },
            processGoogleLogin: function (googleResponse, callback) {
                const data = { redSocial_token: googleResponse.id_token };
                $.post(API_URLS.EVALUATE_JWT, data).done(callback);
            },
            dontShow: function (callback) {
                const data = { user: localStorage.user };
                $.post(API_URLS.DONT_SHOW_NETPAGE, data).done(callback);
            },
        };

        const addEventListeners = function() {
            ui.buttons.facebook.click(function (evt) {
                evt.preventDefault();
                // Call method from facebook-script.js
                fbLogin(callbacks.facebookLogin);
            });

            ui.buttons.google.click(function (evt) {
                evt.preventDefault();
                // Call method from google-script.js
                googleLogin(callbacks.googleLogin);
            });
        }

        return {
            init: init,
        };
    })();

    indexUI.init();
});