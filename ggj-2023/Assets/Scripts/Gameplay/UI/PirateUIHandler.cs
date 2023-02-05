using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PirateUIHandler : MonoBehaviour
{
  public GameObject[] fishIcons;
  private int _swallowCounter= 0;

  public void BindPirate(PirateController pirateController)
  {
    PlayerCharacterController player= pirateController.AssignedPlayerController;
    Color[] playerColors= player.PlayerUI.BorderUI.PlayerColors;
    Color playerColor= playerColors.IsIndexValid(player.PlayerID) ? playerColors[player.PlayerID] : Color.white;

    foreach (GameObject icon in fishIcons)
    {
      icon.GetComponent<Image>().color = playerColor;
    }

    pirateController.PirateSwallowed+= OnPirateSwallowed;
  }

  public void OnPirateSwallowed(PirateController pirate)
  {
    if (_swallowCounter >= 0 && _swallowCounter < fishIcons.Length)
    {
      fishIcons[_swallowCounter].SetActive(true);
    }
    
    _swallowCounter++;
  }

  void Start()
  { 
    foreach (GameObject icon in fishIcons)
    {
      icon.SetActive(false);
    }
  }
}
