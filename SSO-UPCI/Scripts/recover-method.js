$(document).ready(function () {
    //Logica nueva (22/06/2022)
    //define variables para textbox SMS y MAIL
    const smstext = document.getElementById('smstext');
    const mailtext = document.getElementById('mailtext');
    //Oculta textbox SMS y MAIL
    smstext.style.display = "none";
    mailtext.style.display = "none";
    //FIN

    const recoverMethodUI = (function () {
        let ui = {};

        const init = function () {
            const $form = $("form");
            ui = {
                form: $form,
                radios: $form.find("input:radio"),
                buttons: {
                    send: $('#btn-enviar'),
                },
                toast: $form.find('.toast'),
                spans: {
                    sms: $form.find('#span-sms').length ? $form.find('#span-sms') : null,
                    email: $form.find('#span-email').length ? $form.find('#span-email') : null,
                },
                inputs: {
                    sms: $('#input-confirm-sms').length ? $('#input-confirm-sms') : null,
                    email: $('#input-confirm-email').length ? $('#input-confirm-email') : null,
                }
            };
            ////chequea radio buton SMS por default
            //ui.radios.get(0).checked = true;                                  
            ////FIN
            ui.buttons.send.prop('disabled', true);

            addEventsListeners();
        };

        const uiFlags = {
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

            //Logica nueva (22/06/2022)
            hideErrors: function () {
                ui.toast.removeClass('visible');
            },
            clearInputErrors: function () {
                ui.form.find('input').removeClass('is-invalid');
            },
            validateInputs: function () {
                let errorObj = {};
                const checkedOption = getCheckedOption();
                if (checkedOption === OPTION_SMS) {
                    if (ui.inputs.sms.val() == '') {
                        errorObj.sms = "El campo teléfono es requerido";
                    } else if (ui.inputs.sms.val() != atob(localStorage.cel)) {
                        errorObj.sms = "El número ingresado no coincide";
                    }
                }
                if (checkedOption === OPTION_EMAIL) {
                    if (ui.inputs.email.val() == '') {
                        errorObj.email = "El campo correo electrónico es requerido";
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
                if (document.getElementById("input-sms").checked) {
                    return {
                        user: localStorage.user,
                        usuario_nombre: localStorage.username,
                        usuario_telefono: localStorage.cel,
                    };
                }
                if (document.getElementById("input-email").checked) {
                    return {
                        user: localStorage.user,
                        usuario_nombre: localStorage.username,
                        Usuario_correoPersonal: localStorage.email
                    };
                }
            }
            //FIN
        };

        //Logica nueva (22/06/2022)
        const services = {
            sendCode: function (callback) {
                const data = uiActions.getLocalData();
                const radio_sms = document.getElementById("input-sms");
                const radio_email = document.getElementById("input-email");
                let serviceUrl;
                if (radio_sms.checked)   serviceUrl = API_URLS.PROCESS_CODE_SMS;
                if (radio_email.checked) serviceUrl = API_URLS.PROCESS_CODE_EMAIL;
                //const serviceUrl = ui.inputs.sms ? API_URLS.PROCESS_CODE_SMS : API_URLS.PROCESS_CODE_EMAIL;
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
        //FIN

        const getCheckedOption = function () {
            return ui.form.find('[name=recover-method]:checked').val();
        }

        const validateData = function (credentialsData) {
            console.info(credentialsData);
            let errorObj = {};
            if (ui.inputs.user.val() == '') {
                errorObj.user = "El campo usuario es requerido";
            }

            return errorObj;
        };

        const getFormData = function () {
            return {
                user: ui.inputs.user.val(),
            };
        };

        const addEventsListeners = function () {
            //Logica nueva (22/06/2022)
            const numericCodes = [48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 96, 97, 98, 99, 100, 101, 102, 103, 104, 105];
            ui.inputs.sms && ui.inputs.sms.keydown(function (event) {
                if (event.keyCode != 8) {
                    if (!numericCodes.includes(event.keyCode)) {
                        event.preventDefault();
                    }
                }
            });
            //FIN

            ui.radios.click(function (evt) {

                ui.buttons.send.prop('disabled', false);
                const checkedOption = getCheckedOption();
                if (checkedOption === OPTION_SMS) {
                    if (localStorage.cel) {
                        //Logica nueva (15.06.2022)
                        smstext.style.display = "block";
                        mailtext.style.display = "none";

                        const phone = atob(localStorage.cel);
                        const phoneTrail = phone.replace(phone.substring(0, 6), '******');
                        ui.spans.sms.text(phoneTrail);
                        //FIN
                    }
                }
                if (checkedOption === OPTION_EMAIL) {
                    if (localStorage.email) {
                        //Logica nueva (15.06.2022)
                        smstext.style.display = "none";
                        mailtext.style.display = "block";

                        const email = atob(localStorage.email);
                        const name = email.substring(0, email.lastIndexOf('@'));
                        const domain = email.substring(email.lastIndexOf('@') + 1);
                        const nameTrail = name.replace(name.substring(0, name.length - 4), '******');
                        const emailTrail = (nameTrail + '@' + domain).toLowerCase();
                        ui.spans.email.text(emailTrail);
                        //FIN
                    }
                }
            });

            ui.buttons.send.click(function (evt) {

                evt.preventDefault();
                const checkedOption = getCheckedOption();
                if (checkedOption === OPTION_SMS) {
                    const phoneNumber = localStorage.cel ? (localStorage.cel == 'null' ? null : localStorage.cel ): null;
                    if (!phoneNumber) {
                        uiActions.setToastMessage({
                            title: UI_ERRORS.ERROR,
                            message: UI_ERRORS.RECOVER.NO_PHONE_REGISTERED,
                        });
                        uiActions.showToast();
                        evt.preventDefault();   
                        evt.stopPropagation();
                        return;
                    }
                }
                if (checkedOption === OPTION_EMAIL) {
                    const email = localStorage.email ? (localStorage.email == 'null' ? null : localStorage.email ) : null;
                    if (!email) {
                        uiActions.setToastMessage({
                            title: UI_ERRORS.ERROR,
                            message: UI_ERRORS.RECOVER.NO_EMAIL_REGISTERED,
                        });
                        uiActions.showToast();
                        evt.preventDefault();
                        evt.stopPropagation();
                        return;
                    }                    
                }

                //Logica nueva (22/06/2022)
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
                //FIN

            });
        };

        return {
            init: init,
        };
    })();

    recoverMethodUI.init();
});