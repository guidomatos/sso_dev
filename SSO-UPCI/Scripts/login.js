$(document).ready(function () {
    const loguinUI = (function () {
        let ui = {};

        const init = function () {
            const $form = $("form");
            ui = {
                form: $form,
                inputs: {
                    user: $form.find("#input-user"),
                    password: $form.find("#input-password"),
                },
                buttons: {
                    login: $form.find("#btn-login"),
                },
                toast: $form.find('.toast'),
                links: {
                    forgotPass: $('#link-forgot-pass')
                }
            };

            ui.links.forgotPass.prop('href', UI_URLS.RECOVER_PASS);
            addEventsListeners();
        };

        const uiFlags = {
            spinner: false,
            toast: false,
        };

        const uiActions = {
            showToast: function () {
                if (!uiFlags.toast) {
                    uiFlags.toast = true;
                    ui.toast.addClass('visible');
                }
            },
            hideToast: function () {
                if (uiFlags.toast) {
                    uiFlags.toast = false;
                    ui.toast.removeClass('visible');
                }
            },
            setToastMessage: function (toastMessage) {
                ui.toast.find('.toast-title').text(toastMessage.title);
                ui.toast.find('.toast-message').text(toastMessage.message);
            },
            getFormData: function () {
                return {
                    user: btoa(ui.inputs.user.val()),
                    password: btoa( ui.inputs.password.val() )
                };
            },
            validateCredentials: function (credentialsData) {
                let errorObj = {};
                if (ui.inputs.user.val() == '') {
                    errorObj.user = UI_ERRORS.FORMS.USER_REQUIRED;
                }
                if (ui.inputs.password.val() == '') {
                    errorObj.password = UI_ERRORS.FORMS.PASSWORD_REQUIRED;
                }

                return errorObj;
            },
            setInputErrors: function (errorObj) {
                if (errorObj.user && errorObj.password) {
                    uiActions.setToastMessage({
                        title: UI_ERRORS.FORMS.CORRECT_ERRORS,
                        message: UI_ERRORS.FORMS.INPUTS_REQUIRED,
                    });
                    ui.inputs.user.addClass('is-invalid');
                    ui.inputs.password.addClass('is-invalid');
                    uiActions.showToast();
                } else {
                    if (errorObj.user) {
                        ui.inputs.user.addClass('is-invalid');
                        uiActions.setToastMessage({
                            title: UI_ERRORS.FORMS.CORRECT_ERRORS,
                            message: errorObj.user,
                        });
                        uiActions.showToast();
                    }
                    if (errorObj.password) {
                        uiActions.setToastMessage({
                            title: UI_ERRORS.FORMS.CORRECT_ERRORS,
                            message: errorObj.password,
                        });
                        uiActions.showToast();
                        ui.inputs.password.addClass('is-invalid');
                    }
                }
            },
            clearInputErrors: function () {
                ui.form.find('input').removeClass('is-invalid');
            },
            toggleSpinner: function () {
                const $spinner = ui.buttons.login.find('span');
                uiFlags.spinner = !uiFlags.spinner;
                if (uiFlags.spinner) {
                    $spinner.removeClass('hide');
                } else {
                    $spinner.addClass('hide');
                }
            }
        };

        const services = {
            loginWithCredentials: function (credentials, callback) {
                $.post(API_URLS.VALIDATE_CREDENTIALS, credentials)
                    .done(callback)
                    .fail(function (jqXHR, textStatus) {
                        console.error('Error:' + textStatus);
                        uiActions.setToastMessage({
                            title: UI_ERRORS.SERVER_ERROR,
                            message: textStatus,
                        });
                        uiActions.showToast();
                    });
            },
            dontShow: function (callback) {
                const data = { user: localStorage.user };
                $.post(API_URLS.DONT_SHOW_NETPAGE, data).done(callback);
            },
        };

        const callbacks = {
            login: function (response) {
                console.log(response);
                ui.buttons.login.prop('disabled', false);
                uiActions.toggleSpinner();
                if (response) {
                    if (response.ok) {
                        localStorage.user = response.obj.usuario_login;
                        //localStorage.username = response.obj.usuario_nombre;
                        //localStorage.email = response.obj.Usuario_correoPersonal;
                        //localStorage.cel = response.obj.usuario_telefono;
                        localStorage.urlFederada = response.federada;

                        //TODO: enhance this. Temporary assignment
                        UI_URLS.NEXT_PAGE = response.federada;

                        //SI EL FLAG ES POSITIVO VA A LA VISTA RECUPERAR PASSWORD
                        if (response.flagRecPas) {
                            redirectUrl = location.protocol + '//' + API_URL_BASE + UI_URLS.RECOVER_PASS;
                            window.location.href = redirectUrl;
                        }
                        else
                        {
                            services.dontShow(callbacks.dontShow);
                        }

                    } else {
                        console.log('Error:' + response.mensaje);
                        uiActions.setToastMessage({
                            title: UI_ERRORS.LOGIN.USER_PASS_INCORRECT,
                            message: response.mensaje,
                        })
                        uiActions.showToast();
                    }
                } else {
                    uiActions.setToastMessage({
                        title: UI_ERRORS.UNKNOWN_ERROR,
                        message: UI_ERRORS.SERVER_NOT_RESPOND,
                    })
                    uiActions.showToast();
                    console.error(UI_ERRORS.UNKNOWN_ERROR + ' : ' + UI_ERRORS.SERVER_NOT_RESPOND);
                }
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

        const addEventsListeners = function () {

            ui.buttons.login.click(function (evt) {
                evt.preventDefault();
                uiActions.hideToast();
                const credentialsData = uiActions.getFormData();

                uiActions.clearInputErrors();
                const validationErrors = uiActions.validateCredentials(credentialsData);
                uiActions.setInputErrors(validationErrors);

                if (!Object.keys(validationErrors).length) {
                    ui.buttons.login.prop('disabled', true);
                    uiActions.toggleSpinner();
                    services.loginWithCredentials(credentialsData, callbacks.login);
                }
            });
        };

        return {
            init: init,
        };
    })();

    loguinUI.init();
});