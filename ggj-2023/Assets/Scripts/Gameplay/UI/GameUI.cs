using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUI : Singleton<GameUI>
{
  public MainMenuUIHandler MainMenuUI;
  public UIPageBase WaitingForPlayersUI;
  public WinGameUIHandler WinGameUI;
  public LoseGameUIHandler LoseGameUI;
  public EndGameUIHandler EndGameUI;

  private void Awake()
  {
    Instance = this;
  }
}
