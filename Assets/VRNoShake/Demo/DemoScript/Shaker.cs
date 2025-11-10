using System.Collections;
using System.Collections.Generic;
using UnityEngine;



//shake
public class Shaker : MonoBehaviour
{
    Vector3 SourcePoint;//
    Vector3 SourceEuler;
    public bool PositionShake = true;
    public bool RotationShake = true;

    public Transform Contrast;//¶Ô±È
    private void Start()
    {
        Application.targetFrameRate = 120;//120HZ
        SourcePoint = transform.position;
        SourceEuler = transform.eulerAngles;
    }

    // Update is called once per frame
    void Update()
    {
        if (PositionShake)
        {
            transform.position = SourcePoint + (Random.value - 0.5f) * Vector3.one * 0.2f;
        }

        if (RotationShake)
        {
            transform.eulerAngles = SourceEuler + (Random.value - 0.5f) * Vector3.one * 50f;
        }

        Contrast.position = transform.position + new Vector3(-3, 0, 0);
        Contrast.rotation = transform.rotation;
    }
}
