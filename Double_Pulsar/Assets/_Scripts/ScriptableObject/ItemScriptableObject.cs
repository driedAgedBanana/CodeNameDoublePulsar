using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class ItemScriptableObject : ScriptableObject
{
    public GameObject itemPrefab;
}
