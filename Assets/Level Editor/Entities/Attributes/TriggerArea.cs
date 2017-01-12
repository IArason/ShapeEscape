using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TriggerArea : LevelEntityAttribute {

    // In case of refactoring
    // public bool hold = false;
    public float cooldown = 2f;

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "player")
            GetComponent<Trigger>().OnTrigger();
    }

    public void Initialize(float cooldown, bool editable)
    {
        this.cooldown = cooldown;

        if(!editable)
        {
            Destroy(GetComponent<MeshFilter>());
            Destroy(GetComponent<MeshRenderer>());
        }
    }

    public override Serialized Serialize(Dictionary<LevelEntity, int> objectToID)
    {
        return new SerializedTriggerArea(cooldown);
    }

    public class SerializedTriggerArea : Serialized
    {
        public float cooldown;
            
        public SerializedTriggerArea() { }

        public SerializedTriggerArea(float cooldown) { this.cooldown = cooldown; }

        public override void InstantiateSelf(Dictionary<int, LevelEntity> idList, bool editable)
        {
            idList[parentID].GetComponent<TriggerArea>().Initialize(cooldown, editable);
        }
    }
}
