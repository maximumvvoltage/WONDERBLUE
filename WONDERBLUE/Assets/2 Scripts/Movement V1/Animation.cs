using UnityEngine;
using UnityEngine.EventSystems;

public class ShumAnim : MonoBehaviour
{
    public Animator animator;
    public int animBase;
    public int animLayer;

    public ShumIsMoving moveScript;
    public ShumIsSwimming swimScript;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetInteger("animBaseInt", 1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Walk()
    {
        if (moveScript.speed <= 0f)
            animator.SetInteger("animBaseInt", 1);
        
        if (moveScript.speed == 6f)
            animator.SetInteger("animBaseInt", 2);
        
        if (moveScript.speed == 10f)
            animator.SetInteger("animBaseInt", 3);
    }
}
