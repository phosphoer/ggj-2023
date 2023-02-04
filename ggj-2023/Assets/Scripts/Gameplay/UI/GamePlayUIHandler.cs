using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlayUIHandler : UIPageBase
{
  public float LowWarningFraction = 0.1f;
  public SoundBank LowTimerWarning;
  public RectTransform TimerTransform;

  private float _previousRemainingFraction;

  protected override void Awake()
  {
    base.Awake();
    Shown += OnShown;
  }

  private void OnShown()
  {
    TimerTransform.transform.localScale = new Vector3(1, 1, 1);
    _previousRemainingFraction= 1.0f;
  }

  private void Update()
  {
    float gameTimer = GameStateManager.Instance.TimeInState;
    float gameDuration = GameStateManager.Instance.GameplayDuration;
    float newRemainingFraction = Mathf.Clamp01((gameDuration - gameTimer) / gameDuration);

    TimerTransform.transform.localScale = new Vector3(newRemainingFraction, 1, 1);

    if (_previousRemainingFraction >= LowWarningFraction && newRemainingFraction < LowWarningFraction)
    {
      if (LowTimerWarning != null)
      {
        AudioManager.Instance.PlaySound(gameObject, LowTimerWarning);
      }
    }
  }
}
