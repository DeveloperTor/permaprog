namespace PermaProg.PermaProgCode.Model;
using Godot.Collections;

public class UpgradeableModel
{
    public string SliderName = "";
    public string ButtonName = "";
    public string CurrentLevelName = "";

    public int MaxLevel;
    public Array<int> Vals = [];
    public Array<int> UpgCosts = [];

    public int CurrentLevel;
    public bool Unlocked;
}