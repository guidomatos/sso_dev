using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Security.Claims;
using System.Web;
using AuthenticationTypes = System.DirectoryServices.AuthenticationTypes;

public class AD
{
    public AD()
    {
    }

    public class res
    {
        public bool ok;
        public string msg;
        public int code;

        public AdUser AdUser { get; set; }
    }

    public class AdUser
    {
        public string Email { get; set; }
        public string EmailUPC { get; set; }
        public string Phone { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string CN { get; set; }
        public string OU { get; set; }
        public string SamAcountName { get; set; }
        public string ExpiraCuenta { get; set; }
    }

    public res auth(string user, string pass)
    {
        //if (user == "mail@jdbpocketware.com") return new res { ok = true, code = 0, msg = "Ok" };

        string server = getDC();

        int code = -1;
        try
        {
        //TODO: Actualizar al nuevo servicio AD
            using (var _entry = new DirectoryEntry())
            {
                _entry.Username = user;
                _entry.Password = pass;
                _entry.Path = "LDAP://" + server + "/DC=upc,DC=edu,DC=pe";
                DirectorySearcher _searcher = new DirectorySearcher(_entry);
                _searcher.ClientTimeout = TimeSpan.FromSeconds(10);
                _searcher.Filter = "(objectclass=user)";

                SearchResult _sr = _searcher.FindOne();

                return new res { ok = true, msg = "Conexión establecida exitosamente.", code = 0 };
            }
            //return new res { ok = true, msg = "Conexión establecida exitosamente.", code = 0 };
        }
        catch (DirectoryServicesCOMException x)
        {
            //https://www-01.ibm.com/support/docview.wss?uid=swg21290631
            if (x.ExtendedErrorMessage.Contains(" 773")) code = 4;
            else if (x.ExtendedErrorMessage.Contains(" 52e")) code = 3;
            else if (x.ExtendedErrorMessage.Contains(" 775")) code = 1;
            else if (x.ExtendedErrorMessage.Contains(" 533") || x.ExtendedErrorMessage.Contains(" 530") || x.ExtendedErrorMessage.Contains(" 531")) code = 2;
            else if (x.ExtendedErrorMessage.Contains(" 532") || x.ExtendedErrorMessage.Contains(" 701")) code = 5;
            else if (x.ExtendedErrorMessage.Contains(" 525")) code = 6;

            return new res { ok = false, code = code, msg = x.ExtendedErrorMessage };
        }
    }

    public res getUser(string user)
    {
        //if (user == "mail@jdbpocketware.com") return new res { ok = true, code = 0, msg = "Ok", AdUser = new AdUser() { Email = user, EmailUPC = user, Id = user, Name = user, LastName = user } };

        var us = new AdUser();

        var directoryEntry = GetDirectoryEntryByUserName(user);
        if (directoryEntry == null) return new res() { ok = false, msg = "No se puede obtener datos de " + user };

        us.Name = directoryEntry.Properties["givenname"].Count > 0 ? directoryEntry.Properties["givenname"][0].ToString() : user;
        us.LastName = directoryEntry.Properties["sn"].Count > 0 ? directoryEntry.Properties["sn"][0].ToString() : user;
        us.Email = (string)directoryEntry.Properties["mailsso"].Value;
        us.EmailUPC = directoryEntry.Properties["mail"].Count > 0 ? directoryEntry.Properties["mail"][0].ToString() : user;
        us.Phone = (string)directoryEntry.Properties["mobile"].Value;
        //us.Phone = string.IsNullOrEmpty((string)directoryEntry.Properties["mobile"].Value) ? "." : (string)directoryEntry.Properties["mobile"].Value;
        us.OU = (string)directoryEntry.Properties["distinguishedname"].Value;
        us.ExpiraCuenta = directoryEntry.InvokeGet("PasswordExpirationDate").ToString();
        directoryEntry.Close();

        return new res { ok = true, msg = "Usuario " + user + " / DN =" + us.OU + " / Cel = " + us.Phone, AdUser = us, code = 0 };
    }

