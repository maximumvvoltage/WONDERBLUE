using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class Stampcard : MonoBehaviour
{ //STAMPCARD IS PURELY VISUAL; IT HANDLES THE PARENT OF THE BUTTONS, AND CONTROLLING WHEN THE MENU SHOULD BE SHOWN AND HIDDEN.
    
    [Header("Scripts")]
    [SerializeField] private DialogueManager dialogueManager;
    [SerializeField] private StampButton stampButton;
    
    
    [Header("Animators")]
    public Animator npcInteractionAnimator;
    public Animator questObjects;
    
    [Header("GameObjects & Transforms")]
    public GameObject stampCard;
    public GameObject dialogueUI;
    
    [Header("Text Assets")]
    public TextMeshProUGUI locationName;
    public TextMeshProUGUI locationDesc;
    public TextMeshProUGUI stampcardInfo;
    
    public bool questComplete;
    public StampStatus stampStatus; //the public enum
    
    public enum StampStatus
    {
        NotStarted,
        Started,
        Claimable,
        Completed
    }

    public void ShowStampCard() 
    {
        stampCard.SetActive(true);
        npcInteractionAnimator.Play("Hide-NPC-Interaction");
        npcInteractionAnimator.SetTrigger("Show");
        dialogueUI.SetActive(false);
    }
    public void HideStampCard() 
    {
        npcInteractionAnimator.Play("Show-NPC-Dialogue");
        npcInteractionAnimator.SetTrigger("Hide");
        dialogueUI.SetActive(true); 
        stampCard.SetActive(false);
    }

    /*public void ShowQuestObjects()
    {
        if (dialogueManager.currentStory.canContinue == false && StampStatus.Started == stampStatus || StampStatus.Claimable == stampStatus)
        {
            questObjects.Play("EnterQuestObjets");
        }

        if(StampStatus.Completed == stampStatus || dialogueManager.dialogueIsPlaying)
        {
            questObjects.Play("ExitQuestObjets");
        }
    }*/

    public void AmendQuest()
    {
        if (stampButton.stampComplete)
            return;
        
        if (stampButton.questSO.questComplete && stampStatus == StampStatus.Claimable) 
        {
            stampButton.buttonImage.sprite = stampButton.spriteClaim;
        }

        stampcardInfo.text = stampButton.questSO.questDescription;

        switch (stampStatus)
        {
            case StampStatus.NotStarted:
                
                stampStatus = StampStatus.Started;
                stampButton.buttonImage.sprite = stampButton.spritePending;
                break;
                
            case StampStatus.Started:
                
                stampStatus = StampStatus.Started;
                stampButton.questSO.questComplete = false;
                
                questObjects.Play("EnterQuestObjets");

                locationName.text = stampButton.questSO.questName; //the location name 
                locationDesc.text = stampButton.questSO.questDescription;
                stampcardInfo.text = stampButton.questSO.questDescription;
                
                stampButton.buttonImage.sprite = stampButton.spriteStarted;
                StampButton.InvokeOnStampActivated(stampButton);
                break;
            
            case StampStatus.Claimable:
                
                stampStatus = StampStatus.Claimable;
                stampButton.questSO.questComplete = true;
                
                locationName.text = "Talk to Mawiri";
                locationDesc.text = "Task complete! Go back to Mawiri to claim your stamp!";
                
                stampButton.buttonImage.sprite = stampButton.spriteClaim;
                break;
            
            case StampStatus.Completed:
                
                stampStatus = StampStatus.Completed;
                stampButton.buttonImage.sprite = stampButton.spriteComplete;
                
                questObjects.Play("ExitQuestObjets");
                
                stampButton.questSO.questComplete = stampButton.spriteComplete;

                break;
        }
    }
}