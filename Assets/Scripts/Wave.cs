using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Wave
{
    public enum Difficulty { Normal, Hard, GodMode};
    public int WaveNumber;
    public string Title;
    public int AmountOfEnemiesToBeSpawned;
    public Difficulty DifficultyLevel;
    public bool IsThereABoss;
    
}
