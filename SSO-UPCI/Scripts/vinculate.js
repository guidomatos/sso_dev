$(document).ready(function () {
    const vinculateAccountsUI = (function () {
        let ui = {};

        const init = function () {
            const $btnFacebook = $('#btn-facebook');
            const $btnGoogle = $('#btn-google');

            ui = {
                buttons: {
                    facebook: $btnFacebook,
                    google: $btnGoogle,
                    later: $('#btn-vincular-despues'),
                    continue: $('#btn-vincular-continuar'),
                },
                toggles: {
                    facebook: $('#switch-facebook'),
                    google: $('#switch-google'),
                },
                checks: {
                    dontShow: $('#chk-dont-show-again')
                },
                usersDivs: {
                    facebook: $btnFacebook.next(),
                    google: $btnGoogle.next(),
                }
            };

            ui.buttons.continue.hide();
            ui.buttons.later.hide();

            services.facebook(callbacks.facebook);
            services.google(callbacks.google);
            services.dontShow(callbacks.dontShow);

            ui.buttons.continue.prop('href', localStorage.urlFederada);
            ui.buttons.later.prop('href', localStorage.urlFederada);

            addEventListeners();
        };

        const uiFlags = {
            toggleFacebook: false,
            toggleGoogle: false,
            toogleDontShow: false,
            continue: false,
            //JuancarlosRodriguez 09-09-2020 ini
            continueFacebook: false,
            continueGoogle: false,
            //JuancarlosRodriguez 09-09-2020 fin
        };

        const uiActions = {
            toggleFacebook: function () {
                uiFlags.toggleFacebook = !uiFlags.toggleFacebook;
                ui.toggles.facebook.prop('checked', uiFlags.toggleFacebook);
                const $vinculateText = ui.toggles.facebook.closest('.control').find('strong');
                if (uiFlags.toggleFacebook) {
                    $vinculateText.addClass('hide');
                } else {
                    $vinculateText.removeClass('hide');
                }
            },
            toggleGoogle: function () {
                uiFlags.toggleGoogle = !uiFlags.toggleGoogle;
                ui.toggles.google.prop('checked', uiFlags.toggleGoogle);
                const $vinculateText = ui.toggles.google.closest('.control').find('strong');
                if (uiFlags.toggleGoogle) {
                    $vinculateText.addClass('hide');
                } else {
                    $vinculateText.removeClass('hide');
                }
            },
            setCheckDontShow: function (checked) {
                uiFlags.toogleDontShow = checked;
                ui.checks.dontShow.prop('checked', uiFlags.toogleDontShow);
            },
            setFacebookProfile: function (userData) {
                uiFlags.toggleFacebook = true;
                ui.toggles.facebook.prop('checked', uiFlags.toggleFacebook);
                ui.toggles.facebook.closest('.control').find('strong').hide();
                ui.usersDivs.facebook.find('span').html( atob(userData.name) );
                ui.usersDivs.facebook.find('img').prop('src', atob(userData.picture) );
            },
            setGoogleProfile: function (userData) {
                uiFlags.toggleGoogle = true;
                ui.toggles.google.prop('checked', uiFlags.toggleGoogle);
                ui.toggles.google.closest('.control').find('strong').hide();
                ui.usersDivs.google.find('span').html( atob(userData.name) );
                ui.usersDivs.google.find('img').prop('src', atob(userData.picture) );
            },
            unsetFacebookProfile: function () {
                uiFlags.toggleFacebook = false;
                ui.toggles.facebook.closest('.control').find('strong').show();
                ui.usersDivs.facebook.addClass('hide');
                ui.usersDivs.facebook.find('span').html('');
                ui.usersDivs.facebook.find('img').prop('src', '');
            },
            unsetGoogleProfile: function () {
                uiFlags.toggleGoogle = false;
                ui.toggles.google.closest('.control').find('strong').show();
                ui.usersDivs.google.addClass('hide');
                ui.usersDivs.google.find('span').html('');
                ui.usersDivs.google.find('img').prop('src', '');
            },
            getLocalInfoFacebook: function (fbResponse) {
                return {
                    redSocial_id: 1,
                    user: localStorage.user,
                    redSocial_name: btoa(fbResponse.first_name),
                    redSocial_email: btoa(fbResponse.email),
                    redSocial_picture: btoa(fbResponse.picture.data.url),
                };
            },
            getLocalInfoGoogle: function (googleResponse) {
                return {
                    redSocial_id: 2,
                    user: localStorage.user,
                    redSocial_token: googleResponse.id_token
                };
            },
            setButton: function () {
                if (uiFlags.continue) {
                    ui.buttons.continue.show();
                    ui.buttons.later.hide();
                } else {
                    ui.buttons.continue.hide();
                    ui.buttons.later.show();
                }
            }
        };

        const callbacks = {
            facebook: function (response) {
                if (response.ok) {
                    uiActions.toggleFacebook();
                    ui.usersDivs.facebook.removeClass('hide');
                    uiActions.setFacebookProfile(response.objJwtClaims);
                    uiFlags.continue = true;
                }
                uiActions.setButton();
            },
            google: function (response) {
                if (response.ok) {
                    uiActions.toggleGoogle();
                    ui.usersDivs.google.removeClass('hide');
                    uiActions.setGoogleProfile(response.objJwtClaims);
                    uiFlags.continue = true;
                }
                uiActions.setButton();
            },
            dontShow: function (response) {
                if (response.ok) {
                    uiActions.setCheckDontShow(response.noMostrar);
                }
            },
            updateDontShow: function (response) {
                // TODO: set some ui action
                if (response.ok) {
                    console.info('Update dont show value successfully');
                } else {
                    console.error('Server error: ' + response.mensaje);
                }
            },
            facebookLogin: function (fbResponse) {

                ui.usersDivs.facebook.removeClass('hide');
                // TODO: remove redundant code....
                //uiActions.setFacebookProfile({ name: fbResponse.first_name, picture: fbResponse.picture.data.url }); jc (21-04-2022)

                services.processFacebookLogin(fbResponse, function (response) {
                    if (response.ok) {
                        uiActions.toggleFacebook();
                        uiActions.setFacebookProfile(response.objJwtClaims);
                    }
                });
            },
            facebookLoginFail: function (fbResponse) {
                uiFlags.toggleFacebook = true;
                uiActions.toggleFacebook();
                uiActions.unsetFacebookProfile();
            },
            googleLogin: function (googleResponse) {
                if (googleResponse.error) {
                    uiFlags.toggleGoogle = true;
                    uiActions.toggleGoogle();
                    uiActions.unsetGoogleProfile();
                    return;
                }

                services.processGoogleLogin(googleResponse, function (response) {
                    if (response.ok) {
                        ui.usersDivs.google.removeClass('hide');
                        uiActions.setGoogleProfile(response.objJwtClaims);
                    }
                    else {
                        alert('Error procesando informacion de google en nuestro server. TODO: Definir que mensaje de error debe ir aquí!');
                    }
                });
            },
            unlinkFacebookAccount: function (response) {
                if (response.ok) {
                    uiActions.unsetFacebookProfile();
                }
            },
            unlinkGoogleAccount: function (response) {
                if (response.ok) {
                    uiActions.unsetGoogleProfile();
                    uiFlags.toggleGoogle = true;
                    uiActions.toggleGoogle();
                }
            },
        };

        const services = {
            facebook: function (callback) {
                const data = { user: localStorage.user, redSocial_id: 1 };
                $.post(API_URLS.NET_USERS, data).done(callback);
            },
            google: function (callback) {
                const data = { user: localStorage.user, redSocial_id: 2 };
                $.post(API_URLS.NET_USERS, data).done(callback);
            },
            dontShow: function (callback) {
                const data = { user: localStorage.user };
                $.post(API_URLS.DONT_SHOW_NETPAGE, data).done(callback);
            },
            updateDontShow: function (noMostrar, callback) {
                const data = { user: localStorage.user, noMostrar: noMostrar };
                $.post(API_URLS.UPDATE_SHOW_NETPAGE, data).done(callback);
            },
            processFacebookLogin: function (fbResponse, callback) {
                const data = uiActions.getLocalInfoFacebook(fbResponse);
                $.post(API_URLS.PROCESS_FB, data).done(callback);
            },
            processGoogleLogin: function (googleResponse, callback) {
                const data = uiActions.getLocalInfoGoogle(googleResponse);
                $.post(API_URLS.PROCESS_JWT, data).done(callback);
            },
            unlinkFacebookAccount: function (callback) {
                const data = { user: localStorage.user, redSocial_id: 1 };
                $.post(API_URLS.UNLINK_SOCIAL_ACCOUNT, data).done(callback);
            },
            unlinkGoogleAccount: function (callback) {
                const data = { user: localStorage.user, redSocial_id: 2 };
                $.post(API_URLS.UNLINK_SOCIAL_ACCOUNT, data).done(callback);
            }
        };

        const addEventListeners = function () {
            ui.toggles.facebook.click(function (evt) {
                console.log(evt.target.checked);
                if (evt.target.checked) {
                    fbLogin(callbacks.facebookLogin, callbacks.facebookLoginFail);
                } else {
                    services.unlinkFacebookAccount(callbacks.unlinkFacebookAccount);
                    fbLogout();
                }
                //JuancarlosRodriguez 09-09-2020 ini
                uiFlags.continueFacebook = evt.target.checked;
                if (uiFlags.continueFacebook) {
                    ui.buttons.continue.show();
                    ui.buttons.later.hide();
                } else {
                    if (uiFlags.continueGoogle == false) {
                        ui.buttons.continue.hide();
                        ui.buttons.later.show();
                    }
                }
                //JuancarlosRodriguez 09-09-2020 fin
            });
            ui.toggles.google.click(function (evt) {
                console.log(evt.target.checked);
                if (evt.target.checked) {
                    googleLogin(callbacks.googleLogin);
                } else {
                    services.unlinkGoogleAccount(callbacks.unlinkGoogleAccount);
                }
                //JuancarlosRodriguez 09-09-2020 ini
                uiFlags.continueGoogle = evt.target.checked;
                if (uiFlags.continueGoogle) {
                    ui.buttons.continue.show();
                    ui.buttons.later.hide();
                } else {
                    if (uiFlags.continueFacebook == false) {
                        ui.buttons.continue.hide();
                        ui.buttons.later.show();
                    }
                }
                //JuancarlosRodriguez 09-09-2020 fin
            });
            ui.checks.dontShow.click(function (evt) {
                const dontShow = ui.checks.dontShow.prop('checked');
                console.log(dontShow);
                services.updateDontShow(dontShow, callbacks.updateDontShow);
            });
        };

        return {
            init: init,
        };
    })();

    vinculateAccountsUI.init();
});