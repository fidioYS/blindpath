using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace VRNoShake
{
    public class UseKalmanRotate : MonoBehaviour
    {
        public GameObject TargetObj;//Target
        GameObject TargetObjTidai;//
                                  //***********************************
        bool IsUseKalman = false;//Don't Use it
        bool IsUseOneEuroFilter = true;
        public bool IsOpen = true;
        bool IsAdjustValueByV = true;

        KalmanRotation kalman;
        Quaternion OneEuroFilter;
        Quaternion Kalman;
        Quaternion TargetPoint;


        [Range(0, 5f)]
        float Y = 3f;
        [Range(0, 50f)]
        float C = 40f;

        Vector3 oldv1;

        Quaternion oldPoint1, oldPoint2;

        //********************
        [Range(0, 1f)]
        float AddValue = 0.7f;
        [Range(0, 1000f)]
        public float MaxV = 100f;

        float TargetAddValue;
        float LastRagetValue = 1;




        //****************************************************************
        void Start()
        {
            TargetObjTidai = new GameObject();
            IsUseKalman = false;//Don't Use it
            IsUseOneEuroFilter = true;
            kalman = new KalmanRotation();
            //init
            oldPoint1 = transform.rotation;
            oldPoint2 = transform.rotation;

        }


        //****************************************************************
        //****************************************************************
        private void Update()
        {
            if (Mathf.Sqrt(oldv1.magnitude) > MaxV)
            {
                TargetAddValue = 1f;
            }
            else
            {
                TargetAddValue = 0.01f + 0.95f * (Mathf.Sqrt(oldv1.magnitude) / MaxV);
                if (TargetAddValue < LastRagetValue)
                {
                    TargetAddValue = Mathf.Lerp(LastRagetValue, TargetAddValue, Time.unscaledDeltaTime * 10);
                }
                LastRagetValue = TargetAddValue;
            }


            if (IsAdjustValueByV)
            {
                AddValue = TargetAddValue;
            }


            //If open,Go!
            if (IsOpen)
            {
                KaUpdata();
                kalman.SetKalman(Y, C);
            }
            else
            {

                TargetPoint = transform.rotation;
            }

            TargetObj.transform.rotation = TargetPoint;

        }



        //****************************************************************
        public void KaUpdata()
        {
            oldv1 = FixedV(oldPoint1.eulerAngles, oldPoint2.eulerAngles) / Time.unscaledDeltaTime;//speed

            Kalman = kalman.KalmanRotate(transform, oldv1, Time.unscaledDeltaTime, TargetObjTidai.transform);//Kalman
            OneEuroFilter = Quaternion.Lerp(TargetObj.transform.rotation, transform.rotation, AddValue);//OneEuro

            if (IsUseKalman && IsUseOneEuroFilter)
            {
                TargetPoint = Quaternion.Lerp(OneEuroFilter, Kalman, AddValue);
            }
            else if (IsUseKalman)
            {
                TargetPoint = Kalman;
            }
            else if (IsUseOneEuroFilter)
            {
                TargetPoint = OneEuroFilter;
            }


            oldPoint2 = oldPoint1;
            oldPoint1 = TargetPoint;
            TargetObjTidai.transform.rotation = TargetObj.transform.rotation;//updata
        }


        //V change Fix
        public Vector3 FixedV(Vector3 oldpoint1, Vector3 oldpoint2)
        {
            float x = 0, y = 0, z = 0;
            if (Mathf.Abs(oldpoint1.x - oldpoint2.x) > 200)
            {
                if (oldpoint1.x - oldpoint2.x > 0)
                {
                    x = oldpoint1.x - oldpoint2.x - 360f;
                }
                else if (oldpoint1.x - oldpoint2.x < 0)
                {
                    x = oldpoint1.x - oldpoint2.x + 360f;
                }
            }
            else
            {
                x = oldpoint1.x - oldpoint2.x;
            }

            if (Mathf.Abs(oldpoint1.y - oldpoint2.y) > 200)
            {
                if (oldpoint1.y - oldpoint2.y > 0)
                {
                    y = oldpoint1.y - oldpoint2.y - 360f;
                }
                else if (oldpoint1.y - oldpoint2.y < 0)
                {
                    y = oldpoint1.y - oldpoint2.y + 360f;//
                }
            }
            else
            {
                y = oldpoint1.y - oldpoint2.y;
            }

            if (Mathf.Abs(oldpoint1.z - oldpoint2.z) > 200)
            {
                if (oldpoint1.z - oldpoint2.z > 0)
                {
                    z = oldpoint1.z - oldpoint2.z - 360f;
                }
                else if (oldpoint1.z - oldpoint2.z < 0)
                {
                    z = oldpoint1.z - oldpoint2.z + 360f;
                }
            }
            else
            {
                z = oldpoint1.z - oldpoint2.z;
            }
            return new Vector3(x, y, z);
        }


    }

}

