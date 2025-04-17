using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class AddConfiner : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera;
    public Collider confinerCollider;  

    void Start()
    {
        var confiner = virtualCamera.gameObject.AddComponent<CinemachineConfiner>();
        confiner.m_BoundingVolume = confinerCollider;
        confiner.m_Damping = 0.5f;  
    }
}
