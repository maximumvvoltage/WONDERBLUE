using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SearchService;

public class DialogueManager : MonoBehaviour
{
    [Header("Dialogue UI")]
    [SerializeField] private GameObject dialogueUI;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private TextMeshProUGUI displayNameText;
    [SerializeField] private GameObject choicesContainer;
    [SerializeField] private Button choiceButtonPrefab;
    
    [SerializeField] private GameObject player;
    private int selectedChoiceIndex = 0;
    [SerializeField] private Color normalChoiceColor = Color.white;
    [SerializeField] private Color highlightedChoiceColor = Color.yellow;
    private Story currentStory;


    [Header("Portrait")]
    [SerializeField] private GameObject portraitImage;
    [SerializeField] private Animator portraitAnim;
    private const string speaker_tag = "speaker";
    private const string portrait_tag = "portrait";

    public bool dialogueIsPlaying { get; private set; }

    private static DialogueManager instance;
    private List<Button> choiceButtons = new List<Button>();

    private Vector3 frozenPosition;
    private Quaternion frozenRotation;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Multiple instances of DialogueManager detected.");
        }
        instance = this;
    }

    public static DialogueManager GetInstance()
    {
        return instance;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Start()
    {
        dialogueIsPlaying = false;
        dialogueUI.SetActive(false);
    }

    // Update is called once per frame
    private void Update()
    {
        if (dialogueIsPlaying && player != null)
        {
            player.transform.position = frozenPosition;
            //player.transform.rotation = frozenRotation;
        }

        if (!dialogueIsPlaying)
        {
            return;
        }

        if (currentStory != null && currentStory.currentChoices.Count > 0)
        {
            // Move selection up
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                selectedChoiceIndex--;
                if (selectedChoiceIndex < 0)
                    selectedChoiceIndex = currentStory.currentChoices.Count - 1;

                HighlightChoices();
            }

            // Move selection down
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                selectedChoiceIndex++;
                if (selectedChoiceIndex >= currentStory.currentChoices.Count)
                    selectedChoiceIndex = 0;

                HighlightChoices();
            }

            // Select with Enter
            if (Input.GetKeyDown(KeyCode.Return))
            {
                OnChoiceSelected(selectedChoiceIndex);
                return;
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;
            if (currentStory.currentChoices.Count > 0) return;
            if (currentStory.canContinue)
            {
                ContinueStory();
            }
            else
            {
                ExitDialogueMode();
            }
        }
    }

    public void EnterDialogueMode(TextAsset inkJSON)
    {
        currentStory = new Story(inkJSON.text);
        dialogueIsPlaying = true;
        dialogueUI.SetActive(true);
        showPortrait();
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        if (player != null)
        {
            var move = player.GetComponent<Movement>();
            /*if (move != null) move.canMove = false;

            frozenPosition = player.transform.position;
            frozenRotation = player.transform.rotation;*/
        }
        if (currentStory.canContinue)
        {
            ContinueStory();
        }
        else
        {

        }
    }

    private void ExitDialogueMode()
    {
        dialogueIsPlaying = false;
        dialogueUI.SetActive(false);
        hidePortrait();
        dialogueText.text = "";
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        ClearChoices();

        if (player != null)
        {
            var move = player.GetComponent<Movement>();
            //if (move != null) move.canMove = true;
        }
    }

    private void ContinueStory()
    {
        ClearChoices();
        if (currentStory.canContinue)
        {
            dialogueText.text = currentStory.Continue();

            HandleTags(currentStory.currentTags);

            DisplayChoices();
        }
        else if (currentStory.currentChoices.Count > 0)
        {
            DisplayChoices();
        }
        else
        {
            WaitForBeat();
            ExitDialogueMode();
        }
    }

    public IEnumerator WaitForBeat()
    {
        yield return new WaitForSeconds(3);
    }

    private void DisplayChoices()
    {
        ClearChoices();
        selectedChoiceIndex = 0;

        if (currentStory.currentChoices.Count == 0) return;

        for (int i = 0; i < currentStory.currentChoices.Count; i++)
        {
            Choice choice = currentStory.currentChoices[i];
            Button choiceButton = Instantiate(choiceButtonPrefab, choicesContainer.transform);
            choiceButton.gameObject.SetActive(true);

            TextMeshProUGUI buttonText = choiceButton.GetComponentInChildren<TextMeshProUGUI>();
            buttonText.text = choice.text;

            int choiceIndex = i;
            choiceButton.onClick.RemoveAllListeners();
            choiceButton.onClick.AddListener(() => OnChoiceSelected(choiceIndex));

            choiceButtons.Add(choiceButton);
        }

        HighlightChoices();
    }

    private void OnChoiceSelected(int choiceIndex)
    {
        Debug.Log("Choice selected: " + choiceIndex);
        currentStory.ChooseChoiceIndex(choiceIndex);
        ClearChoices();
        ContinueStory();
    }

    private void ClearChoices()
    {
        foreach (var button in choiceButtons)
        {
            Destroy(button.gameObject);
        }
        choiceButtons.Clear();
    }

    private void HighlightChoices()
    {
        for (int i = 0; i < choiceButtons.Count; i++)
        {
            Image img = choiceButtons[i].GetComponentInChildren<Image>();
            if (i == selectedChoiceIndex)
            {
                img.color = highlightedChoiceColor;
            }
            else
            {
                img.color = Color.white;
            }
        }
    }

    private void HandleTags(List<string> currentTags)
    {
        if (currentTags == null || currentTags.Count == 0) return;

        foreach (string tag in currentTags)
        {
            string[] splitTag = tag.Split(':');
            if (splitTag.Length != 2)
            {
                Debug.LogError("There's an issue with the tag");
            }

            string tagKey = splitTag[0].Trim();
            string tagValue = splitTag[1].Trim();

            switch (tagKey)
            {
                case speaker_tag:
                    displayNameText.text = tagValue;
                    break;
                case portrait_tag:
                    Debug.Log("tagValue" + tagValue);
                    portraitAnim.Play(tagValue);
                   break; 
                default:
                    Debug.LogWarning("Tag isn't being used:" + tag);
                    break;
                
            }

        /*if (tag.StartsWith("emotion:"))
        {
            string emotion = tag.Replace("emotion:", "").Trim();
            SetEmotion(emotion);
            continue;
        }*/
        }
    }

    public void SetVariable(string variableName, object value)
    {
        if (currentStory != null)
        {
            currentStory.variablesState[variableName] = value;
        }
    }

    public void EnterDialogueModeWithAccuracy(TextAsset inkJSON, int accuracy)
    {
        showPortrait();

        currentStory = new Story(inkJSON.text);

        if (currentStory.variablesState.GlobalVariableExistsWithName("accuracy"))
        {
            currentStory.variablesState["accuracy"] = accuracy;
        }
        else
        {
            Debug.LogWarning("Ink variable 'accuracy' is not declared in this story.");
        }

        dialogueIsPlaying = true;
        dialogueUI.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        /*if (player != null)
        {
            var move = player.GetComponent<Movement>();
            if (move != null) move.canMove = false;

            frozenPosition = player.transform.position;
            frozenRotation = player.transform.rotation;
        }*/

        if (currentStory.canContinue)
        {
            ContinueStory();
        }
    }

    private void SetEmotion(string emotion)
    {
        if (portraitAnim == null) return;

        portraitAnim.Play(emotion);
    }

    private void showPortrait()
    {
       if (portraitImage != null)
        {
            portraitImage.SetActive(true);
        }
    }
    private void hidePortrait()
    {
        if (portraitImage != null)
        {
            portraitImage.SetActive(false);
        }
    }
}




/*
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
    [SerializeField] private GameObject portraitImage;
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
*/