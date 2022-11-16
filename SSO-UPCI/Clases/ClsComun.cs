using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SSO_UPCI.Clases
{
    public static class ClsComun
    {
        
        public static string QuitaTilde(string _cadena)
        {
            string _valor = _cadena.Trim();

            _valor = _valor.Replace("á", "a");
            _valor = _valor.Replace("Á", "A");

            _valor = _valor.Replace("é", "e");
            _valor = _valor.Replace("É", "E");

            _valor = _valor.Replace("í", "i");
            _valor = _valor.Replace("Í", "I");

            _valor = _valor.Replace("ó", "o");
            _valor = _valor.Replace("Ó", "O");

            _valor = _valor.Replace("ú", "u");
            _valor = _valor.Replace("Ú", "U");

            return _valor;
        }

    }

}