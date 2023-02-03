using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class SplitscreenLayout
{
  private List<Camera> activeCameras = new List<Camera>();

  private static Rect[] gridLayout4 = new Rect[4]
  {
    new Rect(0.0f, 0.5f, 0.5f, 0.5f),
    new Rect(0.5f, 0.5f, 0.5f, 0.5f),
    new Rect(0.5f, 0.0f, 0.5f, 0.5f),
    new Rect(0.0f, 0.0f, 0.5f, 0.5f),
  };
  private static Rect[] gridLayout3 = new Rect[3]
  {
    new Rect(0, 0, 0.5f, 1.0f),
    new Rect(0.5f, 0.5f, 0.5f, 0.5f),
    new Rect(0.5f, 0.0f, 0.5f, 0.5f),
  };

  private static Rect[] gridLayout2 = new Rect[2]
  {
    new Rect(0.0f, 0.0f, 0.5f, 1.0f),
    new Rect(0.5f, 0.0f, 0.5f, 1.0f),
  };

  private static Rect[] gridLayout1 = new Rect[1]
  {
    new Rect(0.0f, 0.0f, 1.0f, 1.0f),
  };

  public void AddCamera(Camera cam)
  {
    activeCameras.Add(cam);
    UpdateViewports();
  }

  public void RemoveCamera(Camera cam)
  {
    activeCameras.Remove(cam);
    UpdateViewports();
  }

  public void SetEnabled(bool bEnable)
  {
    for (Camera camera in activeCameras)
    {
      camera.enabled= bEnable;
    }
  }

  private void UpdateViewports()
  {
    Rect[] gridLayout = gridLayout4;
    if (activeCameras.Count == 3)
      gridLayout = gridLayout3;
    else if (activeCameras.Count == 2)
      gridLayout = gridLayout2;
    else if (activeCameras.Count == 1)
      gridLayout = gridLayout1;

    for (int i = 0; i < activeCameras.Count; ++i)
    {
      activeCameras[i].rect = gridLayout[i];
    }
  }
}