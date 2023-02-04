using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUI : Singleton<GameUI>
{
    public MainMenuUIHandler MainMenuUI;
    public EndGameUIHandler EndGameUI;

    private void Awake()
    {
        Instance = this;
    }
}
