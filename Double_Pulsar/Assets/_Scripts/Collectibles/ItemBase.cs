using UnityEngine;

public abstract class ItemBase : MonoBehaviour
{
    public abstract void OnCollect(PlayerController player);
}
