using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class AnimationConverter : EditorWindow
{
    private AnimationClip selectedClip;
    private AnimationClip lastSelected;

    private string clipDescription;
    private string resultDescription;
    private bool canConvert;

    private bool autoFireLock;
    private bool autoFire;

    [MenuItem("Window/Util/LiqTech Animation Converter")]
    static void Init()
    {
        GetWindow<AnimationConverter>().Show();
    }

    void OnGUI()
    {
        titleContent = new GUIContent("Anim. Conv.");

        selectedClip = (AnimationClip)EditorGUILayout.ObjectField(
            new GUIContent("Clip", "Clip to convert"),
        selectedClip, typeof(AnimationClip), false);

        EditorGUILayout.Space();

        if (selectedClip != lastSelected)
        {
            setupClip();
            lastSelected = selectedClip;

            if(autoFire)
            {
                convertClip();
            }
        }

        EditorGUILayout.LabelField(clipDescription);

        GUI.enabled = !autoFire && canConvert;
        if (GUILayout.Button("Convert frames to use UI.Image"))
        {
            convertClip();
        }
        GUI.enabled = true;


        EditorGUILayout.LabelField(resultDescription);


        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        //EditorGUILayout.LabelField(" ", GUILayout.wi);
        var afrText = autoFireLock ? "[Autofire release]" : " Autofire release ";
        if (GUILayout.Button(afrText, GUILayout.Width(120)))
        {
            autoFireLock = !autoFireLock;
            autoFire = false;
        }
        GUI.enabled = autoFireLock;

        var afTex = autoFire ? "[Autofire]" : " Autofire ";
        if (GUILayout.Button(afTex, GUILayout.Width(120)))
        {
            autoFire = !autoFire;
        }
        GUI.enabled = true;
        EditorGUILayout.LabelField("(Instant conversion)");
        EditorGUILayout.EndHorizontal();

        

    }

    private void setupClip()
    {
        if (!autoFire)
        {
            autoFireLock = false;
        }

        canConvert = false;

        if (selectedClip == null)
        {
            clipDescription = "No clip selected";
        }
        else
        {
            var bindings = AnimationUtility.GetObjectReferenceCurveBindings(selectedClip);

            if (bindings.Length == 0)
            {
                clipDescription = "Clip has no bindings!";
            }
            else if (bindings.Length > 1)
            {
                clipDescription = "Clip has more than two bindings!";
            }
            else
            {
                var b = bindings[0];
                var frames = AnimationUtility.GetObjectReferenceCurve(selectedClip, b);

                clipDescription = "Animation on " + b.type.ToString()
                    + " with " + frames.Length.ToString()
                    + " keyframes.";

                if (b.type == typeof(SpriteRenderer))
                {
                    canConvert = true;
                }
            }
        }
    }

    private void convertClip()
    {
        resultDescription = "";

        if (selectedClip == null)
        {
            resultDescription = "Clip is missing!";
            return;
        }

        var bindings = AnimationUtility.GetObjectReferenceCurveBindings(selectedClip);

        if (bindings.Length != 1)
        {
            resultDescription = "Wrong bindings count!";
            return;
        }

        var b = bindings[0];

        if (b.type == typeof(Image))
        {
            resultDescription = "Already converted.";
            return;
        }
        else if (b.type != typeof(SpriteRenderer))
        {
            resultDescription = "Can't convert this type.";
            return;
        }

        var frames = AnimationUtility.GetObjectReferenceCurve(selectedClip, b);

        AnimationUtility.SetObjectReferenceCurve(selectedClip, b, null);

        b.type = typeof(Image);
        AnimationUtility.SetObjectReferenceCurve(selectedClip, b, frames);

        resultDescription = "Successfully converted animation";

        Undo.RecordObject(selectedClip, "Changed binding type");
    }
}