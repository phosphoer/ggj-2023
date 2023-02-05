using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakingBeamController : MonoBehaviour
{
  public Animator animator;
  public PirateController pirate;

  private void Start()
  {
    if (pirate != null)
      pirate.PirateFull+= OnPirateFull;
  }

  private void OnPirateFull(PirateController pirate)
  {
    animator.SetBool("IsBroken", true);
  }
}
