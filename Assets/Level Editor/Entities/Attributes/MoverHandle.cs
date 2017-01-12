using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MoverHandle : LevelEntity
{
    [SerializeField]
    Color selectedTint = Color.white;
    [SerializeField]
    Color unselectedTint = Color.white;

    public LevelEntity ParentMover { get {
            if(parentMover == null) parentMover = transform.GetComponentInParent<Mover>();
            return parentMover.GetComponent<LevelEntity>();
        } }
    Mover parentMover;

    SpriteRenderer sprite;
    
    void Start()
    {
        parentMover = transform.GetComponentInParent<Mover>();
        sprite = GetComponentInChildren<SpriteRenderer>();
        transform.tag = "Vertex";
        sprite.color = unselectedTint;
        isSerializable = false;
    }

    public override void OnSelect()
    {
        InvokeOnSelect();
        sprite.color = selectedTint;
    }

    public override void OnDeselect()
    {
        InvokeOnDeselect();
        sprite.color = unselectedTint;
        transform.SetParent(parentMover.transform, true);
    }

}