    private static string getDC()
    {
        return Helpers.Amb == "PRO" ? Helpers.DCPRO : Helpers.Amb == "CER" ? Helpers.DCCER : Helpers.DCDEV;
    }

    public res setEmail(string user, string email, string cel = null)
    {
        DirectoryEntry myLdapConnection = createDirectoryEntry();

        DirectorySearcher search = new DirectorySearcher(myLdapConnection);
        search.ClientTimeout = TimeSpan.FromSeconds(10);
        search.Filter = "(cn=" + user + ")";
        if (!String.IsNullOrEmpty(email)) search.PropertiesToLoad.Add("mailsso");
        if (!String.IsNullOrEmpty(cel)) search.PropertiesToLoad.Add("mobile");
        SearchResult result = search.FindOne();

        if (result != null)
        {
            DirectoryEntry entryToUpdate = result.GetDirectoryEntry();

            if (!String.IsNullOrEmpty(email)) entryToUpdate.Properties["mailsso"].Value = email;
            if (!String.IsNullOrEmpty(cel)) entryToUpdate.Properties["mobile"].Value = cel;

            entryToUpdate.CommitChanges();
            return new res() { ok = true, msg = "Datos actualizados correctamente." };
        }
        else return new res() { ok = false, msg = "Usuario no encontrado" };
    }

    static DirectoryEntry createDirectoryEntry()
    {
        string server = getDC();
        string fullPath = "LDAP://" + server + "/DC=upc,DC=edu,DC=pe";
        string usAdmin = Helpers.adUser;
        string pwdAdmin = Helpers.adPassword;

        DirectoryEntry ldapConnection = new System.DirectoryServices.DirectoryEntry(fullPath, usAdmin, pwdAdmin, AuthenticationTypes.Secure);

        return ldapConnection;
    }

    public res ChangePasswordAD(string user, string pwd)
    {
        try
        {
            var directoryEntry = GetDirectoryEntryByUserName(user);
            if (directoryEntry == null) return new res() { ok = false, msg = "No se puede obtener " + user };
            directoryEntry.Invoke("SetPassword", new object[] { pwd });
            directoryEntry.Properties["LockOutTime"].Value = 0;

            directoryEntry.Close();
            return new res() { ok = true, msg = "Cambio de contraseña exitoso para " + user };
        }
        catch (Exception ex)
        {
            return new res() { ok = false, msg = ex.Message };
        }
    }

    private static DirectoryEntry GetDirectoryEntryByUserName(string userName)
    {
        var de = GetDirectoryObject(Helpers.adUser, Helpers.adPassword);
        var deSearch = new DirectorySearcher(de)
        { SearchRoot = de, Filter = "(&(objectCategory=user)(cn=" + userName + "))", ClientTimeout = TimeSpan.FromSeconds(10) };

        var results = deSearch.FindOne();
        return results != null ? results.GetDirectoryEntry() : null;
    }

    private static DirectoryEntry GetDirectoryObject(string usAdmin, string pwdAdmin)
    {
        string fullPath = "LDAP://" + getDC() + "/DC=upc,DC=edu,DC=pe";
        var directoryEntry = new DirectoryEntry(fullPath, usAdmin, pwdAdmin, AuthenticationTypes.Secure);
        return directoryEntry;
    }

    public SearchResultCollection GetModifiedUsers(DateTime date)
    {
        var dateStr = "20190920000000.0Z";// date.ToString("YYYYMMDDHHmmss.sZ");
        var de = GetDirectoryObject(Helpers.adUser, Helpers.adPassword);
        var deSearch = new DirectorySearcher(de)
        { SearchRoot = de, Filter = "(&(objectCategory=user)(whenChanged>=" + dateStr + "))", ClientTimeout = TimeSpan.FromSeconds(10) };

        var results = deSearch.FindAll();
        return results;
    }
}