using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using VRNoShake;
public class SwitchWeapon : MonoBehaviour
{
    private SteamVR_Action_Boolean m_GrabGrip = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("GrabGrip");//SteamVR抓取
    
    public UseKalman NoShakePosition;
    public UseKalmanRotate NoShakeRotation;

    public GameObject[] WeaponList;
    public Material IsUse;
    int i = 0;


    // Update is called once per frame
    void Update()
    {
        //左手按下抓持键，关闭滤波
        //open/close 
        if (m_GrabGrip.GetStateDown(SteamVR_Input_Sources.LeftHand))
        {
            if (NoShakePosition.IsOpen)
            {
                NoShakePosition.IsOpen = false;
                NoShakeRotation.IsOpen = false;
                IsUse.SetColor("_Color",Color.white);
            }
            else
            {
                NoShakePosition.IsOpen = true;
                NoShakeRotation.IsOpen = true;
                IsUse.SetColor("_Color", Color.red);
            }
            
        }


        //右手按下抓持键，更换武器
        //SwitchWeapon
        if (m_GrabGrip.GetStateDown(SteamVR_Input_Sources.RightHand))
        {
            WeaponList[i].SetActive(false);
            i++;
            if (i >= WeaponList.Length)
            {
                i = 0;
            }
            WeaponList[i].SetActive(true);

            //更新为当前武器的滤波参数
            OnSwitch(WeaponList[i].GetComponent<NoShake>());
            
        }
    }

    //切换武器
    //set this
    void OnSwitch(NoShake Weapon)
    {
        NoShakePosition.MaxV = Weapon.MaxPositionV;
        NoShakeRotation.MaxV = Weapon.MaxRotationV;
    }
}
