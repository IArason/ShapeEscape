using System;
using UnityEngine;

public class ConnectionManager : MonoBehaviour {

    public static string cognitoID
    {
        get
        {
            return _cognitoID;
        }
    }
    static string _cognitoID;
    
    public static void Connect(Action<bool> callback)
    {
        CognitoSingleton.Instance.Credentials.GetIdentityIdAsync((id) =>
        {
            if(id.Exception == null)
            {
                _cognitoID = id.Response;
                callback(true);
            }
            else
            {
                callback(false);
            }
        });

    }
}
