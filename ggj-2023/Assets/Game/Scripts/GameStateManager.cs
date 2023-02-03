using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : Singleton<GameStateManager>
{
  public enum GameStage
  {
    Invalid,
    MainMenu,
    Gameplay,
    EndGame
  }

  public static event System.Action GameStateChangeEvent;

  private GameStage _gameStage = GameStage.Invalid;
  public GameStage CurrentStage => _gameStage;
  public GameStage EditorDefaultStage = GameStage.MainMenu;

  public SoundBank MusicMenuLoop;
  public CameraControllerBase MenuCamera;

  //private List<PlayerCharacterController> _players;

  private void Awake()
  {
    Instance = this;
    //PlayerManager.PlayerJoined += OnPlayerJoined;
  }

  private void OnDestroy()
  {
    //PlayerManager.PlayerJoined -= OnPlayerJoined;
  }

  // Start is called before the first frame update
  private void Start()
  {
    // Base camera controller
    //CameraControllerStack.Instance.PushController(MenuCamera);

    GameStage InitialStage = GameStage.MainMenu;
#if UNITY_EDITOR
    InitialStage = EditorDefaultStage;
#endif

    SetGameStage(InitialStage);
  }

  // Update is called once per frame
  private void Update()
  {
    GameStage nextGameStage = _gameStage;

    switch (_gameStage)
    {
      case GameStage.MainMenu:
        break;
      case GameStage.Gameplay:
        break;
      case GameStage.EndGame:
        break;
    }

    SetGameStage(nextGameStage);
  }

  public void NewGame()
  {
    ResetGameStats();
    SetGameStage(GameStage.Gameplay);
  }

  public void SetGameStage(GameStage newGameStage)
  {
    if (newGameStage != _gameStage)
    {
      OnExitStage(_gameStage, newGameStage);
      OnEnterStage(newGameStage);
      _gameStage = newGameStage;
    }
  }

  public void OnExitStage(GameStage oldGameStage, GameStage newGameStage)
  {
    switch (oldGameStage)
    {
      case GameStage.MainMenu:
        {
          if (MusicMenuLoop != null)
          {
            AudioManager.Instance.FadeOutSound(gameObject, MusicMenuLoop, 3f);
          }

          CameraManager.Instance.MenuCameraStack.PopCurrentController();

          GameUI.Instance.MainMenuUI.Hide();
        }
        break;

      case GameStage.Gameplay:
        {
          // GameUI.Instance.ScenarioUI.Hide();
          // GameUI.Instance.AngelUI.Hide();
          // GameUI.Instance.DevilUI.Hide();
          // GameUI.Instance.SidebarUI.Hide();
        }
        break;
      case GameStage.EndGame:
        {
          GameUI.Instance.EndGameUI.Hide();
        }
        break;
    }
  }

  public void OnEnterStage(GameStage newGameStage)
  {
    GameStateChangeEvent?.Invoke();

    switch (newGameStage)
    {
      case GameStage.MainMenu:
        {
          CameraManager.Instance.SetScreenLayout(CameraManager.eScreenLayout.MenuCamera);

          GameUI.Instance.MainMenuUI.Show();

          if (MusicMenuLoop != null)
          {
            AudioManager.Instance.FadeInSound(gameObject, MusicMenuLoop, 3.0f);
          }

          ResetGameStats();
        }
        break;
      case GameStage.Gameplay:
        {
          CameraManager.Instance.SetScreenLayout(CameraManager.eScreenLayout.MultiCamera);
          // CameraManager.Instance.ScenarioCameraStack.SwitchController(ScenarioManager.Instance.CurrentScene.CurrentCamera);

          // GameUI.Instance.AngelUI.AssignPlayer(ePlayer.AngelPlayer);
          // GameUI.Instance.AngelUI.Show();

          // GameUI.Instance.DevilUI.AssignPlayer(ePlayer.DevilPlayer);
          // GameUI.Instance.DevilUI.Show();

          //GameUI.Instance.SidebarUI.Show();
        }
        break;
      case GameStage.EndGame:
        {
          CameraManager.Instance.SetScreenLayout(CameraManager.eScreenLayout.MenuCamera);
          GameUI.Instance.EndGameUI.Show();
          //CameraControllerStack.Instance.PushController(MenuCamera);
        }
        break;
    }
  }

  void ResetGameStats()
  {
    //ScenarioManager.Instance.ResetGameStats();
  }

  //public PlayerCharacterController GetPlayerGameObject(int playerId)
  //{
  //  return null;
  //}

  //private void OnPlayerJoined(PlayerCharacterController player)
  //{
  //  if (PlayerManager.Instance.Players.Count == 1)
  //  {
  //    player.CameraStack.Camera = CameraManager.Instance.LeftPlayerCamera;
  //    player.Team = ePlayer.DevilPlayer;

  //    if (DevilProps != null)
  //    {
  //      Instantiate(DevilProps, player.gameObject.transform, false);
  //    }

  //    _devilPlayer = player;
  //  }
  //}

  private void SpawnPlayers()
  {
    //PlayerManager.Instance.enabled = true;
  }

  private void DespawnPlayers()
  {
    //PlayerManager.Instance.enabled = false;
    //PlayerManager.Instance.RespawnAllPlayers();
  }

  //public PlayerCharacterController GetPlayer(int playerId)
  //{
  //  if (playerId >= 0 && playerId < _players.Length)
  //    return _players[playerId];
  //  else
  //    return null;
  //}
}