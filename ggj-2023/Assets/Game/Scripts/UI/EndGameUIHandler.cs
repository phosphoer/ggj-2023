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

  public SoundBank UIReveal;
  public SoundBank TieAudio;

  protected override void Awake()
  {
    base.Awake();
    Shown += OnShown;
  }

  private void OnShown()
  {
  }

  void Update()
  {

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
