using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public static class LevelEditor {

    public static void InstantiateObject(GameObject prefab, Vector2 screenOffset)
    {
        var pos = LevelEditorUtils.GetClosestSnappedToGrid(
            ObjectManipulator.Instance.gridUnit,
            LevelEditorUtils.ScreenTo2DWorldPlane(
            new Vector2((Screen.width - screenOffset.x) / 2f, (Screen.height - screenOffset.y) / 2f)));
       
        var go = (GameObject)UnityEngine.Object.Instantiate(prefab, pos, prefab.transform.rotation);

        new UndoInstantiate(go);

        var entity = go.GetComponent<LevelEntity>();
        ObjectManipulator.Instance.SetSelection(entity);
    }
}

// .ToString() used to designate subfolder under Resources/LevelEditor/**
public enum ObjectClass
{
    Dynamic,
    Hazard,
    Prop,
    Polygon,
    Special
}