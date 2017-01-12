using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

[System.Serializable]
public class Level
{
    public LevelEntity.Serialized[] sceneEntities;
    public LevelEntityAttribute.Serialized[] sceneComponents;
    public ColorTheme.Serialized levelColorTheme;

    public string levelName = "MyLevel";

    public Level(string name)
    {
        levelName = name;
    }

    public void SaveLevel()
    {
        // Serialize gameobjects
        var gameObjects = Object.FindObjectsOfType<LevelEntity>().Where(x => x.isSerializable && x.gameObject.activeInHierarchy).ToArray();
        sceneEntities = new LevelEntity.Serialized[gameObjects.Length];

        // Pairs IDs and gameobjects
        Dictionary<LevelEntity, int> idList = new Dictionary<LevelEntity, int>();

        for (int i = 0; i < gameObjects.Length; i++)
        {
            sceneEntities[i] = gameObjects[i].SerializeEntity(i);
            if (sceneEntities[i] == null) Debug.LogError(gameObjects[i].name + "serialized to null");
            idList.Add(gameObjects[i], i);
        }

        // Serialize components
        var components = Object.FindObjectsOfType<LevelEntityAttribute>().Where(x => x.isSerializable && x.gameObject.activeInHierarchy).ToArray();
        sceneComponents = new LevelEntityAttribute.Serialized[components.Length];

        for (int i = 0; i < components.Length; i++)
        {
            //dynamic comp = components[i];
            sceneComponents[i] = components[i].Serialize(idList);
            sceneComponents[i].parentID = idList[components[i].GetComponent<LevelEntity>()];
        }

        levelColorTheme = ColorManager.Instance.selectedTheme.Serialize();
    }

    public void LoadLevel(bool playable)
    {
        // Pairs IDs and gameobjects
        Dictionary<int, LevelEntity> idList = new Dictionary<int, LevelEntity>();

        foreach(LevelEntity.Serialized g in sceneEntities)
        {
            LevelEntity obj;
            int id;
            g.InstantiateSelf(out obj, out id);
            if(obj != null)
                idList.Add(id, obj.GetComponent<LevelEntity>());
        }

        if (sceneComponents == null)
        {
            Debug.LogError("sceneComponents null");
            return;
        }
        foreach(LevelEntityAttribute.Serialized c in sceneComponents)
        {
            c.InstantiateSelf(idList, !playable);
        }

        ColorManager.Instance.selectedTheme = levelColorTheme.Deserialize();
    }

    public string Serialize()
    {
        var serializer = new JsonSerializer { TypeNameHandling = TypeNameHandling.All };
        serializer.NullValueHandling = NullValueHandling.Ignore;

        return JsonConvert.SerializeObject(this, Formatting.None, new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All
        });
    
    }
}
