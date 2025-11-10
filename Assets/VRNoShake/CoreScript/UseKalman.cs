using UnityEngine;

namespace VRNoShake
{
    public class UseKalman : MonoBehaviour
    {
        public GameObject TargetObj;
        public bool IsUseKalman = true;
        public bool IsUseOneEuroFilter = true;
        public bool IsOpen = true;
        //***********************************
        Kalman kalman;
        Vector3 OneEuroFilter;
        Vector3 Kalman;
        Vector3 TargetPoint;//Final

        //Adjust Kalman
        [Range(0, 3f)]
        public float Y = 1f;
        [Range(0, 50f)]
        public float C = 40f;

        //some
        Vector3 oldv1;
        Vector3 oldv2;
        Vector3 av;
        Vector3 oldPoint1, oldPoint2, oldPoint3;

        [Range(0, 1f)]
        float TargetAddValue = 0;
        float AddValue = 0.7f;
        bool IsAdjustValueByV = true;
        [Range(0, 20f)]
        public float MaxV = 2f;

        // Start is called before the first frame update
        void Start()
        {
            //IsUseKalman = true;
            //IsUseOneEuroFilter = true;
            kalman = new Kalman();
            //Init
            oldPoint1 = this.transform.position;
            oldPoint2 = this.transform.position;
            oldPoint3 = this.transform.position;

        }

        private void Update()
        {
            //GetAddValue
            if (Mathf.Sqrt(oldv1.magnitude) > MaxV)
            {
                TargetAddValue = 1f;
            }
            else
            {
                TargetAddValue = 0.01f + 0.99f * (Mathf.Sqrt(oldv1.magnitude) / MaxV);
            }

            //Update AddValue
            if (IsAdjustValueByV)
            {
                AddValue = TargetAddValue;
            }


            //If open
            if (IsOpen)
            {
                KaUpdata();
                kalman.SetKalman(Y, C);//update Kalman,Maybe don't need it
            }
            else
            {
                TargetPoint = this.transform.position;
            }

            //Update Target
            TargetObj.transform.position = TargetPoint;
        }



        //Get results
        public void KaUpdata()
        {
            //updata speed
            oldv1 = (oldPoint1 - oldPoint2) / Time.unscaledDeltaTime;
            oldv2 = (oldPoint2 - oldPoint3) / Time.unscaledDeltaTime;
            av = (oldv1 - oldv2) / Time.unscaledDeltaTime;

            Kalman = kalman.KalmanGo(this.transform.position, oldv1, av, Time.unscaledDeltaTime, oldPoint1);//kalman
            OneEuroFilter = AddValue * transform.position + (1 - AddValue) * TargetObj.transform.position;//OneEuro

            //if you need,you can use one of they
            if (IsUseKalman && IsUseOneEuroFilter)
            {
                TargetPoint = (1 - AddValue) * OneEuroFilter + (Kalman * AddValue);
            }
            else if (IsUseKalman)
            {
                TargetPoint = Kalman;
            }
            else if (IsUseOneEuroFilter)
            {
                TargetPoint = OneEuroFilter;
            }

            //updata oldpoints
            oldPoint3 = oldPoint2;
            oldPoint2 = oldPoint1;
            oldPoint1 = TargetPoint;
        }
    }

}
