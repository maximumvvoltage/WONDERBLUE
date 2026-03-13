using UnityEngine;

[CreateAssetMenu(fileName = "Collectables", menuName = "Scriptable Objects/Collectables")]
public class CollectableSO : ScriptableObject
{
    public string itemName;
    public string itemDescription;
    public int itemID;
    public string LossieComment;
}
