using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public List<CinemachineVirtualCamera> cameras;

    public CinemachineVirtualCamera CurrentCamera => cameras[cameraIndex];

    private int cameraIndex;

    private void Start()
    {
        EnableCurrent();
    }

    public void NextCamera(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            cameraIndex++;
            cameraIndex %= cameras.Count;
    
            EnableCurrent();
        }
    }

    public void PreviousCamera(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            cameraIndex--;
            if (cameraIndex < 0)
                cameraIndex = cameras.Count - 1;

            EnableCurrent();
        }
    }

    private void EnableCurrent()
    {
        foreach (var camera in cameras)
        {
            camera.gameObject.SetActive(false);
        }

        CurrentCamera.gameObject.SetActive(true);
    }
}