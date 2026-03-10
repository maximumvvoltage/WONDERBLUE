using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class StampButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Button Settings")]
    private Button stampButton;
    public bool stampComplete;
    public Image buttonImage;

    [Header("Sprites")]
    public Sprite spritePending;
    public Sprite spriteStarted;
    public Sprite spriteClaim;
    public Sprite spriteComplete;

    [Header("Everything Else")]
    private Stampcard stampcard;
    public QuestSO questSO;
    public bool questStarted;
    
    public Transform Waypoint => waypoint;
    [SerializeField] private Transform waypoint; //waypoints connected to each individual button
    public static event Action<StampButton> OnStampActivated;

    public static void InvokeOnStampActivated(StampButton stamp)
    {
        OnStampActivated?.Invoke(stamp);
    }
    
    private void Start()
    {
        buttonImage.sprite = spritePending;
        stampcard = FindObjectOfType<Stampcard>();
    }
    
    // --------- mouse activation!

    public void OnPointerEnter(PointerEventData eventData)
    {
        stampcard.stampcardInfo.text = questSO.questDescription;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        stampcard.stampcardInfo.text = " ";
    }
    
    public void OnButtonPressed()
    {
        stampcard.SetActiveButton(this);
        stampcard.AmendQuest();
    }
}