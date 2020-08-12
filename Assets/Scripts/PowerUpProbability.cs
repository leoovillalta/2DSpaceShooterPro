using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PowerUpProbability
{
    public enum StatusEffect { Positive,Negative};
    public GameObject PowerUp;
    public float SpawnProbability;
    public StatusEffect _statusEffect;
}
