using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GravidadeColor : MonoBehaviour
{
    struct Cube
    {
        public Vector3 position;
        public Color color;
        public float Mass;
        public float velocidade;
        public static int cubeCount = 0;
        public int colCheck;
    }


    public ComputeShader cs;

    //Variaveis
    public int iteractions = 50;
    public int count = 100;
    GameObject[] gameObjects;
    Cube[] dt;
    bool RGPU;
    bool RCPU;
    private float timer = 0;

    public bool OnCheck = false;
    public int OnGround = 0;

    // Config
    public GameObject modelPref;

    public float minMass;

    public float maxMass;

    public float minVelocity;

    public float maxVelocity;

    public float Height;

    public float Gap;

    public float tempo;
    void Start()
    {
        gameObjects = new GameObject[count * count];
        OnCheck = false;

    }

    // Update is called once per frame
    void Update()
    {
        if (RGPU)
            ProcessoGPU();
        if (RCPU)
            ProcessoCPU();

        if (dt != null && OnGround >= dt.Length && OnCheck == false)
        {
            OnCheck = true;
            Debug.Log("Tempo de Execução: " + (int)(timer * 1000));
            Debug.Log("Quantidade de Obejtos: " + dt.Length);
        }


        if (OnGround != 0)
        {
            tempo += Time.deltaTime;
            if (tempo >= 3)
            {
                ReloadScene();
                tempo = 0;
            }
        }
    }
    private void ReloadScene()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }
    void OnGUI()
    {
        if (dt == null && OnCheck == false)
        {
            if (GUI.Button(new Rect(0, 0, 100, 50), "Create"))
            {

                dt = new Cube[count * count];
            }
        }
        //      <CPU>
        if (dt != null)
        {
            if (GUI.Button(new Rect(110, 0, 100, 50), "CPU"))
            {
                ExecutarCPU();
            }
        }
        //      <GPU>
        if (dt != null)
        {
            if (GUI.Button(new Rect(220, 0, 100, 50), "GPU"))
            {
                ExecutarGPU();
            }
        }
    }
    private void ExecutarGPU()
    {
        createCube();
        RGPU = true;

    }

    private void ExecutarCPU()
    {
        createCube();
        RCPU = true;
    }
    private void createCube()
    {

        for (int i = 0; i < count; i++)
        {
            for (int j = 0; j < count; j++)
            {

                Color _color = Random.ColorHSV();

                GameObject go = GameObject.Instantiate(modelPref, new Vector3((-count / 2 + i) * Gap, Height, (-count / 2 + j) * Gap), Quaternion.identity);
                go.GetComponent<MeshRenderer>().material.SetColor("_Color", _color);

                gameObjects[i * count + j] = go;

                dt[i * count + j] = new Cube();
                dt[i * count + j].position = go.transform.position;
                dt[i * count + j].color = _color;
                dt[i * count + j].Mass = Random.Range(minMass, maxMass);
                dt[i * count + j].velocidade = Random.Range(minVelocity, maxVelocity);
                dt[1 * count + j].colCheck = 0;
            }
        }
    }
    private void ProcessoGPU()
    {
        timer += Time.deltaTime;

        for (int i = 0; i < dt.Length; i++)
        {
            if (dt[i].colCheck == 0)
            {
                float v = dt[i].velocidade;
                v += (9.8f / dt[i].Mass) * timer;
                dt[i].velocidade = v;
                dt[i].position.y -= dt[i].velocidade * timer;

                if (dt[i].position.y <= 1 && dt[i].colCheck < 1)
                {
                    dt[i].position.y = 1;
                    gameObjects[i].GetComponent<MeshRenderer>().material.SetColor("_Color", Random.ColorHSV());
                    dt[i].colCheck = 1;
                }
                gameObjects[i].transform.position = dt[i].position;

                if (dt[i].colCheck == 1)
                {
                    OnGround++; dt[i].colCheck = 2;
                }
            }
        }
    }
    private void ProcessoCPU()
    {
        timer += Time.deltaTime;
        for (int i = 0; i < dt.Length; i++)
        {
            if (dt[i].colCheck == 0)
            {
                float v = dt[i].velocidade;
                v += (9.8f / dt[i].Mass) * timer;
                dt[i].velocidade = v;
                dt[i].position.y -= dt[i].velocidade * timer;
            }
            if (dt[i].position.y <= 1 && dt[i].colCheck < 1)
            {
                dt[i].position.y = 1;
                gameObjects[i].GetComponent<MeshRenderer>().material.SetColor("_Color", Random.ColorHSV());
                dt[i].colCheck = 1;
            }
            gameObjects[i].transform.position = dt[i].position;

            if (dt[i].colCheck == 1)
            {
                OnGround++;
                dt[i].colCheck = 2;
            }
        }
    }
}
