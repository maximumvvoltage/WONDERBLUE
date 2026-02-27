using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
	[SerializeField] private DialogueManager dialogueManager;
	[Header("Visual Cue")]
	[SerializeField] private GameObject visualCue = null;

	[Header("Ink")]
	[SerializeField] private TextAsset inkJSON;

	private bool isInRange = false;
	private bool hasPlayed = false;

	void Start()
	{
		if (visualCue != null) visualCue.SetActive(false);
	}

	void Update()
	{

		if (hasPlayed) return;


		if (visualCue != null)
		{
			if (isInRange && !DialogueManager.GetInstance().dialogueIsPlaying)
			{
				visualCue.SetActive(true);

				if (Input.GetKeyDown(KeyCode.E))
				{
					DialogueManager.GetInstance().EnterDialogueMode(inkJSON);
					hasPlayed = true;
					visualCue.SetActive(false);
				}
			}
			else
			{
				visualCue.SetActive(false);
			}
		}
		else
		{
			if (isInRange && !DialogueManager.GetInstance().dialogueIsPlaying)
			{
				DialogueManager.GetInstance().EnterDialogueMode(inkJSON);
				hasPlayed = true;
			}
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			isInRange = true;
			Debug.Log("Player in range");
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			isInRange = false;
		}
	}

}


/*
 using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
	public DialogueManager dialogueManager;
	
    [SerializeField] private GameObject visualCue;
	[SerializeField] private TextAsset inkJSON;
	[SerializeField] private string name;
	
    public bool playerInRange;
    public  bool interacting;

    private void Awake()
	{
		playerInRange = false;
		visualCue.SetActive(false);
	}

	private void Update()
	{
		if (playerInRange && !dialogueManager.currentlySpeaking && Input.GetKeyDown(KeyCode.E))
		{
			interacting = true;
			visualCue.SetActive(false);
			dialogueManager.BeginSpeaking(inkJSON);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		visualCue.SetActive(true);
		playerInRange = true;
	}

	private void OnTriggerExit(Collider other)
	{
		playerInRange = false;
		visualCue.SetActive(false);
	}
}
*/
