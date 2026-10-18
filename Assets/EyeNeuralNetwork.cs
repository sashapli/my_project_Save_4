using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeNeuralNetwork : MonoBehaviour
{
    public Camera Eye;
    public Texture2D CurrentTex;
    void Start()
    {
        CurrentTex = new Texture2D(28, 28, TextureFormat.RGBA32, false);
        CurrentTex.wrapMode = TextureWrapMode.Clamp;
    }

    // Update is called once per frame
    void Update()
    {
        RenderTexture.active = Eye.targetTexture;

        Rect rect = new Rect(0, 0, 28, 28);
        CurrentTex.ReadPixels(rect, 0, 0);
        CurrentTex.Apply();

        RenderTexture.active = null;
    }
}
