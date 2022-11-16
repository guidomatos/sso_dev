$(document).ready(function () {
    const recoverConfirmUI = (function () {
        let ui = {};

        const init = function () {
            const $form = $("form");
            ui = {
                form: $form,
                spans: {
                    sms: $form.find('#span-sms').length ? $form.find('#span-sms') : null,
                    email: $form.find('#span-email').length ? $form.find('#span-email') : null,
                },
                inputs: {
                    sms: $('#input-confirm-sms').length ? $('#input-confirm-sms') : null,
                    email: $('#input-confirm-email').length ? $('#input-confirm-email') : null,
                },
                buttons: {
                    send: $('#btn-enviar'),
                },
                links: {
                    changeMethod: $('#link-recover-method'),
                },
                toast: $('.toast')
            };

            ui.links.changeMethod.prop('href', UI_URLS.RECOVER_METHOD);
            uiActions.fillEmailOrSms();
            addEventListeners();
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
            fillEmailOrSms: function () {
                if (ui.spans.sms && ui.inputs.sms) {
                    const phone = atob(localStorage.cel);
                    const phoneTrail = phone.replace(phone.substring(0, 6), '******');
                    ui.spans.sms.text(phoneTrail);
                }
                if (ui.spans.email && ui.inputs.email) {
                    
                    const email = atob(localStorage.email);
                    const name = email.substring(0, email.lastIndexOf('@'));
                    const domain = email.substring(email.lastIndexOf('@') + 1);
                    const nameTrail = name.replace(name.substring(0, name.length - 4), '******');
                    const emailTrail = (nameTrail + '@' + domain).toLowerCase();
                    ui.spans.email.text(emailTrail);
                
                }
            },
            getFormData: function () {
                if (ui.inputs.sms) {
                    return {
                        sms: ui.inputs.sms.val()
                    };
                }
                if (ui.inputs.email) {
                    return {
                        email: ui.inputs.email.val()
                    };
                }
            },
            validateInputs: function () {
                let errorObj = {};
                if (ui.inputs.sms) {
                    if (ui.inputs.sms.val() == '') {
                        errorObj.sms = "El campo es requerido";
                    } else if (ui.inputs.sms.val() != atob(localStorage.cel)) {
                        errorObj.sms = "El número ingresado no coincide";
                    }
                }
                if (ui.inputs.email) {
                    if (ui.inputs.email.val() == '') {
                        errorObj.email = "El campo es requerido";
                    }
                    else if (ui.inputs.email.val() != atob(localStorage.email)) {
                        errorObj.email = "El correo ingresado no coincide.";
                    } else {
                        const emailRegExp = new RegExp(/^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$/);
                        if (!emailRegExp.test(ui.inputs.email.val())) {
                            errorObj.email = "El correo ingresado es incorrecto.";
                        }
                    }
                }
                return errorObj;
            },
            setInputErrors: function (errorObj) {
                if (errorObj.sms) {
                    ui.inputs.sms.addClass('is-invalid');
                }
                if (errorObj.email) {
                    ui.inputs.email.addClass('is-invalid');
                }
            },
            clearInputErrors: function () {
                ui.form.find('input').removeClass('is-invalid');
            },
            showErrors: function () {
                ui.toast.addClass('visible');
            },
            hideErrors: function () {
                ui.toast.removeClass('visible');
            },
            toggleSpinner: function () {
                const $spinner = ui.buttons.send.find('span.glyphicon');
                const $buttonLabel = ui.buttons.send.find('.button-label');
                uiFlags.spinner = !uiFlags.spinner;
                if (uiFlags.spinner) {
                    $buttonLabel.text('Enviando')
                    $spinner.removeClass('hide');
                } else {
                    $buttonLabel.text('Enviar')
                    $spinner.addClass('hide');
                }
            },
            getLocalData: function () {
                if (ui.inputs.sms) {
                    return {
                        user: localStorage.user,
                        usuario_nombre: localStorage.username,
                        usuario_telefono: localStorage.cel,
                    };
                } else if (ui.inputs.email) {
                    return {
                        user: localStorage.user,
                        usuario_nombre: localStorage.username,
                        Usuario_correoPersonal: localStorage.email
                    };
                }
            }
        };

        const services = {
            sendCode: function (callback) {
                const data = uiActions.getLocalData();
                const serviceUrl = ui.inputs.sms ? API_URLS.PROCESS_CODE_SMS : API_URLS.PROCESS_CODE_EMAIL;
                $.post(serviceUrl, data).done(callback)
                    .fail(function (jqXHR, textStatus) {
                        console.error('Error:' + textStatus);
                    });
            },
        };

        const callbacks = {
            sendCode: function (response) {
                ui.buttons.send.prop('disabled', false);
                uiActions.toggleSpinner();

                if (response.ok) {
                    localStorage.enviadoPor = ui.inputs.sms ? "SMS" : "CORREO";
                    const redirectUrl = location.protocol + '//' + API_URL_BASE + UI_URLS.RECOVER_VERIFICATION;
                    window.location.href = redirectUrl;
                } else {
                    uiActions.setToastMessage({
                        title: 'Ocurrió un error',
                        message: 'Error. El servidor no pudo enviar el código',
                    });
                    uiActions.showToast();
                    console.error('Error sending code: ' + response.mensaje);
                }
            }
        };

        const addEventListeners = function () {
            const numericCodes = [48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 96, 97, 98, 99, 100, 101, 102, 103, 104, 105];
            ui.inputs.sms && ui.inputs.sms.keydown(function (event) {
                if (event.keyCode != 8) {
                    if (!numericCodes.includes(event.keyCode)) {
                        event.preventDefault();
                    }
                }
            });
            ui.buttons.send.click(function (evt) {
                evt.preventDefault();
                uiActions.hideErrors();

                uiActions.clearInputErrors();
                const validationErrors = uiActions.validateInputs();
                uiActions.setInputErrors(validationErrors);

                if (!Object.keys(validationErrors).length) {
                    ui.buttons.send.prop('disabled', true);
                    uiActions.toggleSpinner();
                    services.sendCode(callbacks.sendCode);
                } else {
                    uiActions.setToastMessage({
                        title: 'Corregir los campos',
                        message: validationErrors.sms || validationErrors.email
                    });
                    uiFlags.toast = false;
                    uiActions.showToast();
                }
            });
        };

        return {
            init: init,
        };
    })();

    recoverConfirmUI.init();
});