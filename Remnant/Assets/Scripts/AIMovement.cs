using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class AIMovement : MonoBehaviour
{
    NavMeshAgent agent;
    public float maxTime = 1.0f;
    public float minDistance = 1.0f;

    float timer;
    public Transform playerToFollow;


    public bool isDead;

    Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        timer = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead) agent.speed = 0;
        timer -= Time.deltaTime;
        if(timer< 0)
        {
            float distance = Vector3.Distance(playerToFollow.position, agent.destination);
            if (distance > minDistance)
            {
                agent.destination = playerToFollow.position;
            }
            timer = maxTime;
        }
        animator.SetFloat("Speed", agent.velocity.magnitude);
    }
}
