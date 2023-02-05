using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PirateUIHandler : MonoBehaviour
{
  public GameObject[] fishIcons;

  public void BindPirate(PirateController pirateController)
  {
    pirateController.PirateSwallowed+= OnPirateSwallowed;

    int playerIndex= pirateController.AssignedPlayerController.PlayerID;
  }

  public void OnPirateSwallowed(PirateController pirate)
  {
    int fishCount= Mathf.RoundToInt(pirate.FoodWeight);
    for (int iconIndex = 0; iconIndex < fishIcons.Length; iconIndex++)
    {
      fishIcons[iconIndex].SetActive(iconIndex < fishCount);
    }
  }

  void Start()
  {
    foreach (GameObject icon in fishIcons)
    {
      icon.SetActive(false);
    }
  }
}
