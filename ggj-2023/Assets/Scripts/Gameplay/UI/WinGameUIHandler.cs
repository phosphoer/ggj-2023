using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinGameUIHandler : UIPageBase
{
  public TMPro.TMP_Text WinLabel;

  protected override void Awake()
  {
    base.Awake();
    Shown += OnShown;    
  }

  private void OnShown()
  {
    WinLabel.text = string.Format("Player {0} had the chonkiest pirate!", GameStateManager.Instance.WinningPlayerID);
  }
}
