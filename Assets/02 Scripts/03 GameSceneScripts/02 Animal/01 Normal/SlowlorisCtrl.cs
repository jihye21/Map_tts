using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SlowlorisCtrl : MonoBehaviour
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
    private float commRange;
    private float watchRange;

    #region ����
    private enum State
    {
        WATCH,
        COMM,
        WALK,
        SLEEP,
        DIE,
        RUN


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
        if (currentState == State.DIE) return;

        float distanceToPlayer = Vector3.Distance(playerTransform.position, transform.position);

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
        if (distanceToPlayer <= commRange)
        {
            if (currentState != State.COMM)
            {
                currentState = State.COMM;
                anim.SetBool("IsInteract", true);

            }


        }
        if (distanceToPlayer <= watchRange)
        {
            if (currentState != State.RUN)
            {
                //anim.SetBool("IsRun", true);
                currentState = State.WATCH;
                if (PlayerCtrl.moveSpeed != 5.0f)
                {
                    currentState = State.WATCH;
                    anim.SetBool("IsRun", true);
                }
            }
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
        */
        else
        {
            if (currentState != State.WALK)
            {
                currentState = State.WALK;
                agent.enabled = true;
                animator.SetBool("IsWalk", true);
                agent.speed = originalSpeed;
                SetRandomDestination();
                idleTimer = 0f; // IDLE ���·� ���� �� Ÿ�̸� �ʱ�ȭ
            }
        }

        if (health <= 0)
        {
            currentState = State.DIE;
            // DIE ���� ����
        }
        if (transform.position.y < 1.91f)
        {
            // y���� 1.91�� ����
            Vector3 newPosition = transform.position;
            newPosition.y = 1.91f;
            transform.position = newPosition;
            // waterplane�� �浹�� ���
            Debug.Log("Waterplane�� ��ҽ��ϴ�!");
            // ������ �������� ȸ����Ű��

            transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            // ���ο� ������ �������� �̵�
            Vector3 randomDirection = Random.insideUnitSphere * roamingRange;
            randomDirection += transform.position;

            NavMeshHit hit;

            // ��ȿ�� ��ġ�� ã�� ������ ��� ���ø�
            while (!NavMesh.SamplePosition(randomDirection, out hit, roamingRange, 1))
            {
                randomDirection = Random.insideUnitSphere * roamingRange;
                randomDirection += transform.position;
            }

            // ���ο� �������� �̵�
            agent.SetDestination(hit.position);
        }
        /*
        if (currentState == State.WALK && (!agent.pathPending && agent.remainingDistance < 0.5f))
        {
            SetRandomDestination();
        }
        "GetRemainingDistance" can only be called on an active agent that has been placed on a NavMesh.
UnityEngine.StackTraceUtility:ExtractStackTrace ()
        */
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
