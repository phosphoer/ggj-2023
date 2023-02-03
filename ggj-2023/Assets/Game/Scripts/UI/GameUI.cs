using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUI : Singleton<GameUI>
{
    public WorldAttachedUI AngelWorldAttachedUI;
    public WorldAttachedUI DevilWorldAttachedUI;
    public PlayerUIHandler AngelUI;
    public PlayerUIHandler DevilUI;
    public MainMenuUIHandler MainMenuUI;
    public SettingsUIHandler SettingsUI;
    public SidebarUIHandler SidebarUI;
    public ScenarioIntroUIHandler ScenarioIntroUI;
    public ScenarioUIHandler ScenarioUI;
    public ScenarioOutroUIHandler ScenarioOutroUI;
    public EndGameUIHandler EndGameUI;

    private void Awake()
    {
        Instance = this;
    }
}
