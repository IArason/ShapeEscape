using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerSpawner : LevelEntityAttribute {

    public void SpawnPlayer()
    {
        var player = Resources.Load<GameObject>("Player");
        Instantiate(player, transform.position, Quaternion.identity);
    }

    public override Serialized Serialize(Dictionary<LevelEntity, int> objectToID)
    {
        return new SerializedPlayerSpawner();
    }

    public class SerializedPlayerSpawner : Serialized
    {
        public override void InstantiateSelf(Dictionary<int, LevelEntity> idList, bool editable)
        {
            if(!editable)
                idList[parentID].GetComponent<PlayerSpawner>().SpawnPlayer();
        }
    }

    
}
