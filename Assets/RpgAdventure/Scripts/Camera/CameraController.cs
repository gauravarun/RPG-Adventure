using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

namespace RpgAdventure
{
    public class CameraController : MonoBehaviour
{
        [SerializeField]
        CinemachineFreeLook freelookCamera;

        public CinemachineFreeLook PlayerCam
        {
            get 
            {
                return freelookCamera;
            }
        }
    
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            freelookCamera.m_XAxis.m_MaxSpeed = 400;
            freelookCamera.m_YAxis.m_MaxSpeed = 10;
        }

        if (Input.GetMouseButtonUp(1))
        {
            freelookCamera.m_XAxis.m_MaxSpeed = 0;
            freelookCamera.m_YAxis.m_MaxSpeed = 0;
        }
    }
}
}


