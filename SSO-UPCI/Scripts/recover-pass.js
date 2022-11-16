$(document).ready(function () {
    const recoverPassUI = (function () {
        let ui = {};

        const init = function () {
            const $form = $("form");
            ui = {
                form: $form,
                inputs: {
                    user: $form.find("#input-username"),
                },
                buttons: {
                    continue: $form.find("#btn-continuar"),
                },
                toast: $form.find('.toast')
            };

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
            positionToast: function () {
                const offset = ui.titleSection.offset();
                const height = ui.titleSection.height();
                ui.toast.css({ top: (Math.round(offset.top) + height + 15) + 'px' });
            },
            setToastMessage: function (objMessage) {
                ui.toast.find('.toast-title').text(objMessage.title);
                ui.toast.find('.toast-message').text(objMessage.message);
            },
            setInputErrors: function (errorObj) {
                if (errorObj.user) {
                    ui.inputs.user.addClass('is-invalid');
                }
            },
            clearInputErrors: function () {
                ui.form.find('input').removeClass('is-invalid');
            },
            validateData: function () {
                let errorObj = {};
                if (ui.inputs.user.val() == '') {
                    errorObj.user = "El campo usuario es requerido";
                }

                return errorObj;
            },
            getFormData: function () {
                return {
                    user: btoa( ui.inputs.user.val() )
                };
            },
            toggleSpinner: function () {
                const $spinner = ui.buttons.continue.find('span');
                uiFlags.spinner = !uiFlags.spinner;
                if (uiFlags.spinner) {
                    $spinner.removeClass('hide');
                } else {
                    $spinner.addClass('hide');
                }
            },
            setLocalInfo: function (userInfo) {
                localStorage.user      = userInfo.usuario_login;
                localStorage.username  = userInfo.usuario_nombre;
                localStorage.email     = userInfo.Usuario_correoPersonal;
                localStorage.cel       = userInfo.usuario_telefono;
                localStorage.apPaterno = userInfo.usuario_apPaterno;
                localStorage.apMaterno = userInfo.usuario_apMaterno;
                console.log(userInfo.usuario_apMaterno);
            }
        };

        const services = {
            validateOnlyUser: function (data, callback) {
                $.post(API_URLS.VALIDATE_USER, data)
                    .done(callback)
                    .fail(function (jqXHR, textStatus) {
                        console.error('Error:' + textStatus);
                    });
            },
        };

        const callbacks = {
            validateOnlyUser: function (response) {
                console.log(response);
                ui.buttons.continue.prop('disabled', false);
                uiActions.toggleSpinner();

                if (response) {
                    if (response.ok) {
                        uiActions.setLocalInfo(response.obj);
                        const redirectUrl = location.protocol + '//' + API_URL_BASE + UI_URLS.RECOVER_METHOD;
                        window.location.href = redirectUrl;
                    } else {
                        console.log('Error:' + response.mensaje);
                        uiActions.setToastMessage({
                            title: 'Usuario incorrecto',
                            message: response.mensaje,
                        });
                        uiActions.showToast();
                    }
                } else {
                    uiActions.setToastMessage({
                        title: 'Error de servidor',
                        message: 'No hay respuesta de servidor',
                    });
                    uiActions.showToast();
                    console.error('Unknow server error: No response')
                }
            },
        };

        const addEventsListeners = function () {
            ui.inputs.user.keydown(function (event) {
                if (event.keyCode == 13) {
                    ui.buttons.continue.trigger('click');
                }

                if (event && event.key && !event.key.match(/[a-zA-Z0-9]/)) {
                    event.preventDefault();
                }
            });

            ui.inputs.user.blur(function (evt) {
                const valor = evt.target.value.replace(/[^\w\s]/gi, '');
                ui.inputs.user.val(valor);
            });
            ui.buttons.continue.click(function (evt) {
                evt.preventDefault();
                uiActions.hideToast();
                uiActions.clearInputErrors();

                const userData = uiActions.getFormData();
                const validationErrors = uiActions.validateData();
                uiActions.setInputErrors(validationErrors);

                if (!Object.keys(validationErrors).length) {
                    ui.buttons.continue.prop('disabled', true);
                    uiActions.toggleSpinner();
                    services.validateOnlyUser(userData, callbacks.validateOnlyUser);
                } else {
                    uiActions.setToastMessage({
                        title: 'Corregir los campos',
                        message: validationErrors.user
                    });
                    uiActions.showToast();
                }
            });
        };

        return {
            init: init,
        };
    })();

    recoverPassUI.init();
});