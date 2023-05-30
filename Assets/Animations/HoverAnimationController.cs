using UnityEngine;

public class HoverAnimationController : MonoBehaviour
{
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void BeginAnimation()
    {
        animator.SetBool("HoverBool", true);
    }

    public void EndAnimation()
    {
        animator.SetBool("HoverBool", false);
    }

}