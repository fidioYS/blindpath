using UnityEngine;

namespace VRNoShake
{
    public class KalmanRotation
    {
        float p = 1f;
        float Y = 0.3f;
        float C = 1f;
        public Quaternion X;
        Vector3 YX;
        float k;

        Vector3 Fix;

        Vector3 OldEuler;
        Quaternion OldQua;
        public Quaternion KalmanRotate(Transform NowX, Vector3 v, float dt, Transform OldX)
        {
            OldEuler = OldX.eulerAngles;
            OldQua = OldX.rotation;
            OldX.Rotate(v * dt, Space.Self);
            //fix
            Fix = FixEuler(OldX.eulerAngles, OldEuler, v);
            if (Fix.x > 0 && Fix.y > 0 && Fix.z > 0)
            {
                p = p + Y;
                k = p / (p + C);
                X = Quaternion.Lerp(OldX.rotation, NowX.rotation, k);
                p = (1 - k) * p;
                return X;
            }
            else
            {
                OldX.rotation = OldQua;//rotaback
                p = p + Y;
                k = p / (p + C);
                X = OldX.rotation;
                OldX.Rotate(new Vector3(Fix.x * v.x, Fix.y * v.y, Fix.z * v.z) * dt, Space.Self);
                X = Quaternion.Lerp(OldX.rotation, NowX.rotation, k);
                p = (1 - k) * p;
                return X;
            }


        }

        public void SetKalman(float YY, float CC)
        {
            Y = YY;
            C = CC;
        }


        //fix
        public Vector3 FixEuler(Vector3 AfterRota, Vector3 oldeuler, Vector3 v)
        {
            float x, y, z;
            if (AfterRota.x - oldeuler.x > 0 && v.x > 0)
            {
                x = 1;
            }
            else if (AfterRota.x - oldeuler.x < 0 && v.x < 0)
            {
                x = 1;
            }
            else
            {
                x = -1;
            }

            if (AfterRota.y - oldeuler.y > 0 && v.y > 0)
            {
                y = 1;
            }
            else if (AfterRota.y - oldeuler.y < 0 && v.y < 0)
            {
                y = 1;
            }
            else
            {
                y = -1;
            }

            if (AfterRota.z - oldeuler.z > 0 && v.z > 0)
            {
                z = 1;
            }
            else if (AfterRota.z - oldeuler.z < 0 && v.z < 0)
            {
                z = 1;
            }
            else
            {
                z = -1;
            }

            return new Vector3(x, y, z);
        }


        public Vector3 FixYX(Vector3 Now, Vector3 Yx)
        {
            float x = Yx.x, y = Yx.y, z = Yx.z;

            if (Mathf.Abs(Now.x - Yx.x) > 200)
            {
                if (Now.x > 200)
                {
                    x = 359.9f;
                }
                if (Now.x < 200)
                {
                    x = 0.1f;
                }
            }

            if (Mathf.Abs(Now.y - Yx.y) > 200)
            {
                if (Now.y > 200)
                {
                    y = 359.9f;
                }
                if (Now.y < 200)
                {
                    y = 0.1f;
                }
            }

            if (Mathf.Abs(Now.z - Yx.z) > 200)
            {
                if (Now.z > 200)
                {
                    z = 359.9f;
                }
                if (Now.z < 200)
                {
                    z = 0.1f;
                }
            }

            return new Vector3(x, y, z);

        }
    }
}

