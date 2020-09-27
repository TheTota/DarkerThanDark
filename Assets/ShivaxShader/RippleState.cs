using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RippleState : MonoBehaviour
{
    public Material material;
    public float speedFactor = 1;
    public float maxRadius;

    private float speed;
    private bool started;

    // Multiple single Wave
    List<Vector4> origins;
    List<float> times;

    void Start()
    {
        maxRadius = material.GetFloat("_MaxRadius");
        origins = new List<Vector4>();
        times = new List<float>();

        material.SetVectorArray("_Origins", new Vector4[255]);
        material.SetFloatArray("_SpeedsOrigin", new float[255]);

    }

    // Update is called once per frame
    void Update()
    {
        material.SetFloat("_MaxRadius", maxRadius);

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit info))
        {
            if (Input.GetMouseButtonDown(0) && !started)
            {
                var worldPos = info.point;

                CreateWave(worldPos);
            }
        }

        HandleWaves();

    }

    private void CreateWave(Vector3 worldPos)
    {
        var origin = new Vector4(worldPos.x, worldPos.y, worldPos.z, 0);

        maxRadius = material.GetFloat("_MaxRadius");

        origins.Add(origin);
        times.Add(0);

        material.SetVectorArray("_Origins", origins);
    }

    private void HandleWaves()
    {
        var speeds = new float[times.Count].ToList();

        // Calculate speeds progression
        for (int i = 0; i < speeds.Count; i++)
        {
            times[i] += speedFactor * Time.deltaTime;
            speeds[i] = 1 - (maxRadius - times[i]) / maxRadius; 
        }

        if(speeds.Count > 0)
        {
            // Update shader
            material.SetFloatArray("_SpeedsOrigin", speeds);
        }
    }

    private void CreateSuccessiveWave(Vector3 worldPos)
    {
        var origin = new Vector4(worldPos.x, worldPos.y, worldPos.z, 0);

        maxRadius = material.GetFloat("_MaxRadius");
        material.SetVector("_Origin", origin);

        StartCoroutine(StartSuccessiveWave(3, 0.5f));
        started = true;
    }

    private IEnumerator StartSuccessiveWave(int number, float timeSpace)
    {
        var time = new List<float>(number);
        var speed = new List<float>(number);
    
        for (int i = 0; i < number; i++)
        {
            time.Add( -i * timeSpace);
            speed.Add(0);

            yield return null;
        }

        float val = 0;

        while(val < 1)
        {
            for (int i = 0; i < number; i++)
            {
                time[i] += speedFactor * Time.deltaTime;
                speed[i] = 1 - (maxRadius - time[i]) / maxRadius;
              
                yield return null;
            }

            material.SetFloatArray("_Speeds", speed);

            val = Mathf.Min(speed.ToArray());
        }

        started = false;
    }

    private void CreateSingleWave(Vector3 worldPos)
    {
        var origin = new Vector4(worldPos.x, worldPos.y, worldPos.z, 0);

        maxRadius = material.GetFloat("_MaxRadius");
        material.SetVector("_Origin", origin);

        StartCoroutine("StartWave");
        started = true;
    }


    private IEnumerator StartWave()
    {
        float time = 0;

        while (speed < 1)
        {
            time += speedFactor * Time.deltaTime;
            speed = 1 - (maxRadius - time) / maxRadius;

            material.SetFloat("_Speed", speed);

            yield return null;
        }

        speed = 0;

        started = false;
    }
}
