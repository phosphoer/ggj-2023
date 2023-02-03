using UnityEngine;
using Rewired;

public class PlayerCharacterController : MonoBehaviour
{
  public CameraControllerPlayer PlayerCamera => _playerCamera;

  public int RewiredPlayerId = 0;
  public GameCharacterController Character = null;
  public CameraControllerStack CameraStack = null;
  public CameraControllerPlayer PlayerCameraPrefab = null;

  private CameraControllerPlayer _playerCamera;

  private void Update()
  {
    if (CameraStack.Camera != null && _playerCamera == null)
    {
      _playerCamera = Instantiate(PlayerCameraPrefab);

      CameraStack.PushController(_playerCamera);
      CameraStack.SnapTransformToTarget();
    }

    var rewiredPlayer = ReInput.players.GetPlayer(RewiredPlayerId);
    if (rewiredPlayer != null)
    {
      Character.DesiredSpeed = rewiredPlayer.GetAxis(RewiredConsts.Action.MoveForward);
      Character.DesiredTurn = rewiredPlayer.GetAxis(RewiredConsts.Action.Strafe);
    }
  }
}