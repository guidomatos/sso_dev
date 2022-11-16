using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

/// <summary>
/// Descripción breve de Helpers
/// </summary>
public class Helpers
{
    public static string adUser = ConfigurationManager.AppSettings["adUser"];
    public static string adPassword = ConfigurationManager.AppSettings["adPassword"];
    public static string Amb = ConfigurationManager.AppSettings["AMB"];
    public static string DCDEV = ConfigurationManager.AppSettings["DCDEV"];
    public static string DCCER = ConfigurationManager.AppSettings["DCCER"];
    public static string DCPRO = ConfigurationManager.AppSettings["DCPRO"];

    public Helpers()
    {
        //
        // TODO: Agregar aquí la lógica del constructor
        //
    }
}