$(document).ready(function () {

    var cadena;

    $('#txtPas1').keyup(function ()
    {
        $('#strengthMessage').html(checkStrength($('#txtPas1').val()));
        $('#fortaleza').html(cadena);
    })

    function checkStrength(password) {
        cadena = ""
        // Longitud 8
        if (password.length > 7) cadena = cadena + " lg:8"
        // Si la contraseña contiene minúscula
        if (password.match( /[a-z]/ ) ) cadena = cadena + " minúscula."
        // Si la contraseña mayúscula
        if (password.match( /[A-Z]/ ) ) cadena = cadena + " mayúscula."
        // Si tiene números
        if (password.match( /[0-9]/ ) ) cadena = cadena + " #"
        // Si tiene un carácter especial, aumenta el valor de fortaleza.
        if (password.match( /[!,%,&,@,#,$,^,*,?,_,~]/)) cadena = cadena + " Símbolo"

        //================================================
        var strength = 0
        // longitud 8, aumenta fortaleza 
        if (password.length > 7) strength += 2
        // Si la contraseña contiene minúscula, aumenta el valor de fortaleza.
        if (password.match( /[a-z]/ ) ) strength += 1
        // Si la contraseña contiene mayúscula, aumenta el valor de fortaleza.
        if (password.match( /[A-Z]/ ) ) strength += 1
        // Si tiene números, aumenta el valor de fortaleza.
        if (password.match( /[0-9]/ ) ) strength += 1
        // Si tiene un carácter especial, aumenta el valor de fortaleza.
        if (password.match(/[!,%,&,@,#,$,^,*,?,_,~]/)) strength += 1

        // Evalua el valor la fortaleza
        if (strength == 0) {
            $('#strengthMessage').removeClass()
            return ''
        }else if (strength < 5) {
            $('#strengthMessage').removeClass()
            $('#strengthMessage').addClass('Short')
            return 'Débil'
        } else if (strength == 5) {
            $('#strengthMessage').removeClass()
            $('#strengthMessage').addClass('Good')
            return 'Buena'

        } else {
            $('#strengthMessage').removeClass()
            $('#strengthMessage').addClass('Strong')
            return 'Fuerte'
        }

    }
});