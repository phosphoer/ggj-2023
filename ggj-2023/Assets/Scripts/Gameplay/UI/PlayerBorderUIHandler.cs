using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerBorderUIHandler : MonoBehaviour
{
  public Image[] Edges;
  public Color[] PlayerColors;

  public void BindPlayerController(PlayerCharacterController player)
  {
    int colorIndex = Mathf.Min(player.PlayerID, PlayerColors.Length -1);

    foreach(Image edge in Edges)
    {
      edge.color = PlayerColors[colorIndex];
    }
  }
}
