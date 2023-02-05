using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndGameUIHandler : UIPageBase
{
  [SerializeField]
  private Button _buttonNewGame = null;

  [SerializeField]
  private Button _buttonQuit = null;

  protected override void Awake()
  {
    base.Awake();
    _buttonNewGame.onClick.AddListener(OnPlayAgainClicked);
    _buttonQuit.onClick.AddListener(OnQuitGameClicked);
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
