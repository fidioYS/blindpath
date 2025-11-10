using UnityEngine;


namespace VRNoShake
{ 
    public class Kalman
    {
        float p = 1f;
        float Y = 0.3f;
        float C = 1f;
        public Vector3 X;
        Vector3 YX;
        float k;

        public Vector3 KalmanGo(Vector3 NowX, Vector3 v, Vector3 av, float dt, Vector3 OldX)
        {
            YX = OldX + 0.1f * (v * dt + 0.5f * av * dt * dt);
            p = p + Y;
            k = p / (p + C);
            X = (1 - k) * YX + k * NowX;
            p = (1 - k) * p;
            return X;
        }

        public void SetKalman(float YY, float CC)
        {
            Y = YY;
            C = CC;
        }


    }
}

