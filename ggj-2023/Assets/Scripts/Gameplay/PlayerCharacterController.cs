using UnityEngine;
// using Rewired;

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

    // var rewiredPlayer = ReInput.players.GetPlayer(RewiredPlayerId);
    // if (rewiredPlayer != null)
    // {
    //   Character.DesiredSpeed = rewiredPlayer.GetAxis(RewiredConsts.Action.Move);
    //   Character.DesiredTurn = rewiredPlayer.GetAxis(RewiredConsts.Action.Turn);

    //   if (rewiredPlayer.GetButtonDown(RewiredConsts.Action.SelfDestruct))
    //   {
    //     PlayerManager.Instance.RespawnPlayer(this);
    //     _playerCamera.CameraStart();
    //     CameraStack.SnapTransformToTarget();
    //   }
    //   if (rewiredPlayer.GetButtonDoublePressDown(RewiredConsts.Action.Slap))
    //   {
    //     Character.DoubleSlap();
    //   }
    //   else if (rewiredPlayer.GetButtonSinglePressDown(RewiredConsts.Action.Slap))
    //   {
    //     Character.FastSlap();
    //   }
    // }
  }
}