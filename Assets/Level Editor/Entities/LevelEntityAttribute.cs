using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(LevelEntity))]
public abstract class LevelEntityAttribute : MonoBehaviour
{
    public bool isSerializable = true;

    [SerializeField]
    protected LevelEntity targetEntity;

    protected virtual void Awake()
    {
        if(targetEntity == null)
            targetEntity = GetComponentInParent<LevelEntity>();

        targetEntity.RegisterListener(
            OnSelect,
            OnDeselect,
            OnPointerEnter,
            OnPointerExit
            );
    }

    protected virtual void OnSelect() { }
    protected virtual void OnDeselect() { }
    protected virtual void OnPointerEnter() { }
    protected virtual void OnPointerExit() { }

    #region Serialization

    public virtual Serialized Serialize(Dictionary<LevelEntity, int> objectToID)
    {
        throw new InvalidOperationException(
            "Error on " + gameObject.name + ", component " + GetType().ToString() + "\n" +
            "This object should either be marked as unserializable or override the serialization functions.\n"
            + "Try disabling the object serializable checkbox.");
    }

    public abstract class Serialized
    {
        public int parentID;
        public int[] targetIDs;

        public virtual void InstantiateSelf(Dictionary<int, LevelEntity> idList, bool editable)
        {
            throw new InvalidOperationException(
                "This object should either be marked as unserializable or override the serialization functions.\n"
                + "Try disabling the object serializable checkbox.");
        }
    }

    #endregion
}