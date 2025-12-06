using UnityEngine;

public class RobotManager : MonoBehaviour
{
    Animator animator;
    GameObject player;

    float MIN_DISTANCE = 2.0f;
    float animatorSpeed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        animatorSpeed = animator.speed;
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(transform.position, player.transform.position) < MIN_DISTANCE)
        {
            animatorSpeed = animator.speed == 0 ? animatorSpeed : animator.speed;
            animator.speed = 0.0f;
            animator.SetBool("stopped", true);
        } else
        {
            animator.speed = animatorSpeed;
            animator.SetBool("stopped", false);
        }
    }
}
