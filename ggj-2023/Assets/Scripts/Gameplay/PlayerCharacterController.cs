using UnityEngine;
using Rewired;

public class PlayerCharacterController : MonoBehaviour
{
  public CameraControllerStack CameraStack => _cameraStack;
  public CameraControllerPlayer CameraController => _cameraController;
  public PlayerUI PlayerUI => _playerUI;

  public int RewiredPlayerId = 0;
  public GameCharacterController Character = null;
  public CameraControllerStack CameraStackPrefab = null;
  public CameraControllerPlayer CameraControllerPrefab = null;
  public PlayerUI PlayerUIPrefab = null;

  private InteractableUI _hudMessageUI;
  public Transform PlayerHudUIAnchor => _playerHudUIAnchor;
  public InteractableUI PlayerHudPrefab => _playerHudPrefab;
  public float PlayerHudHeight = 0;

  private CameraControllerStack _cameraStack;
  private CameraControllerPlayer _cameraController;
  private PlayerUI _playerUI;

  [SerializeField]
  private Transform _playerHudUIAnchor;

  private bool _isReady= true;
  private bool _isAllowedToMove= true;

  [SerializeField]
  private InteractableUI _playerHudPrefab = null;

  public event System.Action<PlayerCharacterController> PlayerReady;

  private void Awake()
  {
    _cameraStack = Instantiate(CameraStackPrefab);
    _cameraController = Instantiate(CameraControllerPrefab, Character.CameraRoot);
    _cameraController.transform.SetIdentityTransformLocal();

    CameraStack.PushController(_cameraController);
    CameraStack.SnapTransformToTarget();

    _playerUI = Instantiate(PlayerUIPrefab);
    _playerUI.Canvas.worldCamera = _cameraStack.Camera;

    Character.InteractionController.PlayerUI = _playerUI;
  }

  public bool GetIsReady()
  {
    return _isReady;
  }

  public void ClearReadyFlag()
  {
    _isReady= false;
  }

  public void SetReadyFlag()
  {
    if (!_isReady)
    {
      _isReady= true;
      PlayerReady?.Invoke(this);
    }
  }

  public void SetIsAllowedToMove(bool flag)
  {
    _isAllowedToMove= flag;
    Cursor.lockState = flag ? CursorLockMode.Locked : CursorLockMode.None;
  }

  private void Update()
  {
    var rewiredPlayer = ReInput.players.GetPlayer(RewiredPlayerId);
    if (rewiredPlayer != null)
    {
      if (!_isReady)
      {
        if (rewiredPlayer.GetButtonDown(RewiredConsts.Action.Interact))
        {
          SetReadyFlag();
        }
      }
      else if (_isAllowedToMove)
      {
        Character.MoveAxis = rewiredPlayer.GetAxis(RewiredConsts.Action.MoveForward);
        Character.StrafeAxis = rewiredPlayer.GetAxis(RewiredConsts.Action.Strafe);
        Character.LookHorizontalAxis = rewiredPlayer.GetAxis(RewiredConsts.Action.LookHorizontal);
        Character.LookVerticalAxis = rewiredPlayer.GetAxis(RewiredConsts.Action.LookVertical);

        if (rewiredPlayer.GetButtonDown(RewiredConsts.Action.Interact))
        {
          Character.Interact();
        }
      }
    }
  }

  public void ShowHudMessage(string message)
  {
    ClearHudMessage();

    var uiRoot = PlayerUI.OnScreenUI.ShowItem(PlayerHudUIAnchor, Vector3.up * PlayerHudHeight);
    _hudMessageUI = Instantiate(_playerHudPrefab, uiRoot);
    _hudMessageUI.transform.SetIdentityTransformLocal();
    _hudMessageUI.InteractionText = message;
  }

  public void ClearHudMessage()
  {
    if (PlayerUI != null)
    {
      if (_hudMessageUI != null)
        PlayerUI.OnScreenUI.HideItem(_hudMessageUI.transform.parent as RectTransform);

      _hudMessageUI = null;
    }
  }
}