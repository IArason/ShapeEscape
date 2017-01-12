using Amazon;
using Amazon.CognitoIdentity;
using Amazon.Lambda;
using Amazon.Runtime;
using UnityEngine;

public class CognitoSingleton : PersistentSingleton<CognitoSingleton>
{
    public string IdentityPoolId = "us-east-1:3d03e8ed-931d-4bd2-b0a2-0aa2700a4e6d";
    public string CognitoIdentityRegion = RegionEndpoint.USEast1.SystemName;
    private RegionEndpoint _CognitoIdentityRegion
    {
        get { return RegionEndpoint.GetBySystemName(CognitoIdentityRegion); }
    }
    public string LambdaRegion = RegionEndpoint.USEast1.SystemName;
    private RegionEndpoint _LambdaRegion
    {
        get { return RegionEndpoint.GetBySystemName(LambdaRegion); }
    }

    void Start()
    {
        UnityInitializer.AttachToGameObject(gameObject);
    }

    private IAmazonLambda _lambdaClient;
    private CognitoAWSCredentials _credentials;
    public CognitoAWSCredentials Credentials
    {
        get
        {
            if (_credentials == null)
                _credentials = new CognitoAWSCredentials(IdentityPoolId, _CognitoIdentityRegion);
            return _credentials;
        }
    }

    public IAmazonLambda LambdaClient
    {
        get
        {
            if (_lambdaClient == null)
            {
                _lambdaClient = new AmazonLambdaClient(Credentials, _LambdaRegion);
            }
            return _lambdaClient;
        }
    }

    /*
    [BitStrap.Button]
    public void ListFunctions()
    {

        Debug.Log("Listing all of your Lambda functions...");
        Client.ListFunctionsAsync(new Amazon.Lambda.Model.ListFunctionsRequest(),
        (responseObject) =>
        {
            string text = "";
            if (responseObject.Exception == null)
            {
                text += "Functions: \n";
                foreach (FunctionConfiguration function in responseObject.Response.Functions)
                {
                    text += "    " + function.FunctionName + "\n";
                }
            }
            else
            {
                text += responseObject.Exception + "\n";
            }
            eventOutput = text;
        }
        );
    }*/
}
