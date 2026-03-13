using UnityEngine;

public class QuestArea : MonoBehaviour
{
    [SerializeField] private StampButton linkedStamp; // drag the specific StampButton here
    [SerializeField] private Stampcard stampcard;
    private bool questCompleted = false;
    
    private bool playerInside = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInside = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInside = false;
    }

    private void Update()
    {
        if (playerInside && Input.anyKeyDown)
        {
            CompleteQuest();
        }
    }

    private void CompleteQuest()
    {
        if (linkedStamp == null) return;
        if (questCompleted) return; //prevents it from resetting
    
        questCompleted = true;
        linkedStamp.questSO.questComplete = true;
        stampcard.SetActiveButton(linkedStamp);
        stampcard.stampStatus = Stampcard.StampStatus.Claimable;
        stampcard.AmendQuest(); // THIS whichforces an immediate visual update. took a long ass while to figure out
    }
}