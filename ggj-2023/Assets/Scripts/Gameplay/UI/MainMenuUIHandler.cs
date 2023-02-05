using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenuUIHandler : UIPageBase
{
  [SerializeField]
  private Button _buttonNewGame = null;

  [SerializeField]
  private Button _buttonQuit = null;

  protected override void Awake()
  {
    base.Awake();
    _buttonNewGame.onClick.AddListener(OnNewGameClicked);
    _buttonQuit.onClick.AddListener(OnQuitGameClicked);
  }

  public void OnNewGameClicked()
  {
    GameStateManager.Instance.NewGame();
    Debug.Log("Button Click");
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
