using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoShake : MonoBehaviour
{
    [Range(0, 20f)]
    public float MaxPositionV = 2f;//速度平滑最大值，该值越大，武器越重越不跟手，默认为2
    [Range(0, 1000f)]
    public float MaxRotationV = 100f;//角速度平滑最大值，该值越大，武器越重越不跟手，默认为100
}
