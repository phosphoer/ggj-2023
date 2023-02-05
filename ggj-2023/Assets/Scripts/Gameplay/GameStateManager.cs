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
    PlayerWinCutscene,
    PlayerLoseCutscene,
    EndGame
  }

  public static event System.Action GameStateChangeEvent;

  private GameStage _gameStage = GameStage.Invalid;
  public GameStage CurrentStage => _gameStage;
  public GameStage EditorDefaultStage = GameStage.MainMenu;

  private float _timeInStage = 0;
  public float TimeInState => _timeInStage;

  public float GameplayDuration = 300.0f;
  public float EndCutSceneDuration = 5.0f;

  private int _winningPlayerID = -1;
  public int WinningPlayerID => _winningPlayerID;

  public SoundBank MusicMenuLoop;
  public CameraControllerBase MenuCamera;

  private List<PlayerCharacterController> _players = new List<PlayerCharacterController>();

  private void Awake()
  {
    Instance = this;
    PlayerManager.PlayerJoined += OnPlayerJoined;
    PlayerManager.PlayerWon += OnPlayerWon;
  }

  private void OnDestroy()
  {
    PlayerManager.PlayerJoined -= OnPlayerJoined;
    PlayerManager.PlayerWon -= OnPlayerWon;
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
        if (_timeInStage >= GameplayDuration)
        {
          nextGameStage = GameStage.PlayerLoseCutscene;
        }
        break;
      case GameStage.PlayerWinCutscene:
      case GameStage.PlayerLoseCutscene:
        if (_timeInStage >= EndCutSceneDuration)
        {
          nextGameStage = GameStage.EndGame;
        }
        break;
      case GameStage.EndGame:
        break;
    }
    _timeInStage += Time.deltaTime;

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
          GameUI.Instance.GameplayUI.Hide();
        }
        break;

      case GameStage.PlayerWinCutscene:
        {
          GameUI.Instance.WinGameUI.Hide();
        }
        break;

      case GameStage.PlayerLoseCutscene:
        {
          GameUI.Instance.LoseGameUI.Hide();
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

    _timeInStage = 0.0f;

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
		  // Get rid of any pirate that wasn't assigned to a player
		  PlayerManager.Instance.DeactivateUnassignedPirates();

		  // Show the game timer UI
		  GameUI.Instance.GameplayUI.Show();
		}
		break;

      case GameStage.PlayerWinCutscene:
        {
          CameraManager.Instance.SetScreenLayout(CameraManager.eScreenLayout.WinCamera);
          PlayerManager.Instance.LockAllPlayers();
          GameUI.Instance.WinGameUI.Show();
        }
        break;

      case GameStage.PlayerLoseCutscene:
        {
          CameraManager.Instance.SetScreenLayout(CameraManager.eScreenLayout.LoseCamera);
          PlayerManager.Instance.LockAllPlayers();
          GameUI.Instance.LoseGameUI.Show();
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

      // If the player joined pre-game, take them thru the ready flow
      if (_gameStage < GameStage.Gameplay)
      {
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

  private void OnPlayerWon(PlayerCharacterController readyPlayer)
  {
    _winningPlayerID = readyPlayer.PlayerID;
    SetGameStage(GameStage.PlayerWinCutscene);
  }
}