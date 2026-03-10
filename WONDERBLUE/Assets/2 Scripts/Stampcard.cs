using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Stampcard : MonoBehaviour
{ 
    //STAMPCARD IS PURELY VISUAL; IT HANDLES THE PARENT OF THE BUTTONS, AND CONTROLLING WHEN THE MENU SHOULD BE SHOWN AND HIDDEN.
    
    [Header("Scripts")]
    [SerializeField] private DialogueManager dialogueManager;
    [SerializeField] private StampButton stampButton;
    
    
    [Header("Animators")]
    public Animator npcInteractionAnimator;
    public Animator questAnim;
    private bool questObjectsVisible = false;
    
    [Header("GameObjects & Transforms")]
    public GameObject stampCard;
    public GameObject dialogueUI;
    
    [Header("Text Assets")]
    public TextMeshProUGUI locationName;
    public TextMeshProUGUI locationDesc;
    public TextMeshProUGUI stampcardInfo;
    
    public bool questComplete;
    public StampStatus stampStatus = StampStatus.Started; //even if the stamp hasn't started yet, the button will start 
                                                          //looking like its pending, so that on the first click, all the quest
    
    public enum StampStatus
    {
        Started,
        Claimable,
        Completed
    }

    void Start()
    {
        RectTransform rectTrans = GetComponent<RectTransform>();
    }

    void Update()
    {
        if (!dialogueManager.dialogueIsPlaying && 
            (stampStatus == StampStatus.Started || stampStatus == StampStatus.Claimable))
        {
            if (!questObjectsVisible) //only calling if not already open
            {
                questObjectsVisible = true;
                ShowQuestObjects();
            }
        }
        else if (dialogueManager.dialogueIsPlaying)
        {
            if (questObjectsVisible) //only call if not already closed
            {
                questObjectsVisible = false;
                HideQuestObjects();
            }
        }
    }
    //------- show / hide quest objects
    public void ShowQuestObjects()
    {
        questAnim.Play("EnterQuestObjets");
        Debug.Log("opened");
    }
    public void HideQuestObjects()
    {
        questAnim.Play("ExitQuestObjets");
        Debug.Log("closed");
    }
    
    //----- show / hide stampcard

    public void ShowStampCard() 
    {
        stampCard.SetActive(true);
        npcInteractionAnimator.Play("Hide-NPC-Dialogue");
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
    
    //------ setting the stamp that the game gets all its most updated info from
    
    public void SetActiveButton(StampButton button)
    {
        stampButton = button;
    }
    
    //-------- the infamous AmendQuest

    public void AmendQuest()
    {
        if (stampButton.stampComplete)
            return;

        stampcardInfo.text = stampButton.questSO.questDescription;

        switch (stampStatus)
        {
                
            case StampStatus.Started:
                
                stampStatus = StampStatus.Started;
                stampButton.buttonImage.sprite = stampButton.spriteStarted;
                stampButton.questSO.questComplete = false;

                locationName.text = stampButton.questSO.questName; //the location name 
                locationDesc.text = stampButton.questSO.questDescription;
                stampcardInfo.text = stampButton.questSO.questDescription;
                
                ShowQuestObjects();
                StampButton.InvokeOnStampActivated(stampButton);
                
                break;

            case StampStatus.Claimable:

                if (!stampButton.questSO.questComplete)
                    return;
                
                stampStatus = StampStatus.Claimable;
                stampButton.buttonImage.sprite = stampButton.spriteClaim;
                locationName.text = "Talk to Mawiri";
                locationDesc.text = "Task complete! Go back to Mawiri to claim your stamp!";
                
                break;
            
            case StampStatus.Completed:

                if (stampStatus != StampStatus.Claimable)
                    return;
                
                stampStatus = StampStatus.Completed;
                stampButton.buttonImage.sprite = stampButton.spriteComplete;
                stampButton.questSO.questComplete = true;
                HideQuestObjects();
                
                break;
        }
    }
}