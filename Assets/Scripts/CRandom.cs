using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CRandom : MonoBehaviour
{
    struct Cube
    {
        public Vector3 position;
        public Color color;
    }

    public ComputeShader cs;
    public int iteractions = 50;
    public int count = 100;
    GameObject[] gameObjects;
    Cube[] dt;
    public GameObject modelPref;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnGUI()
    {
        if (dt == null)
        {
            if (GUI.Button(new Rect(0, 0, 100, 50), "Create"))
            {
                createCube();
            }
        }
//      <CPU>
        if (dt != null)
        {
            if (GUI.Button(new Rect(110, 0, 100, 50), "CPU"))
            {
                for (int k = 0; k < iteractions; k++)
                {
                    for (int i = 0; i < gameObjects.Length; i++)
                    {
                        gameObjects[i].GetComponent<MeshRenderer>().material.SetColor("_Color", Random.ColorHSV());
                    }
                }
            }
        }
//      <GPU>
        if (dt != null)
        {
            if (GUI.Button(new Rect(220, 0, 100, 50), "GPU"))
            {
                int totalSize = 4 * sizeof(float) + 3 * sizeof(float);

                ComputeBuffer computeBuffer = new ComputeBuffer(dt.Length, totalSize);
                computeBuffer.SetData(dt);

                cs.SetBuffer(0, "cubes", computeBuffer);
                cs.SetInt("iteractions", iteractions);

                cs.Dispatch(0, dt.Length / 10, 1, 1);

                computeBuffer.GetData(dt);

                for (int i = 0; i < gameObjects.Length; i++)
                {
                    gameObjects[i].GetComponent<MeshRenderer>().material.SetColor("_Color", dt[i].color);
                }

                computeBuffer.Dispose();

            }
        }
    }
    private void createCube()
    {
        dt = new Cube[count * count];
        gameObjects = new GameObject[count * count];

        for (int i = 0; i < count; i++)
        {
            float offsetX = (-count / 2 + i);

            for (int j = 0; j < count; j++)
            {
                float offsetY = (-count / 2 + j);

                Color _color = Random.ColorHSV();

                GameObject go = GameObject.Instantiate(modelPref, new Vector3(offsetX * 0.7f, 0, offsetY * 0.7f), Quaternion.identity);
                go.GetComponent<MeshRenderer>().material.SetColor("_Color", _color);

                gameObjects[i * count + j] = go;

                dt[i * count + j] = new Cube();
                dt[i * count + j].position = go.transform.position;
                dt[i * count + j].color = _color;
            }
        }
    }
}
