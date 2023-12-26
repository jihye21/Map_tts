using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PandaCtrl : MonoBehaviour
{

    #region ���༺
    private Transform playerTransform;
    private PlayerCtrl playerCtrl; // �÷��̾� ��Ʈ�ѷ� ����
    private Rigidbody rb;
    

    public float roamingRange = 10.0f;
    public float health = 100.0f; // ������ ü��
    public float currTime; // ���� �ð��� �����ϴ� ����

    private NavMeshAgent agent;
    
    private float originalSpeed; //�� �ӵ�
    private State currentState; //������
    private float idleTimer = 0f; // IDLE ���� Ÿ�̸�

    private Animator animator; // �ִϸ����� ������Ʈ
    private Animator anim; //22
    private float commRange = 7.0f; //���� ����
    private float watchRange = 10.0f; //�þ� ����

    private Vector3 currPosition;//���� ��ġ

    public LayerMask groundLayer; // ���� ��Ÿ���� ���̾�
    public float raycastLength = 10.0f; // ����ĳ��Ʈ ����

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
    void AlignObjectToGround()
    {
        RaycastHit hit;
        // ������Ʈ �Ʒ��� ����ĳ��Ʈ�� �߻�
        if (Physics.Raycast(transform.position, -Vector3.up, out hit, raycastLength, groundLayer))
        {
            Vector3 targetPosition = hit.point; // ���� �浹�� ����
            transform.position = targetPosition;

            Quaternion targetRotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
            transform.rotation = targetRotation;
        }
    }
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
  

    void Update()
    {
        AlignObjectToGround();
        currTime += Time.deltaTime;

        if (currentState == State.DIE || agent == null || !agent.isOnNavMesh) return;

        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        if (currentState == State.WALK)
        {
            originalSpeed = 3;
            animator.SetBool("IsWalk", true);
            anim.SetBool("IsGuard", false);
            anim.SetBool("IsWalk", false);
            anim.SetBool("IsInteraction", false);
            anim.SetBool("IsRun", false);
            anim.SetBool("IsSleep", false);
            anim.SetBool("IsEat", false);
            idleTimer += Time.deltaTime;
            agent.enabled = true;
            SetRandomDestination();

            if (idleTimer >= 10.0f && currTime > 70.0f)
            {
                Debug.Log("sleep time");
                currentState = State.SLEEP;
                agent.enabled = false;
                currPosition = transform.position;
                animator.SetBool("IsSleep", true); // SLEEP ������ �� IsSleep �ִϸ��̼� Ȱ��ȭ
                animator.SetBool("IsWalk", false);
                anim.SetBool("IsGuard", false);
                anim.SetBool("IsEat", false);
                anim.SetBool("IsInteraction", false);
                anim.SetBool("IsRun", false);
                Debug.Log("IsSleep");

                if ((currTime % 100) < 70)//���� �Ͼ��
                {
                    idleTimer = 0f; // Ÿ�̸� �ʱ�ȭ
                    Debug.Log("wake up");
                    agent.enabled = true;
                }
            }
            else
            {
                originalSpeed = 3;
                animator.SetBool("IsWalk", true);
                anim.SetBool("IsGuard", false);
                anim.SetBool("IsWalk", false);
                anim.SetBool("IsInteraction", false);
                anim.SetBool("IsRun", false);
                anim.SetBool("IsSleep", false);
                anim.SetBool("IsEat", false);
                idleTimer += Time.deltaTime;
                agent.enabled = true;
                SetRandomDestination();
                
            }

        }

        if (distanceToPlayer <= watchRange)
        {
            if (PlayerCtrl.moveSpeed < 2.0f)
            {
                transform.LookAt(playerTransform.position);

                anim.SetBool("IsGuard", true);
                anim.SetBool("IsWalk", false);
                anim.SetBool("IsInteraction", false);
                anim.SetBool("IsRun", false);
                anim.SetBool("IsSleep", false);
                anim.SetBool("IsEat", false);

                if (anim.GetBool("IsGuard"))
                {
                    // ���� ��ġ�� �����ϰ� �ش� ��ġ�� ����
                    currPosition = transform.position;


                    if (distanceToPlayer <= commRange)
                    {
                        currentState = State.COMM;
                        anim.SetBool("IsInteraction", true);
                        anim.SetBool("IsWalk", false);
                        anim.SetBool("IsGuard", false);
                        anim.SetBool("IsRun", false);
                        anim.SetBool("IsEat", false);
                        anim.SetBool("IsSleep", false);

                    }
                    else
                    {
                        anim.SetBool("IsInteraction", false);
                        anim.SetBool("IsGuard", true);
                    }
                }

            }
            
            else
            {
                anim.SetBool("IsGuard", false);
                anim.SetBool("IsInteraction", false);
                anim.SetBool("IsWalk", true);
                anim.SetBool("IsRun", false);
                anim.SetBool("IsEat", false);
                anim.SetBool("IsSleep", false);
            }
            

            if (PlayerCtrl.moveSpeed > 2.0f)
            {
                Debug.Log("a");
                currentState = State.RUN;
                distanceToPlayer += 10.0f;

                anim.SetBool("IsRun", true);
                anim.SetBool("IsWalk", false);
                anim.SetBool("IsInteraction", false);
                anim.SetBool("IsGuard", false);
                anim.SetBool("IsEat", false);
                anim.SetBool("IsSleep", false);

                if (currentState == State.RUN)
                {
                    originalSpeed = 6;
                    SetRandomDestination();
                }
            }




        }


        if (distanceToPlayer > watchRange)
        {
            currentState = State.WALK;
            anim.SetBool("IsWalk", true);
            anim.SetBool("IsInteraction", false);
            anim.SetBool("IsGuard", false);
            anim.SetBool("IsRun", false);
            anim.SetBool("IsEat", false);
            anim.SetBool("IsSleep", false);
        }


        
    }

    
    void SetRandomDestination()
    {
        float distanceToDestination = Vector3.Distance(transform.position, agent.destination);
        /*
        Vector3 randomDirection = Random.insideUnitSphere * roamingRange;
        randomDirection += transform.position;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, roamingRange, 1);
        agent.SetDestination(hit.position);
        */

        if (distanceToDestination < 2.0f) // �Ӱ谪�� ��Ȳ�� �°� ����
        {
            Vector3 randomDirection = Random.insideUnitSphere * roamingRange;
            randomDirection += transform.position;
            NavMeshHit hit;

            if (NavMesh.SamplePosition(randomDirection, out hit, roamingRange, 5))
            {
                agent.SetDestination(hit.position);
            }
        }
    }
    
    void OnCollisionEnter(Collision collision)
    {
        // �浹�� ������Ʈ�� Rigidbody�� �ִ��� Ȯ��
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            // ������Ʈ�� �ӵ��� ���ӵ��� 0���� ����
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
        if (collision.gameObject.CompareTag("BULLET"))
        {
            Debug.Log("���������� ������ ��������");
        }

        if (collision.gameObject.CompareTag("Player"))
        {
            TakeDamage(10.0f);
        }
    }

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
