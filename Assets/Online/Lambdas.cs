using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Amazon.Lambda.Model;
using System.Text;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using Amazon.Runtime;
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System.Linq;

public static class Lambdas {

    public static string cachedPlayerName = "N/A";

    /// <summary>
    /// Changes the current user's name to the given name.
    /// </summary>
    public static void SetOrUpdatePlayerName(string playerName, Action<string> callback)
    {
        // Returns Message:Success or Message:Failure

        var payloadValues = new Dictionary<string, string>();
        CognitoSingleton.Instance.Credentials.GetIdentityIdAsync((id) =>
        {

            payloadValues.Add("PlayerName", playerName);
            payloadValues.Add("PlayerID", id.Response);

            string payload = Serialize(payloadValues);

            CognitoSingleton.Instance.LambdaClient.InvokeAsync(new InvokeRequest()
            {
                FunctionName = "SetOrUpdatePlayerName",
                Payload = payload
            },
            (responseObject) =>
            {
                if (responseObject.Exception == null)
                {
                    var vals = Deserialize<Dictionary<string, string>>(ResponseObjectToString(responseObject));
                    if (vals["Message"] == "Success") cachedPlayerName = playerName;
                    callback(vals["Message"]);
                }
                else
                {
                    callback("Error");
                }
            }
            );
        });

    }

    /// <summary>
    /// Returns the username of the currently logged on player via a callback
    /// </summary>
    public static void GetPlayerName(Action<string> callback)
    {
        // Returns Message:Failure or Message:Success,PlayerName:%s

        var payloadValues = new Dictionary<string, string>()
        {
            { "PlayerID" , CognitoSingleton.Instance.Credentials.GetIdentityId() }
        };

        string payload = Serialize(payloadValues);
        
        CognitoSingleton.Instance.LambdaClient.InvokeAsync(new InvokeRequest()
        {
            FunctionName = "GetPlayerName",
            Payload = payload
        },
        (responseObject) =>
        {
            if (responseObject.Exception == null)
            {
                Debug.Log(ResponseObjectToString(responseObject));

                try
                {

                    var vals = Deserialize<Dictionary<string, string>>(ResponseObjectToString(responseObject));

                    if (vals["Message"] == "Success")
                    {

                        Debug.Log(vals["PlayerName"]);
                        cachedPlayerName = vals.ContainsKey("PlayerName") ? vals["PlayerName"] : "N/A";
                        Debug.Log(cachedPlayerName);
                        Debug.Log("Cached name");
                        callback(cachedPlayerName);
                    }
                    else
                    {
                        callback("N/A");
                    }
                }
                catch(Exception e)
                {
                    Debug.Log(e);
                }
            }
            else
            {
                Debug.Log("Exception:" + responseObject.Exception);
            }
        }
        );
    }

    /// <summary>
    /// Uploads level data and updates level name.
    /// </summary>
    public static void AddOrUpdateLevel(SharedLevel level, 
        Action<string> message)
    {
        // Returns Message:Failure or Message:Success,LevelID:%s
        var payloadValues = new Dictionary<string, string>()
        {
            { "PlayerID" , CognitoSingleton.Instance.Credentials.GetIdentityId() },
            { "LevelData" , level.LevelJSON },
            { "LevelName" , level.LevelName }
        };

        if (level.LevelID != null)
            payloadValues.Add("LevelID", level.LevelID);
        

        string payload = JsonConvert.SerializeObject(payloadValues, Formatting.None, new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.None
        });

        // string payload = DictionaryToJsonParser(payloadValues);

