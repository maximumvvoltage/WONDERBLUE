using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using Unity.VisualScripting;

public class StampButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Button Settings")]
    private Button stampButton;
    public Image buttonImage;
    private bool stampComplete;
    public bool stampStarted;

    [Header("Sprites")]
    public Sprite spritePending;
    public Sprite spriteStarted;
    public Sprite spriteClaim;
    public Sprite spriteComplete;

    [Header("Text Assets")]
    public TextMeshProUGUI locationName;
    public TextMeshProUGUI locationDesc;
    public TextMeshProUGUI stampcardInfo;
    
    
    [Header("Everything Else")]
    public QuestSO questSO;
    [SerializeField] private Transform waypoint;
    public bool questStarted;
    
    public static event Action<StampButton> OnStampActivated;
    public Transform Waypoint => waypoint;

    private void Start()
    {
        buttonImage.sprite = spritePending;
    }

    private void Update()
    {
        if (questSO.questComplete && !stampComplete && buttonImage.sprite != spriteClaim)
        {
            buttonImage.sprite = spriteClaim;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        stampcardInfo.text = questSO.questDescription;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        stampcardInfo.text = " ";
    }

    public void AmendQuest()
    {
        if (stampComplete)
            return;

        stampcardInfo.text = questSO.questDescription;

        if (questSO.questComplete && buttonImage.sprite == spriteClaim)
        {
            buttonImage.sprite = spriteComplete; //once the stamp is claimed, clear the target so it doesnt continue pointing to the quest area
            stampComplete = true;
        }

        
        else if (!questSO.questComplete)
        {
            locationName.text = questSO.questName;
            locationDesc.text = questSO.questDescription;
                
            OnStampActivated?.Invoke(this);
            stampStarted = true;
            stampcardInfo.text = questSO.questDescription;
            questSO.questComplete = false;
            buttonImage.sprite = spriteStarted; //if the quest is not yet complete, set the target to the questTarget in the SO 
        }
    }
}