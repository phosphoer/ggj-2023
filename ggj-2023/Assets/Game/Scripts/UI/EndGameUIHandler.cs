using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndGameUIHandler : UIPageBase
{
  public List<Text> ScenarioWinnerLabels;
  public Text FinalResultLabel;
  public List<Button> Buttons;
  public Color DevilUIColor;
  public Color AngelUIColor;

  public SoundBank UIReveal;
  public SoundBank AngelWinAudio;
  public SoundBank DevilWinAudio;
  public SoundBank TieAudio;

  public float RevealDelay = 0.5f;
  private int _revealCounter;
  private float _revealTimer;

  private int _devilWins = 0;
  private int _angelWins = 0;

  protected override void Awake()
  {
    base.Awake();
    Shown += OnShown;
  }

  private void OnShown()
  {
    List<ePlayer> scenarioWinners = ScenarioManager.Instance.ScenarioWinners;

    _devilWins = 0;
    _angelWins = 0;
    for (int LabelIndex = 0; LabelIndex < ScenarioWinnerLabels.Count; ++LabelIndex)
    {
      ePlayer player = LabelIndex < scenarioWinners.Count ? scenarioWinners[LabelIndex] : ePlayer.Invalid;
      Text label = ScenarioWinnerLabels[LabelIndex];

      switch (player)
      {
        case ePlayer.AngelPlayer:
          label.text = "Angel";
          label.color = AngelUIColor;
          ++_angelWins;
          break;
        case ePlayer.DevilPlayer:
          label.text = "Devil";
          label.color = DevilUIColor;
          ++_devilWins;
          break;
        case ePlayer.Invalid:
          label.text = "Invalid";
          break;
      }

      if (_angelWins > _devilWins)
      {
        FinalResultLabel.text = "Final Result: Angel Wins";
        FinalResultLabel.color = AngelUIColor;
      }
      else if (_devilWins > _angelWins)
      {
        FinalResultLabel.text = "Final Result: Devil Wins";
        FinalResultLabel.color = DevilUIColor;
      }
      else
      {
        FinalResultLabel.text = "Final Result: Draw";
      }
    }

    _revealTimer = 1.0f;
    Buttons[0].Select();
  }

  void Update()
  {
    if (_revealCounter < ScenarioWinnerLabels.Count + 1)
    {
      _revealTimer -= Time.deltaTime;

      if (_revealTimer <= 0.0f)
      {
        if (_revealCounter < ScenarioWinnerLabels.Count)
        {
          // Pop in the next winner label
          Text label = ScenarioWinnerLabels[_revealCounter];
          label.gameObject.GetComponent<UIHydrate>().Hydrate();

          if (TieAudio != null)
          {
            AudioManager.Instance.PlaySound(UIReveal);
          }
        }
        else
        {
          // Pop in remaining UI elements
          foreach (Button button in Buttons)
          {
            button.gameObject.GetComponent<UIHydrate>().Hydrate();
          }
          FinalResultLabel.gameObject.GetComponent<UIHydrate>().Hydrate();

          if (_angelWins > _devilWins)
          {
            if (AngelWinAudio != null)
            {
              AudioManager.Instance.PlaySound(AngelWinAudio);
            }
          }
          else if (_devilWins > _angelWins)
          {
            if (DevilWinAudio != null)
            {
              AudioManager.Instance.PlaySound(DevilWinAudio);
            }
          }
          else
          {
            if (TieAudio != null)
            {
              AudioManager.Instance.PlaySound(TieAudio);
            }
          }
        }

        ++_revealCounter;
        _revealTimer = RevealDelay;
      }
    }
  }

  public void OnPlayAgainClicked()
  {
    SceneManager.LoadScene("MainScene");
  }

  public void OnQuitGameClicked()
  {
    //If we are running in a standalone build of the game
#if UNITY_STANDALONE
    //Quit the application
    Application.Quit();
#endif

    //If we are running in the editor
#if UNITY_EDITOR
    //Stop playing the scene
    UnityEditor.EditorApplication.isPlaying = false;
#endif
  }
}
