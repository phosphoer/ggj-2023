using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
  public Canvas Canvas;
  public WorldAttachedUI OnScreenUI;

  private void Awake()
  {
    Canvas.planeDistance = 1;
  }
}
