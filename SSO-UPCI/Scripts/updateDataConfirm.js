$(function () {
    if (localStorage.SMSEnviado == "NO") {
        $("#IdSpanMail").text(localStorage.emailToUpd);
    }
    else {
        $("#IdSpanMail").text(localStorage.celToUpd);
    }

    function foco(idElemento) {
        document.getElementById(idElemento).focus();
    }
    foco("input-code1");
    $('#input-code1').keyup(function () { foco("input-code2") })
    $('#input-code2').keyup(function () { foco("input-code3") })
    $('#input-code3').keyup(function () { foco("input-code4") })


    $("#btn-confirmar").click(function () {
        var code1 = $("#input-code1").val();
        var code2 = $("#input-code2").val();
        var code3 = $("#input-code3").val();
        var code4 = $("#input-code4").val();

        if (code1 == "" || code2 == "" || code3 == "" || code4 == "")
        {
            //Muestro bordes de color rojo 
            $("#input-code1").addClass("is-invalid");
            $("#input-code2").addClass("is-invalid");
            $("#input-code3").addClass("is-invalid");
            $("#input-code4").addClass("is-invalid");

            //Muestro mensaje de error
            $(".toast").addClass("visible");
            $(".toast-message").text("Ingreso incorrecto");
            return;
        }

        //quito la clase que oculta el spinner, para que se muestre
        $(".spin").removeClass("hide");

        //arma el código de verificación
        var code = code1 + code2 + code3 + code4;

        // Devuelve el dominio: localhost:44370 ó micuenta.upc.edu.pe
        var dominio = window.location.host;                         

        //LLama al controlador /Home/CodeEval (evalua el CODE ingresado)
        var url = "https://" + dominio + "/Home/CodeEval";
        var data = { user: localStorage.user, code: code };
        $.post(url, data).done(function (data) {
            if (data.ok) {
                if (localStorage.SMSEnviado == "NO") {
                    //LLama al controlador /Home/CodeSendSMS para enviar un CODE por SMS
                    var url = "https://" + dominio + "/Home/CodeSendSMS";
                    var data = { user: localStorage.user, usuario_nombre: localStorage.username, usuario_telefono: localStorage.celToUpd, tipoMensaje: "2" };
                    $.post(url, data).done(function (data) {
                        if (data.ok) {
                            localStorage.SMSEnviado = "SI";
                            var urlRedirect = "https://" + dominio + "/Home/updateDataConfirm";
                            $(location).attr('href', urlRedirect);
                            return;
                        }
                        else {
                            //Muestro mensaje de error
                            $(".toast").addClass("visible");
                            $(".toast-message").text(data.mensaje);
                            //agrego la clase que oculta el spinner, para que no se muestre
                            $(".spin").addClass("hide");
                            return;
                        }
                    });
                }
                else
                {
                        //LLama al controlador /Home/updateDataCRM para actualizar: mail y celular
                        var url = "https://" + dominio + "/Home/updateDataCRM";
                        var data = { user: localStorage.user, Usuario_correoPersonal: localStorage.emailToUpd, usuario_telefono: localStorage.celToUpd };
                        $.post(url, data).done(function (data) {
                            if (data.ok) {
                                var urlRedirect = "https://" + dominio + "/Home/updateDataEnd";
                                $(location).attr('href', urlRedirect);
                            }
                            else {
                                //Muestro mensaje de error
                                $(".toast").addClass("visible");
                                $(".toast-message").text(data.mensaje);
                                //agrego la clase que oculta el spinner, para que no se muestre
                                $(".spin").addClass("hide");
                                return;
                            }
                        });
                }
            }
            else
            {
                //Muestro mensaje de error
                $(".toast").addClass("visible");
                $(".toast-message").text(data.mensaje);
                //agrego la clase que oculta el spinner, para que no se muestre
                $(".spin").addClass("hide");
            }
        });

    });

    $("#link-reenviar").click(function () {

        // Devuelve el dominio: localhost:44370 ó micuenta.upc.edu.pe
        var dominio = window.location.host;   
        //LLama al controlador /Home/CodeSend
        if (localStorage.SMSEnviado == "NO")
        {
            var url = "https://" + dominio + "/Home/CodeSend";
            var data = { user: localStorage.user, usuario_nombre: localStorage.username, Usuario_correoPersonal: localStorage.emailToUpd };
        }
        else
        {
            var url = "https://" + dominio + "/Home/CodeSendSMS";
            var data = { user: localStorage.user, usuario_nombre: localStorage.username, usuario_telefono: localStorage.celToUpd };
        }
        $.post(url, data).done(function (data) {
            if (data.ok == false) {
                //Muestro mensaje de error
                $(".toast").addClass("visible");
                $(".toast-message").text(data.mensaje);
                return;
            };
        });
        $("#link-reenviar").text("Reenviado");

    });

});