        CognitoSingleton.Instance.LambdaClient.InvokeAsync(new InvokeRequest()
        {
            FunctionName = "AddOrUpdateLevel",
            Payload = payload
        },
        (responseObject) =>
        {
            if (responseObject.Exception == null)
            {
                var json = ResponseObjectToString(responseObject);
                var dict = Deserialize<Dictionary<string, string>>(json);

                Debug.Log(dict["Message"]);
                if(dict["Message"] == "Success")
                {
                    level.LevelID = dict["LevelID"];
                    GameManager.CacheLevel(
                            level
                        );
                }

                message(dict["Message"]);
            }
            else
            {
                Debug.Log("Exception:" + responseObject.Exception);
                message("Error");
            }
        }
        );
    }

    /// <summary>
    /// Returns all levels owned by this player.
    /// </summary>
    public static void GetPlayerLevels(Action<List<SharedLevel>> callback)
    {
        // Returns Message:Failure or Message:Success,LevelID:%s
        var payloadValues = new Dictionary<string, string>()
        {
            { "PlayerID" , CognitoSingleton.Instance.Credentials.GetIdentityId() }
        };

        string payload = Serialize(payloadValues);

        CognitoSingleton.Instance.LambdaClient.InvokeAsync(new InvokeRequest()
        {
            FunctionName = "GetPlayerLevels",
            Payload = payload
        },
        (responseObject) =>
        {
            if (responseObject.Exception == null)
            {
                var json = ResponseObjectToString(responseObject);

                List<Dictionary<string, string>> levels = new List<Dictionary<string, string>>();

                Debug.Log("Starting try");
                try
                {
                    IDictionary<string, object> dict = 
                        JsonConvert.DeserializeObject<IDictionary<string, object>>(
                        json);

                    JArray thing = dict["Levels"] as JArray;
                    List<object> obj = thing.ToObject<List<object>>();
                    foreach(JObject o in obj)
                    {
                        levels.Add(o.ToObject<Dictionary<string, string>>());
                    }
                }
                catch (Exception e)
                {
                    Debug.Log(e);
                    return;
                }
                Debug.Log("Finished try");

                callback(levels.Select(x => CreateOrGetCachedLevel(x)).ToList());
            }
            else
            {
                Debug.Log("Exception:" + responseObject.Exception);
            }
        }
        );
    }

    public static void DeleteLevel(SharedLevel level)
    {
        DeleteLevel(level.LevelID);
    }

    /// <summary>
    /// Deletes a level owned by the current player
    /// </summary>
    public static void DeleteLevel(string levelID)
    {
        var payloadValues = new Dictionary<string, string>()
        {
            { "PlayerID" , CognitoSingleton.Instance.Credentials.GetIdentityId() },
            { "LevelID" , levelID }
        };

        string payload = Serialize(payloadValues);

        CognitoSingleton.Instance.LambdaClient.InvokeAsync(new InvokeRequest()
        {
            FunctionName = "DeleteLevel",
            Payload = payload
        },
        (responseObject) =>
        {
            if (responseObject.Exception == null)
            {
                var json = ResponseObjectToString(responseObject);
                var msg = Deserialize<Dictionary<string, string>>(json);

                Debug.Log(msg["Message"]);
            }
            else
            {
                Debug.Log("Exception:" + responseObject.Exception);
            }
        }
        );
    }
    
    public enum SortBy
    {
        AgeAsc,
        AgeDesc,
        Popularity
    }

    /// <summary>
    /// Returns a given amount of randomly selected levels.
    /// </summary>
    public static void GetLevels(int count, SortBy sort, Action<List<SharedLevel>> callback)
    {
        // Returns Message:Failure or Message:Success,LevelID:%s
        var payloadValues = new Dictionary<string, string>()
        {
            { "PlayerID" , CognitoSingleton.Instance.Credentials.GetIdentityId() },
            { "Count" , count.ToString() },
            { "SortBy", sort.ToString() }
        };

        string payload = Serialize(payloadValues);

        CognitoSingleton.Instance.LambdaClient.InvokeAsync(new InvokeRequest()
        {
            FunctionName = "GetLevels",
            Payload = payload
        },
        (responseObject) =>
        {
            if (responseObject.Exception == null)
            {
                var json = ResponseObjectToString(responseObject);

                List<Dictionary<string, string>> levels = new List<Dictionary<string, string>>();
                
                try
                {
                    IDictionary<string, object> dict =
                        JsonConvert.DeserializeObject<IDictionary<string, object>>(
                        json);

                    JArray thing = dict["Levels"] as JArray;
                    List<object> obj = thing.ToObject<List<object>>();
                    foreach (JObject o in obj)
                    {
                        levels.Add(o.ToObject<Dictionary<string, string>>());
                    }
                }
                catch (Exception e)
                {
                    Debug.Log(e);
                    return;
                }
                
                try
                {
                    var list = levels.Select(x => CreateOrGetCachedLevel(x)).ToList();
                    callback(list);
                }
                catch(Exception e) { Debug.Log(e); }
            }
            else
            {
                Debug.Log("Exception:" + responseObject.Exception);
            }
        }
        );
    }

    /// <summary>
    /// Gets one random level with less than 10 playcount
    /// </summary>
    public static void Discover(Action<SharedLevel> callback)
    {
        // Returns Message:Failure or Message:Success,LevelID:%s
        var payloadValues = new Dictionary<string, string>()
        {
            { "PlayerID" , CognitoSingleton.Instance.Credentials.GetIdentityId() },
            { "Discover", "Y" }
        };

        string payload = Serialize(payloadValues);

        CognitoSingleton.Instance.LambdaClient.InvokeAsync(new InvokeRequest()
        {
            FunctionName = "GetLevels",
            Payload = payload
        },
        (responseObject) =>
        {
            if (responseObject.Exception == null)
            {
                var json = ResponseObjectToString(responseObject);

                Dictionary<string, string> level = new Dictionary<string, string>();
                
                try
                {
                    if (json == null)
                    {
                        Debug.Log(json);
                        callback(null);
                        return;
                    }

                    IDictionary<string, object> dict =
                        JsonConvert.DeserializeObject<IDictionary<string, object>>(
                        json);

                    if (dict == null)
                    {
                        Debug.Log(dict);
                        callback(null);
                        return;
                    }
                    JArray thing = dict["Levels"] as JArray;

                    if (thing == null)
                    {
                        Debug.Log(thing);
                        callback(null);
                        return;
                    }
                    List<object> obj = thing.ToObject<List<object>>();
                    if (obj == null || obj.Count == 0)
                    {
                        Debug.Log(obj.Count);
                        callback(new SharedLevel());
                        Debug.Log("returning");
                        return;
                    }
                    level = ((JObject)obj[0]).ToObject<Dictionary<string, string>>();
                }
                catch (Exception e)
                {
                    Debug.Log(e);
                    return;
                }


                callback(CreateOrGetCachedLevel(level));
            }
            else
            {
                Debug.Log("Exception:" + responseObject.Exception);
            }
        }
        );

    }

    /// <summary>
    /// Returns the level data of the given level ID.
    /// </summary>
    /// <param name="levelID"></param>
    public static void GetLevelData(SharedLevel level, Action callback)
    {
        if (level.LevelID == null) return;

        // Returns Message:Failure or Message:Success,LevelID:%s
        var payloadValues = new Dictionary<string, string>()
        {
            { "LevelID" , level.LevelID }
        };

        string payload = Serialize(payloadValues);

        CognitoSingleton.Instance.LambdaClient.InvokeAsync(new InvokeRequest()
        {
            FunctionName = "GetLevelData",
            Payload = payload
        },
        (responseObject) =>
        {
            if (responseObject.Exception == null)
            {
                var json = ResponseObjectToString(responseObject);

                string levelData = "";
                
                try
                {
                    Dictionary<string, string> dict = Deserialize<Dictionary<string, string>>(json);

                    if(dict.ContainsKey("LevelData"))
                    {
                        levelData = dict["LevelData"];
                    }else
                    {
                        Debug.Log(dict["Message"]);
                    }
                    
                    level.LevelJSON = levelData;

                }
                catch (Exception e)
                {
                    Debug.Log(e);
                    return;
                }
                callback();
            }
            else
            {
                Debug.Log("Exception:" + responseObject.Exception);
            }
        }
        );

    }

    public static SharedLevel CreateOrGetCachedLevel(Dictionary<string, string> vals)
    {
        foreach(var v in GameManager.cachedLevels)
        {
            if ((vals.ContainsKey("PlayerName") && vals["PlayerName"] == v.AuthorName &&
                vals.ContainsKey("LevelName") && vals["LevelName"] == v.LevelName) ||
                (vals.ContainsKey("LevelID") && vals["LevelID"] == v.LevelID))
            {
                return v;
            }
        }
        SharedLevel level = new SharedLevel(vals);
        GameManager.CacheLevel(level);
        return level;
    }


    public static void AddOrUpdateRating(SharedLevel level, int stars)
    {
        AddOrUpdateRating(level.LevelID, stars);
    }

    /// <summary>
    /// Adds or updates the rating of a given level.
    /// </summary>
    public static void AddOrUpdateRating(string levelID, int stars)
    {
        if (levelID == null) return;

        // Returns Message:Failure or Message:Success,LevelID:%s
        var payloadValues = new Dictionary<string, string>()
        {
            { "PlayerID" , CognitoSingleton.Instance.Credentials.GetIdentityId() },
            { "LevelID" , levelID },
            { "Stars" , stars.ToString() }
        };

        string payload = Serialize(payloadValues);

        CognitoSingleton.Instance.LambdaClient.InvokeAsync(new InvokeRequest()
        {
            FunctionName = "AddOrUpdateRating",
            Payload = payload
        },
        (responseObject) =>
        {
            if (responseObject.Exception == null)
            {
                var json = ResponseObjectToString(responseObject);
                Debug.Log(Deserialize(json)["Message"]);
            }
            else
            {
                Debug.Log("Exception:" + responseObject.Exception);
            }
        }
        );
    }


    public static void GetShareableLink(SharedLevel level, Action<string> callback)
    {
        GetShareableLink(level.LevelID, callback);
    }


    /// <summary>
    /// Gets a shareable link for a given level.
    /// </summary>
    /// <param name=""></param>
    public static void GetShareableLink(string levelID, Action<string> callback)
    {
        if (levelID == null) return;

        // Returns Message:Failure or Message:Success,LevelID:%s
        var payloadValues = new Dictionary<string, string>()
        {
            { "LevelID" , levelID }
        };

        string payload = Serialize(payloadValues);

        CognitoSingleton.Instance.LambdaClient.InvokeAsync(new InvokeRequest()
        {
            FunctionName = "GetShareableLink",
            Payload = payload
        },
        (responseObject) =>
        {
            if (responseObject.Exception == null)
            {
                var json = ResponseObjectToString(responseObject);
                var dict = Deserialize(json);
                Debug.Log(dict["Message"]);
                if(dict.ContainsKey("Link"))
                    callback(dict["Link"]);
            }
            else
            {
                Debug.Log("Exception:" + responseObject.Exception);
            }
        }
        );
    }

    /// <summary>
    /// Gets the level ID associated with a shareable link
    /// </summary>
    public static void GetLevelFromShareableLink(string shareableLink, Action<SharedLevel> callback)
    {
        // Returns Message:Failure or Message:Success,LevelID:%s
        var payloadValues = new Dictionary<string, string>()
        {
            { "ShareableLink" , shareableLink }
        };

        string payload = Serialize(payloadValues);

        CognitoSingleton.Instance.LambdaClient.InvokeAsync(new InvokeRequest()
        {
            FunctionName = "GetLevelFromShareableLink",
            Payload = payload
        },
        (responseObject) =>
        {
            if (responseObject.Exception == null)
            {
                var json = ResponseObjectToString(responseObject);

                Debug.Log(json);
                Dictionary<string, string> level = new Dictionary<string, string>();
                string message = "";
                Debug.Log("Starting try");
                try
                {
                    IDictionary<string, object> dict =
                        JsonConvert.DeserializeObject<IDictionary<string, object>>(
                        json);

                    if (dict.ContainsKey("Levels"))
                    {
                        Debug.Log(dict["Levels"].GetType());
                        JObject levels = dict["Levels"] as JObject;
                        if (levels != null)
                        {
                            level = levels.ToObject<Dictionary<string, string>>();
                        }
                        else Debug.Log("Level is null");
                        JArray messages = dict["Messages"] as JArray;
                        var messageDictObj = messages.ToObject<List<object>>()[0];
                        message = ((JObject)messageDictObj).ToObject<Dictionary<string, string>>()["Message"];
                    }
                    else
                    {
                        Debug.Log("Did not contain levels");
                    }
                }
                catch (Exception e)
                {
                    Debug.Log(e);
                    return;
                }
                

                foreach(var v in level)
                {
                    Debug.Log(v);
                }

                if (message == "Success")
                    callback(CreateOrGetCachedLevel(level));
                else
                    callback(null);
            }
            else
            {
                Debug.Log("Exception:" + responseObject.Exception);
            }
        }
        );
    }
    
    static JsonSerializer serializer = new JsonSerializer
    {
        TypeNameHandling = TypeNameHandling.None,
        NullValueHandling = NullValueHandling.Ignore
    };

    private static string Serialize(object obj)
    {
        string payload = JsonConvert.SerializeObject(obj, Formatting.None, new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.None
        });
        return payload;
    }

    private static Dictionary<string, string> Deserialize(string json)
    {
        return Deserialize<Dictionary<string, string>>(json);
    }

    private static T Deserialize<T>(string json)
    {
        T obj = JsonConvert.DeserializeObject<T>
        (json, new JsonSerializerSettings()
        { TypeNameHandling = TypeNameHandling.All });
        return obj;
    }

    private static string ResponseObjectToString(AmazonServiceResult<InvokeRequest, InvokeResponse> responseObject)
    {
        return Encoding.ASCII.GetString(responseObject.Response.Payload.ToArray());
    }
}
