using UnityEngine;

public class Stampcard : MonoBehaviour
{ //STAMPCARD IS PURELY VISUAL; IT HANDLES THE PARENT OF THE BUTTONS, AND CONTROLLING WHEN THE MENU SHOULD BE SHOWN AND HIDDEN.
    public GameObject dialogueUI;
    public Animator npcInteractionAnimator;
    public GameObject stampCard;
    public StampButton stampButton;

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
    
    public void ResetDescription()
    {
        stampButton.stampcardInfo.text = " ";
    }



}