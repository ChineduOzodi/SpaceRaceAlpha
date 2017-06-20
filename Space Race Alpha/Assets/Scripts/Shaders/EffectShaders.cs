using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectShaders : MonoBehaviour {

    public Material gravity;
	// Use this for initialization

    [ExecuteInEditMode]
	//void OnEnable () {
	//	if(gravity != null)
 //       {
 //           GetComponent<Camera>().SetReplacementShader(gravity, "RenderType");
 //       }
	//}
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, gravity);
    }
 //   // Update is called once per frame
 //   void OnDisable () {
 //       GetComponent<Camera>().ResetReplacementShader();
	//}
}
