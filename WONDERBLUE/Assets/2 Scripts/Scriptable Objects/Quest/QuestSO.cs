using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Quest", menuName = "Scriptable Objects/Quest")]
public class QuestSO : ScriptableObject
{
    public string questName;
    public string questDescription;
    public bool questComplete;
}
