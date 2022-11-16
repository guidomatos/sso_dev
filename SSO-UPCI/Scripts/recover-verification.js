$(document).ready(function () {
    const recoverVerificationUI = (function () {
        let ui = {};

        const init = function () {
            const $form = $("form");
            const $inputs = $form.find("input:text");
            ui = {
                form: $form,
                allInputs: $inputs,
                inputs: {
                	code1: $inputs[0],
                	code2: $inputs[1],
                	code3: $inputs[2],
                	code4: $inputs[3],
                },
                buttons: {
                	confirm: $('#btn-confirmar')
                },
                links: {
                	reSend: $('#link-reenviar'),
                },
                titleSection: $('#title-section'),
                toast: $('.toast'),
            };

            addEventListeners();
            uiActions.setFocus(ui.inputs.code1);
			uiActions.positionToast();
        };

        const uiFlags = {
        	toast: false,
            resending: false,
        }

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
        	setToastText: function(message) {
        		ui.toast.find('.toast-message').text(message);
        	},
        	setFocus: function($element) {
        		$element.focus();
        	},
        	getCompletecode: function() {
        		let code = '';
        		ui.allInputs.each(function (idx, input) {
        			code += input.value;
        		});
        		return code;
        	},
        	setInputErrors: function () {
        		ui.allInputs.each(function (idx, input) {
        			if (input.value == '' || isNaN(input.value)) {
        				input.classList.add('is-invalid');
        				input.value = '';
        			}
        		});
        	},
            forceSetInputErrors: function() {
                ui.allInputs.addClass('is-invalid');
            },
        	clearInputErrors: function () {
                ui.allInputs.removeClass('is-invalid');
            },
            clearInputs: function () {
                ui.allInputs.val('');
            },
        	validateCode: function(code) {
        		uiActions.clearInputErrors();
        		uiActions.setInputErrors();
        		if (code == "" || code.length < 4 || isNaN(code)) {
        			return false;
        		}
        		return true;
        	},
        	getLocalData: function () {
        		if (localStorage.enviadoPor == "SMS") {
        			return {
        				user: localStorage.user,
        				usuario_nombre: localStorage.username,
        				usuario_telefono: localStorage.cel
        			};
        		}
        		if (localStorage.enviadoPor == "CORREO") {
    			    return {
        			   	user: localStorage.user,
        			   	usuario_nombre: localStorage.username,
        			   	Usuario_correoPersonal: localStorage.email
        			};
        		}
        	},
            waitResending: function () {
                if (!uiFlags.resending) {
                    ui.links.reSend.text('Reenviando ...');
                    uiFlags.resending = true;
                } else {
                    ui.links.reSend.text('Reenviar');
                    uiFlags.resending = false;
                }
            }
        };

        const services = {
        	reSend: function(callback) {
        		const data = uiActions.getLocalData();
        		const serviceUrl = localStorage.enviadoPor == "SMS" ? API_URLS.PROCESS_CODE_SMS : API_URLS.PROCESS_CODE_EMAIL;
        		$.post(serviceUrl, data).done(callback);
        	},
        	sendCode: function (code, callback) {
        		 var data = { user: localStorage.user, code: code };
                $.post(API_URLS.EVALUATE_CODE, data).done(callback);
        	},
        };

        const callbacks = {
        	reSend: function (response) {
                uiActions.waitResending();
        		if (response.ok) {
                    uiActions.clearInputErrors();
                    uiActions.clearInputs();
                    // TODO: indicar al usuario que revise su correo
                } else {
                    uiActions.setToastText(response.mensaje);
                    uiActions.showToast();
                }
        	},
        	sendCode: function (response) {
        		if (response.ok) {
        			const redirectUrl = location.protocol + '//' + API_URL_BASE + UI_URLS.CREATE_PASS;
                    window.location.href = redirectUrl;
                } else {
                    uiActions.forceSetInputErrors();
                	uiActions.setToastText(response.mensaje);
                	uiActions.showToast();
                };
        	}
        };

        const addEventListeners = function () {
        	ui.inputs.code1.onkeyup = function () {
        		uiActions.hideToast();
        		if (ui.inputs.code1.value.length > 0) {
        			uiActions.setFocus(ui.inputs.code2);
        		}
        	};
        	ui.inputs.code2.onkeyup = function () {
        		uiActions.hideToast();
        		if (ui.inputs.code1.value.length > 0) {
        			uiActions.setFocus(ui.inputs.code3);
        		}
        	};
        	ui.inputs.code3.onkeyup = function () {
        		uiActions.hideToast();
        		if (ui.inputs.code1.value.length > 0) {
	        		uiActions.setFocus(ui.inputs.code4);
	        	}
        	};
            ui.inputs.code4.onkeyup = function () {
                uiActions.hideToast();
                uiActions.clearInputErrors();
            };

        	ui.buttons.confirm.click(function (evt) {
        		evt.preventDefault();
        		const code = uiActions.getCompletecode();
        		if (!uiActions.validateCode(code)) {
        			uiActions.setToastText('Ingrese los dígitos correctos');
                    uiActions.showToast();
        			return;
        		}
        		services.sendCode(code, callbacks.sendCode);
        	});

        	ui.links.reSend.click(function(evt) {
                evt.preventDefault();
                if (uiFlags.resending) {
                    console.log('no post and return...');
                    return;
                }
                uiActions.hideToast();
                uiActions.waitResending();
        		services.reSend(callbacks.reSend);
        	});
        };

        return {
            init: init,
        };
 	})();

    recoverVerificationUI.init();
});