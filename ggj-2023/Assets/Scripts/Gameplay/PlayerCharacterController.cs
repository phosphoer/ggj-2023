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

  private CameraControllerStack _cameraStack;
  private CameraControllerPlayer _cameraController;
  private PlayerUI _playerUI;

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

    Cursor.lockState = CursorLockMode.Locked;
  }

  private void Update()
  {
    var rewiredPlayer = ReInput.players.GetPlayer(RewiredPlayerId);
    if (rewiredPlayer != null)
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