using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScenarioOutroUIHandler : UIPageBase
{
    [SerializeField]
    private Text _titleTextField = null;

    [SerializeField]
    private Text _resultTextField = null;

    private float _mainResultTimer;
    private float _bonusTimer;
    private string _bonusText;

    public bool IsComplete()
    {
        return _mainResultTimer <= 0 && _bonusTimer <= 0;
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

        if (_titleTextField != null)
        {
            ScenarioManager scenarioMgr= ScenarioManager.Instance;
            Scenario scenario = scenarioMgr.GetCurrentScenario();

            _bonusTimer = 0;
            _bonusText = "";
            _mainResultTimer = scenario.OutroDuration;

            if (scenarioMgr.LastScenarioWinner == ePlayer.AngelPlayer)
            {
                _titleTextField.text = "Angel Wins Scenario!";
                _resultTextField.text = scenario.angelGoals.outroText;

                if (scenarioMgr.AngelStats.HasCompletedBonus())
                {
                    _bonusTimer = scenario.OutroDuration;
                    _bonusText = scenario.angelGoals.bonusOutroText;
                }
            }
            else 
            {
                _titleTextField.text = "Devil Wins Scenario!";
                _resultTextField.text = scenario.devilGoals.outroText;

                if (scenarioMgr.DevilStats.HasCompletedBonus())
                {
                    _bonusTimer = scenario.OutroDuration;
                    _bonusText = scenario.devilGoals.bonusOutroText;
                }
            }
        }
    }

    void Update()
    {
        if (_mainResultTimer > 0)
        {
            _mainResultTimer -= Time.deltaTime;

            // If _mainResultTimer timer elapses and we have bonus text, show that now
            if (_mainResultTimer <= 0 && _bonusTimer > 0)
            {
                _resultTextField.text = _bonusText;
            }
        }
        else if (_bonusTimer > 0)
        {
            _bonusTimer -= Time.deltaTime;
        }
    }
}