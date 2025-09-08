using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.UIElements;

public class CameraFacing : MonoBehaviour
{
    Transform _cameraTransform;
    void Start()
    {
        Init();
    }

    private void Init()
    {
        _cameraTransform = Camera.main.transform;
    }
    void LateUpdate()
    {
        RotateTranform();
    }
    void RotateTranform()
    {
       transform.LookAt(transform.position +_cameraTransform.rotation * Vector3.forward,
         _cameraTransform.rotation * Vector3.up);
    }
}
