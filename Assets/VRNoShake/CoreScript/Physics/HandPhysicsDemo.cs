using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandPhysicsDemo : MonoBehaviour
{
    public GameObject hand;//��ģ��,HandPhysics-3
    public Transform offset;//��ƫ�ƣ�important
    

    public bool IsPhysicsHand = true;//�Ƿ�����ײ�����������, Is Enable Physics

    Rigidbody rigidbody;//The HandPhysics father

    //target
    Vector3 targetPosition;
    Quaternion targetRotation;

    [Header("Maybe don't need change")]
    float Speed = 1f;//��������,convergence speed 
    public float MoveSpeedMax = 1500;
    public float RotaSpeedMax = 100;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = hand.GetComponent<Rigidbody>();
    }



    private void Update()
    {
        if (rigidbody.isKinematic == false)
        {
            targetPosition = offset.position;
            targetRotation = offset.rotation;

            if (IsPhysicsHand)
            {
                //Get velocity Target
                Vector3 velocityTarget = (targetPosition - rigidbody.position) * MoveSpeedMax * Time.fixedUnscaledDeltaTime * Speed;

                //Get angularVelocity Target
                Quaternion rotationDelta = targetRotation * Quaternion.Inverse(rigidbody.rotation);
                float angle;
                Vector3 axis;
                rotationDelta.ToAngleAxis(out angle, out axis);
                Vector3 angularTarget;
                if (angle > 180)
                    angle -= 360;

                if (angle != 0 && float.IsNaN(axis.x) == false && float.IsInfinity(axis.x) == false)
                {
                    angularTarget = angle * axis * 50f * Time.fixedUnscaledDeltaTime * Speed;
                }
                else
                {
                    angularTarget = Vector3.zero;
                }

                //Set rigibody V and AV Target
                rigidbody.linearVelocity = Vector3.MoveTowards(rigidbody.linearVelocity, velocityTarget, 50);
                rigidbody.angularVelocity = Vector3.MoveTowards(rigidbody.angularVelocity, angularTarget, RotaSpeedMax);
                if (Vector3.Distance(transform.position, hand.transform.position) > 1f)
                {
                    rigidbody.linearVelocity = Vector3.zero;
                    rigidbody.angularVelocity = Vector3.zero;
                    hand.transform.position = transform.position;
                }
            }
            else
            {
                rigidbody.linearVelocity = Vector3.zero;
                rigidbody.angularVelocity = Vector3.zero;
                rigidbody.MovePosition(targetPosition);
                rigidbody.MoveRotation(targetRotation);
            }
        }
    }
}
