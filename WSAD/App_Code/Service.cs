using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

// NOTA: puede usar el comando "Rename" del menú "Refactorizar" para cambiar el nombre de clase "Service1" en el código, en svc y en el archivo de configuración.
public class Service : IService
{
    public AD.res Auth(string alias, string password)
    {
        var ad = new AD();
        var r = ad.auth(alias, password);
        return new AD.res() { ok = r.ok, msg = r.msg, code = r.code };
    }

    public AD.res GetUser(string alias)
    {
        var ad = new AD();
        var r = ad.GetUser(alias);
        return new AD.res() { ok = r.ok, msg = r.msg, AdUser = r.AdUser, code = r.code };
    }

    public AD.res SetEmail(string alias, string email)
    {
        var ad = new AD();
        var r = ad.SetEmail(alias, email);
        return new AD.res() { ok = r.ok, msg = r.msg, code = r.code };
    }

    public AD.res ChangePassword(string username, string password)
    {
        var ad = new AD();
        var r = ad.ChangePasswordAD(username, password);
        return r;
    }

    public string ModifiedUsers(DateTime date)
    {
        var ad = new AD();
        var r = ad.GetModifiedUsers(date);
        return r.Count.ToString();
    }
}
