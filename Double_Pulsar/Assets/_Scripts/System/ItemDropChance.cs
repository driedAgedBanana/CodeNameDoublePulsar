using UnityEngine;

[System.Serializable]
public class ItemDropChance
{
    public ItemScriptableObject item;
    [Range(0, 100)] public int dropChancePercentage;
}
