using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject options;
    public bool gamepaused;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //SoundManager.Play("Song1");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenOptions()
    {
        options.SetActive(true);
        gamepaused = true;
    }

    public void CloseOptions()
    {
        options.SetActive(false); 
        gamepaused = false;
    }
}
