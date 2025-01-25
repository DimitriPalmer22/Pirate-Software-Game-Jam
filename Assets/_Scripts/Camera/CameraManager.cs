using System;
using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set; }
    
    #region Serialized Fields

    [SerializeField] private Camera mainCam;
    [SerializeField] private CinemachineCamera virtualCam;
    
    #endregion
    
    #region Getters
    
    public Camera MainCam => mainCam;
    
    public CinemachineCamera VirtualCam => virtualCam;
    
    #endregion

    private void Awake()
    {
        // Set the instance
        Instance = this;
    }
}
