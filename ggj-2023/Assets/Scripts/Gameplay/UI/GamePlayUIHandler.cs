using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamePlayUIHandler : UIPageBase
{
  public float LowWarningFraction = 0.1f;
  public SoundBank LowTimerWarning;
  public Image TimerDial;

  private float _previousRemainingFraction;

  protected override void Awake()
  {
    base.Awake();
    Shown += OnShown;
  }

  private void OnShown()
  {
    TimerDial.fillAmount= 1.0f;
    _previousRemainingFraction= 1.0f;
  }

  private void Update()
  {
    float gameTimer = GameStateManager.Instance.TimeInState;
    float gameDuration = GameStateManager.Instance.GameplayDuration;
    float newRemainingFraction = Mathf.Clamp01((gameDuration - gameTimer) / gameDuration);

    TimerDial.fillAmount = newRemainingFraction;

    if (_previousRemainingFraction >= LowWarningFraction && newRemainingFraction < LowWarningFraction)
    {
      if (LowTimerWarning != null)
      {
        AudioManager.Instance.PlaySound(gameObject, LowTimerWarning);
      }
    }
  }
}
