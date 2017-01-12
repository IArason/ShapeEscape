using UnityEngine;
using System.Collections.Generic;

public class WobbleEffect : MonoBehaviour
{
    static RenderTexture mainCamRT;

    [SerializeField]
    LayerMask layer;

    [SerializeField]
    Shader Post_Outline;
    [SerializeField]
    Shader DrawSimple;
    
    [SerializeField]
    new Camera camera;

    Material Post_Mat;

    [Range(1, 10), SerializeField]
    int outlineWidth = 2;

    [Range(0, 1), SerializeField]
    float highlightIntensity = 0.1f;

    [SerializeField]
    Color outlineColor = Color.cyan;

    [SerializeField]
    bool dottedLine = true;

    void Start()
    {
        camera.enabled = false;
        Post_Mat = new Material(Post_Outline);
        
        camera.clearFlags = CameraClearFlags.Color;
        camera.backgroundColor = Color.black;
    }

    [ImageEffectOpaque]
    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Camera.main.targetTexture = mainCamRT;

        camera.orthographicSize = GetComponent<Camera>().orthographicSize;

        Post_Mat.SetInt("_OutlineWidth", outlineWidth);
        Post_Mat.SetColor("_OutlineColor", outlineColor);
        Post_Mat.SetInt("_DottedLine", dottedLine ? 1 : 0);
        Post_Mat.SetFloat("_HighlightIntensity", highlightIntensity);

        //cull any layer that isn't the outline
        camera.cullingMask = layer.value;

        //make the temporary rendertexture
        RenderTexture TempRT = new RenderTexture(source.width, source.height, 0, RenderTextureFormat.R8);
        
        //set the camera's target texture when rendering
        camera.targetTexture = TempRT;

        //render all objects this camera can render, but with our custom shader.
        camera.RenderWithShader(DrawSimple, "");

        Graphics.Blit(TempRT, mainCamRT, Post_Mat);

        TempRT.Release();
    }
}