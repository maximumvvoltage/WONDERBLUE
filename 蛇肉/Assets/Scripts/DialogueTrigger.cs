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
