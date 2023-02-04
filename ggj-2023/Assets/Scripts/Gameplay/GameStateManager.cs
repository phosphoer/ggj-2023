using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : Singleton<GameStateManager>
{
  public enum GameStage
  {
    Invalid,
    MainMenu,
    WaitingForPlayers,
    WaitingForReady,
    Gameplay,
    EndGame
  }

  public static event System.Action GameStateChangeEvent;

  private GameStage _gameStage = GameStage.Invalid;
  public GameStage CurrentStage => _gameStage;
  public GameStage EditorDefaultStage = GameStage.MainMenu;

  public SoundBank MusicMenuLoop;
  public CameraControllerBase MenuCamera;

  private List<PlayerCharacterController> _players = new List<PlayerCharacterController>();

  private void Awake()
  {
    Instance = this;
    PlayerManager.PlayerJoined += OnPlayerJoined;
  }

  private void OnDestroy()
  {
    PlayerManager.PlayerJoined -= OnPlayerJoined;
  }

  // Start is called before the first frame update
  private void Start()
  {
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
    case GameStage.WaitingForPlayers:
      break;
    case GameStage.WaitingForReady:
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
    SetGameStage(GameStage.WaitingForPlayers);
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

      GameUI.Instance.MainMenuUI.Hide();
    }
    break;

    case GameStage.WaitingForPlayers:
      {
        GameUI.Instance.WaitingForPlayersUI.Hide();
      }
      break;

    case GameStage.WaitingForReady:
    {
      foreach (PlayerCharacterController player in _players)
      {
        player.ClearHudMessage();
        player.SetIsAllowedToMove(true);
      }
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

      // Not allowed to spawn players in the main menu
      PlayerManager.Instance.SetCanSpawnPlayers(false);

      GameUI.Instance.MainMenuUI.Show();

      if (MusicMenuLoop != null)
      {
        AudioManager.Instance.FadeInSound(gameObject, MusicMenuLoop, 3.0f);
      }

      ResetGameStats();
    }
    break;

    case GameStage.WaitingForPlayers:
      {
        // Now we can spawn players
        PlayerManager.Instance.SetCanSpawnPlayers(true);

        // Tell users that we are waiting for players to join
        GameUI.Instance.WaitingForPlayersUI.Show();
      }
      break;

    case GameStage.WaitingForReady:
      {
        // Switch to multi camera mode now that all the players are locked in
        CameraManager.Instance.SetScreenLayout(CameraManager.eScreenLayout.MultiCamera);
      }
      break;

    case GameStage.Gameplay:
    {
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
    }
    break;
    }
  }

  void ResetGameStats()
  {
    //ScenarioManager.Instance.ResetGameStats();
  }

  public PlayerCharacterController GetPlayerController(int playerId)
  {
    if (playerId >= 0 && playerId < _players.Count)
      return _players[playerId];
    else
      return null;
  }

  public GameObject GetPlayerGameObject(int playerId)
  {
    PlayerCharacterController playerController = GetPlayerController(playerId);
    return (playerController != null) ? playerController.gameObject : null;
  }

  private void OnPlayerJoined(PlayerCharacterController player)
  {
    if (_players.Count < 4)
    {
      bool isFirstPlayer = _players.Count == 0;

      // Track all the players that joined
      _players.Add(player);

      // Add the camera for the new player to the split screen layout
      CameraManager.Instance.SplitscreenLayout.AddCamera(player.CameraStack.Camera);

      // Lock player in place until the start of the game
      player.SetIsAllowedToMove(false);

      // Wait for player to acknowledge they are ready
      player.ClearReadyFlag();
      player.ShowHudMessage("Ready?");
      player.PlayerReady += OnPlayerReady;

      // Move onto waiting-for-ready stage now that we have at least one player camera to use
      if (isFirstPlayer)
        SetGameStage(GameStage.WaitingForReady);
    }
  }

  private void OnPlayerReady(PlayerCharacterController readyPlayer)
  {
    readyPlayer.ShowHudMessage("Ready!");
    readyPlayer.PlayerReady -= OnPlayerReady;

    int readyCount = 0;
    foreach (PlayerCharacterController player in _players)
    {
      if (player.GetIsReady())
      {
        readyCount++;
      }
    }

    if (readyCount >= _players.Count)
    {
      SetGameStage(GameStage.Gameplay);
    }
  }
}