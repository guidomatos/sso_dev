$(document).ready(function () {
    const createPassUI = (function () {
        let ui = {};

        const init = function () {
        	const $form = $('form');
            const $list = $form.find('ul.list-validation');
        	ui = {
        		form: $form,
        		spans: {
        			user: $('.span-user'),
        			email: $('.span-email'),
        		},
        		inputs: {
        			password: $form.find('#input-password'),
        			passwordConfirm: $form.find('#input-password-confirm'),
        		},
        		buttons: {
        			continue: $form.find('#btn-continuar'),
        		},
        		toast: $form.find('.toast'),
                list: $list,
                listElements: $list.find('li'),
        	};

            //uiActions.hideListValidation();
        	uiActions.setLocalData();
        	addEventListeners();
        }

        const uiFlags = {
            spinner: false,
            toast: false,
            list: false,
            fortaleza: false,
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
        	positionToast: function() {
        		const offset = ui.titleSection.offset();
        		const height = ui.titleSection.height();
        		ui.toast.css({ top: (Math.round(offset.top) + height + 15) + 'px' });
        	},
        	setToastMessage: function(objMessage) {
        		ui.toast.find('.toast-title').text(objMessage.title);
                ui.toast.find('.toast-message').text(objMessage.message);
        	},
        	setLocalData: function () {
        		ui.spans.user.text(atob(localStorage.user));
                ui.spans.email.text(atob(localStorage.email));
        	},
        	setInputErrors: function (errorObj) {
                if (errorObj.password) {
                    ui.inputs.password.addClass('is-invalid');
                }
                if (errorObj.passwordConfirm) {
                    ui.inputs.passwordConfirm.addClass('is-invalid');
                }
            },
            hideListValidation: function() {
                ui.list.hide();
            },
            toggleListValidation: function() {
                if (ui.inputs.password.is(':focus') || ui.inputs.passwordConfirm.is(':focus')) {
                    if (!uiFlags.list) {
                        ui.list.show();
                        uiFlags.list = true;
                    }

                } else {
                    if (uiFlags.list) {
                        ui.list.hide();
                        uiFlags.list = false;
                    }
                }
            },
        	validatePassword: function () {
        		let errorObj = {};
        		if (ui.inputs.password.val() == '') {
        			errorObj.password = "El campo contraseña es requerido";
        		} else {
                    const strongPassRegexp = new RegExp(/^(?=.{8,}$)(?=.*?[a-z])(?=.*?[A-Z])(?=.*?[0-9])(?=.*?\W).*$/);
                    const password = ui.inputs.password.val();
                    if (!strongPassRegexp.test(password)) {
                        errorObj.password = "Verifica la fortaleza de la contraseña";
                    }
                    if (uiFlags.fortaleza) {
                        errorObj.password = "Verifica la fortaleza de la contraseña jc";
                    }
                }
        		if (ui.inputs.passwordConfirm.val() == '') {
        			errorObj.passwordConfirm = "El campo confirmación de contraseña es requerido";
    			}
    			if (ui.inputs.password.val() && ui.inputs.passwordConfirm.val()) {
    				if (ui.inputs.password.val() !== ui.inputs.passwordConfirm.val()) {
    					errorObj.passwordConfirm = "Las contraseñas deben ser las mismas";
    				}
    			}
    			return errorObj;
        	},
            validateStrong: function(password) {
                if (!password) {
                    ui.listElements.removeClass('good bad');
                } else {
                    const regexMin = new RegExp(/^.{8,}$/);
                    const regexUpperLetter = new RegExp(/(?=.*[A-Z])/);
                    const regexLowerLetter = new RegExp(/(?=.*[a-z])/);
                    const regexDigit = new RegExp(/\d/);
                    const regexSpecialChar = new RegExp(/[@!#$%^&+=]/);

                    ui.listElements.removeClass('good bad');
                    uiFlags.fortaleza = false;

                    $(ui.listElements.get(0)).addClass(regexMin.test(password)         ? 'good' : 'bad');
                    $(ui.listElements.get(1)).addClass(regexUpperLetter.test(password) ? 'good' : 'bad');
                    $(ui.listElements.get(2)).addClass(regexLowerLetter.test(password) ? 'good' : 'bad');
                    $(ui.listElements.get(3)).addClass(regexDigit.test(password)       ? 'good' : 'bad');
                    $(ui.listElements.get(4)).addClass(regexSpecialChar.test(password) ? 'good' : 'bad');

                    let pwd = document.getElementById('input-password').value.toUpperCase();
                    let nomUsuario = atob(localStorage.username).toUpperCase();
                    let apPaterno  = atob(localStorage.apPaterno).toUpperCase();
                    let apMaterno  = atob(localStorage.apMaterno).toUpperCase();
                    let usuario    = atob(localStorage.user).toUpperCase();

                    console.log(pwd);
                    console.log(nomUsuario);
                    console.log(apPaterno);
                    console.log(apMaterno);
                    console.log(usuario);

                    //Valida si contiene el nombre del usuario, apellido paterno o materno en el password ingresado
                    let position1 = pwd.search(nomUsuario);
                    if (position1 >= 0) uiFlags.fortaleza = true;
                    $(ui.listElements.get(5)).addClass(uiFlags.fortaleza==false ? 'good' : 'bad');

                    let position2 = pwd.search(apPaterno);
                    if (position1 >= 0 || position2 >= 0 ) uiFlags.fortaleza = true;
                    $(ui.listElements.get(5)).addClass(uiFlags.fortaleza == false ? 'good' : 'bad');

                    let position3 = pwd.search(apMaterno);
                    if (position1 >= 0 || position2 >= 0 || position3 >= 0 ) uiFlags.fortaleza = true;
                    $(ui.listElements.get(5)).addClass(uiFlags.fortaleza == false ? 'good' : 'bad');

                    //Valida si contiene el usuario en el password ingresado
                    let position = pwd.search(usuario);
                    if (position >= 0) { uiFlags.fortaleza = true } else { uiFlags.fortaleza = false };
                    $(ui.listElements.get(6)).addClass(uiFlags.fortaleza == false ? 'good' : 'bad');
                }
            },
        	clearInputErrors: function () {
                ui.form.find('input').removeClass('is-invalid');
            },
        	getFormData: function () {
	            return {
	                password: ui.inputs.password.val(),
	                passwordConfirm: ui.inputs.passwordConfirm.val(),
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
            }
        }

        const callbacks = {
        	changePassword: function (response) {
                ui.buttons.continue.prop('disabled', false);
                uiActions.toggleSpinner();
        		if (response.ok) {
                    const redirectUrl = location.protocol + '//' + API_URL_BASE + UI_URLS.UPDATED_PASSWORD;
                    window.location.href = redirectUrl;
                } else {
                	uiActions.setToastMessage({
                        title: 'Error',
                        message: response.mensaje,
                    });
                	uiActions.showToast();
                };
        	}
        }

        const services = {
        	changePassword: function (password, callback) {
        		const data = {
        			user: localStorage.user,
        			password: btoa(password)
        		};
                $.post(API_URLS.CHANGE_PASSWORD, data).done(callback);
        	}
        }

        const addEventListeners = function () {

            ui.inputs.password.keyup(function (evt){
                uiActions.validateStrong(evt.target.value);
            })
            ui.inputs.password.focus(uiActions.toggleListValidation);
            ui.inputs.passwordConfirm.focus(uiActions.toggleListValidation);
            //ui.inputs.password.blur(uiActions.toggleListValidation);
            //ui.inputs.passwordConfirm.blur(uiActions.toggleListValidation);
            //fin
        	ui.buttons.continue.click(function (evt) {
        		evt.preventDefault();
                uiActions.hideToast();
                uiActions.clearInputErrors();

                const formData = uiActions.getFormData();
                const inputErrors = uiActions.validatePassword();
                uiActions.setInputErrors(inputErrors);

        		if (!Object.keys(inputErrors).length) {
        			ui.buttons.continue.prop('disabled', true);
                    uiActions.toggleSpinner();
        			services.changePassword(formData.password, callbacks.changePassword);
        		} else {
                    uiActions.setToastMessage({
                        title: 'Corregir los campos',
                        message: inputErrors.password || inputErrors.passwordConfirm
                    });
                    uiActions.showToast();
                }
        	});
        }

        return {
            init: init,
        };
    })();

    createPassUI.init();
});