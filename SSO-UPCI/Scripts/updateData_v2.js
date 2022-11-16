$(function () {

    $("#IdSpanUserName").text(localStorage.username);
    
    $("#btnvalidar").click(function () {

        var idcelular = $("#idcelular").val();
        var idemail = $("#idemail").val();
        var checkBox = document.getElementById("idterminos");

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
        if (checkBox.checked == false) {
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

        // Devuelve el dominio: localhost:44370 ó micuenta.upc.edu.pe
        var dominio = window.location.host;
        var urlRedirect = "https://" + dominio + "/Home/updateDataConfirm";
        $(location).attr('href', urlRedirect);

        //LLama al controlador /Home/CodeSend
        //var url = "https://" + dominio + "/Home/CodeSend";
        //var data = { user: localStorage.user, usuario_nombre: localStorage.username, Usuario_correoPersonal: localStorage.emailToUpd };
        //$.post(url, data).done(function (data) {
        //    if (data.ok) {
        //        var urlRedirect = "https://" + dominio + "/Home/updateDataConfirm";
        //        $(location).attr('href', urlRedirect);
        //    }
        //    else {
        //        //Muestro mensaje de error
        //        $(".toast").addClass("visible");
        //        $(".toast-title").text("Correo electrónico personal");
        //        $(".toast-message").text(data.mensaje);
        //        //agrego la clase que oculta el spinner, para que no se muestre
        //        $(".spin").addClass("hide");
        //        return;
        //    };
        //});

    });//Fin click
});