using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public int level;
    public int unlockedLevel;
    public int numberOfDeaths;

    public GameData(int level, int unlockedLevel, int numberOfDeaths)
    {
        this.level = level;
        this.unlockedLevel = unlockedLevel;
        this.numberOfDeaths = numberOfDeaths;
    }
}
