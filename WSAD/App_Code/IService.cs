using System;
using System.ServiceModel;

[ServiceContract]
public interface IService
{
    [OperationContract]
    AD.res Auth(string alias, string password);

    [OperationContract]
    AD.res GetUser(string alias);

    [OperationContract]
    AD.res SetEmail(string alias, string email);

    [OperationContract]
    AD.res ChangePassword(string username, string password);

    [OperationContract]
    string ModifiedUsers(DateTime date);
}


