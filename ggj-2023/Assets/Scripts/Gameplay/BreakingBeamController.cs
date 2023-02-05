using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakingBeamController : MonoBehaviour
{
  public Animator animator;

  [SerializeField]
  private AnimatorCallbacks _animatorCallbacks = null;

  public PirateController pirate;

  private void Start()
  {
    if (pirate != null)
      pirate.PirateFull+= OnPirateFull;
  }

  private void OnEnable()
  {
    _animatorCallbacks.AddCallback("OnFallStart", OnFallStart);
    _animatorCallbacks.AddCallback("OnFallEnd", OnFallEnd);
  }

  private void OnDisable()
  {
    _animatorCallbacks.RemoveCallback("OnFallStart", OnFallStart);
    _animatorCallbacks.RemoveCallback("OnFallEnd", OnFallEnd);
  }

  private void OnPirateFull(PirateController pirate)
  {
    animator.SetBool("IsBroken", true);
  }

  private void OnFallStart()
  {
    pirate.ActivatePhysics();
  }

  private void OnFallEnd()
  {

  }
}
