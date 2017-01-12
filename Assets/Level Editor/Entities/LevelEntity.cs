using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using BitStrap;
using System.Diagnostics;

public class LevelEntity : MonoBehaviour
{
    [Tooltip("Should this object force snap to grid?")]
    public bool translateSnapOnly = false;
    [Tooltip("Should this object force snapped rotations?")]
    public bool rotateSnapOnly = false;
    public bool canTranslate = true;
    public bool canRotate = true;
    public bool canDuplicate = true;

    bool persistentHighlight = false;

    // Callback events
    protected event Action OnSelectEvent;
    protected event Action OnDeselectEvent;
    protected event Action OnPointerEnterEvent;
    protected event Action OnPointerExitEvent;

    int originalLayer = 0;

    void Awake()
    {
        originalLayer = gameObject.layer;
    }

    public virtual void OnSelect()
    {
        transform.position += Vector3.back * 5;

        InvokeOnSelect();

        if (persistentHighlight) return;

        OnPointerExit();

        foreach (Transform t in GetComponentsInChildren<Transform>())
        {
            if (t.tag != "Vertex")
                t.gameObject.layer = LayerMask.NameToLayer("Outline");
        }
    }

    public virtual void OnDeselect()
    {
        transform.position -= Vector3.back * 5;

        InvokeOnDeselect();

        if (persistentHighlight) return;

        // Prevents OnStartHover effects from getting stuck on selection.
        OnPointerExit();

        foreach (Transform t in GetComponentsInChildren<Transform>())
        {
            if (t.tag != "Vertex")
                t.gameObject.layer = originalLayer;
        }
    }

    public virtual void OnPointerEnter()
    {
        // Ignore hover if selected
        if (gameObject.layer == LayerMask.NameToLayer("Outline")) return;

        InvokeOnPointerEnter();

        foreach (Transform t in GetComponentsInChildren<Transform>())
        {
            if (t.tag != "Vertex")
                t.gameObject.layer = LayerMask.NameToLayer("OutlineHover");
        }
    }

    public virtual void OnPointerExit()
    {
        // Ignore hover if selected
        if (gameObject.layer == LayerMask.NameToLayer("Outline")) return;

        InvokeOnPointerExit();

        if (persistentHighlight) return;

        foreach (Transform t in GetComponentsInChildren<Transform>())
        {
            if (t.tag != "Vertex")
                t.gameObject.layer = originalLayer;
        }
    }

    public void HighlightPersistent(bool value)
    {
        persistentHighlight = value;
        foreach (Transform t in GetComponentsInChildren<Transform>())
        {
            if (t.tag != "Vertex")
                t.gameObject.layer = value ? LayerMask.NameToLayer("OutlineHover") : originalLayer;
        }
    }

    protected void InvokeEvent(Action ev)
    {
        ev.Invoke();
    }

    // Needed for Vertex class to invoke the events.
    protected void InvokeOnSelect() { if (OnSelectEvent != null) OnSelectEvent.Invoke(); }
    protected void InvokeOnDeselect() { if (OnDeselectEvent != null) OnDeselectEvent.Invoke(); }
    protected void InvokeOnPointerEnter() { if (OnPointerEnterEvent != null) OnPointerEnterEvent.Invoke(); }
    protected void InvokeOnPointerExit() { if (OnPointerExitEvent != null) OnPointerExitEvent.Invoke(); }

    public void RegisterListener(
        Action OnSelect = null,
        Action OnDeselect = null,
        Action OnPointerEnter = null,
        Action OnPointerExit = null
        )
    {
        if(OnSelect != null)
            OnSelectEvent += OnSelect;
        if (OnDeselect != null)
            OnDeselectEvent += OnDeselect;
        if (OnPointerEnter != null)
            OnPointerEnterEvent += OnPointerEnter;
        if (OnPointerExit != null)
            OnPointerExitEvent += OnPointerExit;
    }

    public void UnregisterListener(
        Action OnSelect,
        Action OnDeselect,
        Action OnPointerEnter,
        Action OnPointerExit
        )
    {
        OnSelectEvent -= OnSelect;
        OnDeselectEvent -= OnDeselect;
        OnPointerEnterEvent -= OnPointerEnter;
        OnPointerExitEvent -= OnPointerExit;
    }

    #region Serialization

    // Used to disable serialization of objects which are
    // children of other objects, such as vertices on polygons.
    public bool isSerializable = true;

    const string levelEditorResourcePath = "LevelEditor/";

    [SerializeField]
    ObjectClass objectClass = ObjectClass.Prop;

    /// <summary>
    /// Updates the data about this object.
    /// </summary>
    public Serialized SerializeEntity(int ID)
    {
        // Process the name to remove (1) or (Clone).
        string properName = name;
        int index = name.IndexOf(" (");

        if (index == -1)
            index = name.IndexOf("(");

        if (index != -1)
            properName = name.Substring(0, index);

        string prefabPath = levelEditorResourcePath + objectClass.ToString() + "/" + properName;
        

        return new Serialized(
            ID,
            prefabPath,
            transform.position,
            transform.rotation,
            transform.localScale
            );
    }

    public class Serialized
    {
        // Scaling is commented out as it is never used but I already put in the code.

        // Stored in a dictionary on serialization.
        // Used for connecting component references.
        public int instanceID;
        public string prefabPath;

        public float posX;
        public float posY;
        public float posZ;

        public float rotX;
        public float rotY;
        public float rotZ;
        public float rotW;

        /*
        public float scaleX;
        public float scaleY;
        public float scaleZ;
        */

        public Serialized(
            int instanceID, 
            string prefabPath, 
            Vector3 position, 
            Quaternion rotation, 
            Vector3 scale
            )
        {
            this.instanceID = instanceID;
            this.prefabPath = prefabPath;

            posX = position.x;
            posY = position.y;
            posZ = position.z;

            rotX = rotation.x;
            rotY = rotation.y;
            rotZ = rotation.z;
            rotW = rotation.w;

            /*
            scaleX = scale.x;
            scaleY = scale.y;
            scaleZ = scale.z;
            */
        }

        /// <summary>
        /// Creates a LevelEntity using the data found in this class.
        /// </summary>
        public void InstantiateSelf(out LevelEntity self, out int id)
        {
            var prefab = Resources.Load<GameObject>(prefabPath);
            if(prefab == null)
            {
                UnityEngine.Debug.LogError("No prefab at path " + prefabPath);
                self = null;
                id = -1;
                return;
            }
            GameObject obj = (GameObject)Instantiate(Resources.Load<GameObject>(prefabPath),
            new Vector3(posX, posY, posZ),
            new Quaternion(rotX, rotY, rotZ, rotW));
            
            //obj.transform.localScale = new Vector3(scaleX, scaleY, scaleZ);

            self = obj.GetComponent<LevelEntity>();
            id = instanceID;
        }
    }

    /// <summary>
    /// Returns 
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static string GetUntil(string str, string pattern)
    {
        int l = str.IndexOf(pattern);
        if (l > 0)
        {
            return str.Substring(0, l);
        }
        return "";

    }

    #endregion
}

