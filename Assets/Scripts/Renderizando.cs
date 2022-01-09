using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Renderizando : MonoBehaviour
{
    public ComputeShader cs;
    public RenderTexture rendertexture;

    void Start()
    {
        rendertexture = new RenderTexture(256, 256, 32);
        rendertexture.enableRandomWrite = true;
        rendertexture.Create();

        cs.SetTexture(0, "Result", rendertexture);
        cs.SetFloats("resolution", rendertexture.width);
        cs.Dispatch(0, rendertexture.width / 8, rendertexture.width / 8, 1);
    }

    void Update()
    {

    }
}
