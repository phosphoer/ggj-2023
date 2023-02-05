using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KrakenController : MonoBehaviour
{
  public List<GameObject> KrakenArms = new List<GameObject>();
  public float CameraShakeDuration= 0.5f;
  public float CameraShakeMagnitude= 10.0f;

  [SerializeField]
  private SoundBank _krackenRoar = null;

  private float _activationInterval = 0f;
  private int _activatedArmCount = 0;
  private int _randOffset = 0;

  void Start()
  {
    _activationInterval = GameStateManager.Instance.GameplayDuration / (KrakenArms.Count + 1);
    _randOffset = Random.Range(0, KrakenArms.Count);
  }

  // Update is called once per frame
  void Update()
  {
    if (_activatedArmCount < KrakenArms.Count &&
        GameStateManager.Instance.CurrentStage == GameStateManager.GameStage.Gameplay &&
        GameStateManager.Instance.TimeInState >= ((_activatedArmCount + 1) * _activationInterval))
    {
      ActivateArm();
    }
  }

  public void ActivateArm()
  {
    int armIndex= (_activatedArmCount + _randOffset) % KrakenArms.Count;

    KrakenArms[armIndex].SetActive(true);
    _activatedArmCount++;

    if (_krackenRoar != null)
    {
      AudioManager.Instance.PlaySound(_krackenRoar);
    }

    CameraManager.Instance.ShakeActiveCameras(CameraShakeDuration, CameraShakeMagnitude);
  }
}
