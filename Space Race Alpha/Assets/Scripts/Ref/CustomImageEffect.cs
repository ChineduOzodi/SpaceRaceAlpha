using UnityEngine;
using System.Collections;

public class CustomImageEffect : MonoBehaviour {

    public Material Effectmaterial;
    
    void OnRenderImage(RenderTexture src, RenderTexture dst)
    {
        Graphics.Blit(src, dst, Effectmaterial);
    }
}
