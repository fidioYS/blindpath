using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
public class Shoot : MonoBehaviour
{

    private SteamVR_Action_Boolean m_GrabPinch = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("GrabPinch");//SteamVR���
    // Start is called before the first frame update
    public Transform FirePoint;//��ǹ��
    public GameObject Bullet;//�ӵ�
    public float V = 30f;//�ӵ��ٶ�
    // Update is called once per frame
    void Update()
    {
        if (gameObject.activeSelf)
        {
            if (m_GrabPinch.GetStateDown(SteamVR_Input_Sources.RightHand))
            {
                GameObject newbullet =  Instantiate(Bullet);
                newbullet.transform.position = FirePoint.position;
                newbullet.transform.rotation = FirePoint.rotation;
                newbullet.GetComponent<Rigidbody>().linearVelocity = newbullet.transform.forward * V;
            }
        }
    }
}
