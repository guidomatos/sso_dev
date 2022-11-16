$(function () {

    $("#IdSpanMail").text(localStorage.emailToUpd);
    $("#IdSpanCel").text(localStorage.celToUpd);
    

    $("#btn-confirmar").click(function () {

        //quito la clase que oculta el spinner, para que se muestre
        $(".spin").removeClass("hide");
        // Devuelve el dominio: localhost:44370 ó micuenta.upc.edu.pe
        var dominio = window.location.host;
        //LLama al controlador /Home/updateDataCRM
        var url = "https://" + dominio + "/Home/updateDataCRM";
        var data = { user: localStorage.user, Usuario_correoPersonal: localStorage.emailToUpd, usuario_telefono: localStorage.celToUpd };
        $.post(url, data).done(function (data) {
            if (data.ok) {
                var url = "https://" + dominio + "/Home/ConfirmSend";
                var data = {
                    user:                   localStorage.user,
                    usuario_nombre:         localStorage.username,
                    usuario_telefono:       localStorage.celToUpd,
                    Usuario_correoPersonal: localStorage.emailToUpd,
                    flagSendConfirm:        true
                };
                $.post(url, data).done(function (data) {
                    if (data.ok) {
                        var urlRedirect = "https://" + dominio + "/Home/updateDataEnd";
                        $(location).attr('href', urlRedirect);
                    }
                    else {
                        MsgError(data.mensaje);
                    }
                });
            }
            else {
                MsgError(data.mensaje);
            }
        });

    });
    function MsgError(mensaje) {
        $(".toast").addClass("visible");
        $(".toast-message").text(mensaje);
        $(".spin").addClass("hide");
        return;
    }

});