using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwoHandWeapon : MonoBehaviour
{

    public Transform LeftHandSource, RightHandSource;//左右手源,用于获得武器朝向


    //在滤波之后更新插值位置
    private void LateUpdate()
    {
        if (gameObject.activeSelf)
        {
            //枪朝向确定，双手武器基础
            Vector3 TwoHandForward = (LeftHandSource.position - RightHandSource.position).normalized;//取得方向向量
            transform.forward = Vector3.Lerp(transform.forward, TwoHandForward, 0.1f); //右手朝武器方向为武器方向,插值实现,0.1是因为Lerp的起点也在不断变化，顺便实现了插值
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, RightHandSource.eulerAngles.z);//修正Z轴。
        }
        
    }
}
