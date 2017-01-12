using UnityEngine;

public class Vertex : LevelEntity
{
    [SerializeField]
    Color selectedTint = Color.white;
    [SerializeField]
    Color unselectedTint = Color.white;

    public LevelEntity ParentPoly { get { return parentPoly.GetComponent<LevelEntity>(); } }
    Transform parentPoly;

    SpriteRenderer sprite;

    void Start()
    {
        parentPoly = transform.parent;
        sprite = GetComponentInChildren<SpriteRenderer>();
        transform.tag = "Vertex";
        sprite.color = unselectedTint;
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
        transform.SetParent(parentPoly, true);
    }
}
