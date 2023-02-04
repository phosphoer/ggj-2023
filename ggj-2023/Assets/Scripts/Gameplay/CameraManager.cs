using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : Singleton<CameraManager>
{
  public enum eScreenLayout
  {
    Invalid,
    MenuCamera,
    MultiCamera,
    WinCamera,
    LoseCamera
  }

  public CameraControllerStack MenuCameraStack => _menuCameraStack;
  public SplitscreenLayout SplitscreenLayout => _splitscreenLayout;

  [SerializeField]
  private CameraControllerStack _menuCameraStack = null;

  [SerializeField]
  private Camera _menuCamera = null;

  [SerializeField]
  private SplitscreenLayout _splitscreenLayout = null;

  [SerializeField]
  private Camera[] _winCameras = null;

  [SerializeField]
  private Camera _loseCamera = null;  

  private eScreenLayout _cameraLayout = eScreenLayout.Invalid;
  public eScreenLayout CameraLayout => _cameraLayout;

  private void Awake()
  {
    Instance = this;
  }

  public void SetScreenLayout(eScreenLayout targetLayout)
  {
    if (targetLayout != _cameraLayout)
    {
      // Disable all cameras first
      _menuCamera.enabled = false;
      _splitscreenLayout.SetEnabled(false);
      _loseCamera.enabled = false;
      foreach (Camera camera in _winCameras)
      {
        camera.enabled = false;
      }

      // Then enable the one we want
      switch (targetLayout)
      {
        case eScreenLayout.MenuCamera:
          _menuCamera.enabled = true;
          break;
        case eScreenLayout.MultiCamera:
          _splitscreenLayout.SetEnabled(true);
          break;
        case eScreenLayout.WinCamera:
          { 
            // Show the corresponding winners camera
            int winnerIndex= GameStateManager.Instance.WinningPlayerID;
            if (_winCameras.IsIndexValid(winnerIndex))
            {
              _winCameras[winnerIndex].enabled = true;
            }
          }
          break;
        case eScreenLayout.LoseCamera:
          _loseCamera.enabled = true;        
          break;
      }

      _cameraLayout = targetLayout;
    }
  }
}
