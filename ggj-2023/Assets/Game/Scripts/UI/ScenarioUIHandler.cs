using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScenarioUIHandler : UIPageBase
{
  [SerializeField]
  private Text _gameplayTextField = null;

  public float _timer;

  public bool IsComplete()
  {
    return _timer <= 0;
  }

  protected override void Awake()
  {
    base.Awake();
    Shown += OnShown;
  }

  private void OnShown()
  {
    Canvas parentCanvas = this.transform.parent.GetComponent<Canvas>();
    parentCanvas.renderMode = RenderMode.ScreenSpaceCamera;
    parentCanvas.worldCamera = CameraManager.Instance.ScenarioCamera;
    parentCanvas.planeDistance = 1.0f;

    if (_gameplayTextField != null)
    {
      Scenario scenario = ScenarioManager.Instance.GetCurrentScenario();

      _gameplayTextField.text = scenario.GameplayText;
      _timer = scenario.GameplayDuration;
    }
  }

  void Update()
  {
    _timer -= Time.deltaTime;
    if (_timer <= 0)
    {
      Hide();
    }
  }
}
