$(function () {

    $("#IdSpanUserName").text(localStorage.username);

    //Devuelve el dominio: localhost:44370 ó micuenta.upc.edu.pe
    const dominio = window.location.host;

    //LLama al controlador /Home/getDataCRM para traer el celular y correo de CRM
    const Celular = document.getElementById("idcelular");
    const Correo = document.getElementById("idemail");
    var url = "https://" + dominio + "/Home/getDataCRM";
    var data = { user: localStorage.user};
    $.post(url, data).done(function (data) {

        if (data.ok)
        {
            Celular.value = data.celular;
            Correo.value  = data.correo;
        }

    });

    $("#btnvalidar").click(function () {

        var idcelular = $("#idcelular").val();
        var idemail   = $("#idemail").val();
        var checkBox  = document.getElementById("idterminos");

        if (idcelular == "") {
            //Muestro bordes de color rojo 
            $("#idcelular").addClass("is-invalid");
            //Muestro mensaje de error
            $(".toast").addClass("visible");
            $(".toast-title").text("Error de ingreso");
            $(".toast-message").text("Ingrese celular");
            return;
        }
        if (idemail == "") {
            //Muestro bordes de color rojo 
            $("#idemail").addClass("is-invalid");
            //Muestro mensaje de error
            $(".toast").addClass("visible");
            $(".toast-title").text("Error de ingreso");
            $(".toast-message").text("Ingrese correo electrónico personal");
            return;
        }
        if (checkBox.checked == false)
        {
            $("#divTerminos").attr('style', 'border: 1px solid #ff0000; border-radius: 5px 5px 5px 5px;');
            //Muestro mensaje de error
            $(".toast").addClass("visible");
            $(".toast-title").text("Error de ingreso");
            $(".toast-message").text("Aceptar los términos y condiciones");
            return;
        }

        //quito la clase que oculta el spinner, para que se muestre
        $(".spin").removeClass("hide");

        // Crea local storage
        localStorage.emailToUpd = idemail;
        localStorage.celToUpd   = idcelular;

        //LLama al controlador /Home/CodeSend para enviar el CODE por mail
        var url = "https://" + dominio + "/Home/CodeSend";           
        var data = {user:localStorage.user, usuario_nombre:localStorage.username, Usuario_correoPersonal:localStorage.emailToUpd, tipoMensaje:"2"};
        $.post(url, data).done(function (data) {
            if (data.ok) {
                localStorage.SMSEnviado = "NO"; //Inicializa el envio por SMS
                var urlRedirect = "https://" + dominio + "/Home/updateDataConfirm"; 
                $(location).attr('href', urlRedirect);
            }
            else {
                //Muestro mensaje de error
                $(".toast").addClass("visible");
                $(".toast-title").text("Correo electrónico personal");
                $(".toast-message").text(data.mensaje);
                //agrego la clase que oculta el spinner, para que no se muestre
                $(".spin").addClass("hide");
                return;
            };
        });
    });//Fin click

});