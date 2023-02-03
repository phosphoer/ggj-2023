using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : Singleton<CameraManager>
{
  public enum eScreenLayout
  {
    Invalid,
    MenuCamera,
    MultiCamera
  }

  public CameraControllerStack MenuCameraStack => _menuCameraStack;

  [SerializeField]
  private CameraControllerStack _menuCameraStack = null;

  [SerializeField]
  private SplitscreenLayout _splitscreenLayout = new SplitscreenLayout();

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
      switch (targetLayout)
      {
        case eScreenLayout.MenuCamera:
          _menuCamera.enabled = true;
          _splitscreenLayout.SetEnabled(false);
          break;
        case eScreenLayout.MultiCamera:
          _menuCamera.enabled = false;
          _splitscreenLayout.SetEnabled(true);
          break;
      }

      _cameraLayout = targetLayout;
    }
  }
}
