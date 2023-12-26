using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class OtterCtrl : MonoBehaviour
{

    #region ���༺
    private Transform playerTransform;
    private PlayerCtrl playerCtrl; // �÷��̾� ��Ʈ�ѷ� ����
    private Rigidbody rb;


    public float roamingRange = 10.0f;
    public float health = 100.0f; // ������ ü��
    public float currTime; // ���� �ð��� �����ϴ� ����

    private NavMeshAgent agent;
    private Animator animator; // �ִϸ����� ������Ʈ
    private float originalSpeed;
    private State currentState;
    private float idleTimer = 0f; // IDLE ���� Ÿ�̸�


    private Animator anim;
    private float commRange = 5.0f;
    private float watchRange = 10.0f;

    private Vector3 currPosition;

    #region ����
    private enum State
    {
        WATCH,
        COMM,
        WALK,
        SLEEP,
        DIE,
        RUN,
        GUARD


    }
    #endregion
   
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }
    void Start()
    {
        
        playerTransform = GameObject.FindWithTag("Player").GetComponent<Transform>();
        agent = GetComponent<NavMeshAgent>();

        animator = GetComponent<Animator>(); // �ִϸ����� ������Ʈ ��������
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        transform.forward = -Vector3.forward;

        originalSpeed = agent.speed;
        currentState = State.WALK;

    }
    /*
    IEnumerator UpdatePath()
    {
        
        while (true)
        {
            if (currentState == State.TRACE || currentState == State.ATTACK)
            {
                agent.SetDestination(playerTransform.position);
            }
            yield return new WaitForSeconds(0.1f);
        }
        
    }
    */

    void Update()
    {

        if (currentState == State.DIE || agent == null || !agent.isOnNavMesh) return;

        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        if (currentState == State.WALK)
        {
            animator.SetBool("IsWalk", true);
            idleTimer += Time.deltaTime;
            agent.enabled = true;

            if (idleTimer >= 10f && currTime > 70.0f)
            {
                currentState = State.SLEEP;
                agent.enabled = false;
                animator.SetBool("IsSleep", true); // SLEEP ������ �� IsSleep �ִϸ��̼� Ȱ��ȭ
                idleTimer = 0f; // Ÿ�̸� �ʱ�ȭ
            }

        }


        if (distanceToPlayer <= watchRange)
        {

            currentState = State.WATCH;
            
            

            if (PlayerCtrl.moveSpeed < 2.0f)
            {
                transform.LookAt(playerTransform.position);
                anim.SetBool("IsGuard", true);
                anim.SetBool("IsWalk", false);
                anim.SetBool("IsInteraction", false);
                anim.SetBool("IsRun", false);
                if (anim.GetBool("IsGuard"))
                {
                    // ���� ��ġ�� �����ϰ� �ش� ��ġ�� ����
                    currPosition = transform.position;
                    

                    if (distanceToPlayer <= commRange)
                    {
                        transform.forward = -Vector3.forward;
                        currentState = State.COMM;
                        anim.SetBool("IsInteraction", true);
                        anim.SetBool("IsWalk", false);
                        anim.SetBool("IsGuard", false);
                        anim.SetBool("IsRun", false);


                        /*
                        if (anim.GetBool("IsInteraction"))
                        {
                            // ���� ��ġ�� �����ϰ� �ش� ��ġ�� ����
                            currPosition = transform.position;
                        }
                        */
                    }
                }
            }
            else
            {
                
                anim.SetBool("IsGuard", false);
                anim.SetBool("IsInteraction", false);
                anim.SetBool("IsWalk", true);
                anim.SetBool("IsRun", false);
            }


            if (PlayerCtrl.moveSpeed > 2.0f)
            {
                currentState = State.RUN;
                transform.forward = Vector3.forward;
                anim.SetBool("IsRun", true);
                anim.SetBool("IsWalk", false);
                anim.SetBool("IsInteraction", false);
                anim.SetBool("IsGuard", false);
            }




        }
        if (distanceToPlayer > watchRange)
        {
            transform.forward = -Vector3.forward;
            currentState = State.WALK;
            anim.SetBool("IsWalk", true);
            anim.SetBool("IsInteraction", false);
            anim.SetBool("IsGuard", false);
            anim.SetBool("IsRun", false);
        }







        /*
        else if (currentState != State.TRACE && currentState != State.ATTACK && currTime > 70.0f)
        {
            currentState = State.SLEEP;
            agent.enabled = false;
            animator.SetBool("IsSleep", true); // SLEEP ������ �� IsSleep �ִϸ��̼� Ȱ��ȭ
        }
        

        if (currentState == State.SLEEP)
        {
            if (PlayerCtrl.moveSpeed != 5.0f && distanceToPlayer <= chaseRange)
            {
                currentState = State.TRACE;
                agent.enabled = true;
                agent.speed = chaseSpeed;
                animator.SetBool("IsRun", true); // ���� ���·� ���� �� IsSleep �ִϸ��̼� ��Ȱ��ȭ
            }
            return;
        }
        

        else if (distanceToPlayer <= chaseRange)
        {
            if (currentState != State.TRACE)
            {
                animator.SetTrigger("IsAngry");
                animator.SetBool("IsRun", true);
                currentState = State.TRACE;
                agent.enabled = true;
                agent.speed = chaseSpeed;
                idleTimer = 0f; // TRACE ���·� ���� �� Ÿ�̸� �ʱ�ȭ
            }
        }
        
        
            if (currentState != State.WALK)
            {
                currentState = State.WALK;
                agent.enabled = true;
                animator.SetBool("IsWalk", true);
                agent.speed = originalSpeed;
                SetRandomDestination();
                idleTimer = 0f; // IDLE ���·� ���� �� Ÿ�̸� �ʱ�ȭ
            }
       */

        if (health <= 0)
        {
            currentState = State.DIE;
            // DIE ���� ����
        }
        
        if (currentState == State.WALK && (!agent.pathPending && agent.remainingDistance < 0.5f))
        {
            
                SetRandomDestination();
            
        }
    }

    void SetRandomDestination()
    {
        if (currentState == State.WALK)
        {
            Vector3 randomDirection = Random.insideUnitSphere * roamingRange;
            randomDirection += transform.position;
            NavMeshHit hit;
            NavMesh.SamplePosition(randomDirection, out hit, roamingRange, 1);
            agent.SetDestination(hit.position);
        }
    }
    void OnCollisionEnter(Collision collision)
    {
        // �浹�� ������Ʈ�� 'weapon' �±׸� ������ �ִ��� Ȯ��
        if (collision.gameObject.CompareTag("Weapon"))
        {
            // TakeDamage �Լ� ȣ��
            TakeDamage(10.0f);
        }

        if (collision.gameObject.CompareTag("Player"))
        {
            TakeDamage(10.0f);
        }
    }

    /*
    IEnumerator Attack()
    {
        Vector3 currentVelocity = rb.velocity;
        Vector3 currentAngularVelocity = rb.angularVelocity;

        // Rigidbody�� �ӵ��� ���ӵ��� 0���� �����Ͽ� ����ϴ�.
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // 2�� ���� ��ٸ��ϴ�.
        yield return new WaitForSeconds(2);

        // ����� �ӵ��� ���ӵ��� �ٽ� �����մϴ�.
        rb.velocity = currentVelocity;
        rb.angularVelocity = currentAngularVelocity;
    }
    */

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            currentState = State.DIE;
            animator.SetBool("IsDie", true);
            //���ӿ���
        }

    }

    #endregion
}
