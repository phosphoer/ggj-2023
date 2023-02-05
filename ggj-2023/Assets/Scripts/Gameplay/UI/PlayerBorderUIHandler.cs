using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerBorderUIHandler : MonoBehaviour
{
  public Image[] Edges;
  public Color[] PlayerColors;

  public void BindPlayerID(int playerID)
  {
    int colorIndex = Mathf.Min(playerID, PlayerColors.Length -1);

    foreach(Image edge in Edges)
    {
      edge.color = PlayerColors[colorIndex];
    }
  }
}
