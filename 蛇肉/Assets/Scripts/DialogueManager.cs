using TMPro;
using UnityEngine;
using Ink.Runtime;

public class DialogueManager : MonoBehaviour
{
    private static DialogueManager instance;
    private DialogueTrigger dialogueTrigger;
    public KeyCode ContinueKey = KeyCode.Q;
    
    [Header("Dialogue UI")]
    [SerializeField] private GameObject entireDialogueUI;
    [SerializeField] private TextMeshProUGUI dialogueText;
    public TextMeshProUGUI nameText;
    
    private Story currentStory;
    public bool currentlySpeaking;
    

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        nameText = GetComponent<TMPro.TextMeshProUGUI>();
        currentlySpeaking = false;
        entireDialogueUI.SetActive(false);
    }

    private void Update()
    {
        //nameText.text = dialogueTrigger.name;
        
        if (!currentlySpeaking)
        {
            return;
        }
        
        if (currentStory.canContinue && Input.GetKeyDown(ContinueKey))
        {
            ContinueSpeakingMode();
        }
    }

    public static DialogueManager GetInstance()
    {
        return instance;
    }

    public void BeginSpeaking(TextAsset inkJSON)
    {
        currentStory = new Story(inkJSON.text);
        currentlySpeaking = true;
        entireDialogueUI.SetActive(true);
        
        ContinueSpeakingMode();
    }

    private void ContinueSpeakingMode()
    {
        if (currentStory.canContinue)
        {
            dialogueText.text = currentStory.Continue();
        }
        else
        {
            ExitSpeakingMode();
        }
    }
    
    private void ExitSpeakingMode()
    {
        currentlySpeaking = false;
        entireDialogueUI.SetActive(false);
        dialogueText.text = "";
    }
}
