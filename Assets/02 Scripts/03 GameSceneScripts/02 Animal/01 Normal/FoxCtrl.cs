using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FoxCtrl : MonoBehaviour
{

    #region ���༺
    private Transform playerTransform;
    private PlayerCtrl playerCtrl; // �÷��̾� ��Ʈ�ѷ� ����
    private Rigidbody rb;

    public float roamingRange = 100.0f;
    public float health = 100.0f; // ������ ü��
    public float currTime; // ���� �ð��� �����ϴ� ����

    private NavMeshAgent agent;
    private Animator animator; // �ִϸ����� ������Ʈ
    private float originalSpeed;
    private State currentState;
    private float idleTimer = 0f; // IDLE ���� Ÿ�̸�


    private Animator anim;
    private float commRange = 7.0f;
    private float watchRange = 15.0f;

    private Vector3 currPosition;

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
    bool IsInWater()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 2.0f))
        {
            // ���⿡�� hit.collider.tag ���� ����Ͽ� ���� �Ǻ��� �� �ֽ��ϴ�.
            // ���� ���, ���� �±װ� "Water"�� ���:
            return hit.collider.CompareTag("Water");
        }

        return false;
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

            if (idleTimer >= 10.0f && (currTime % 100) >= 70)
            {
                Debug.Log("sleep time");
                currentState = State.SLEEP;
                agent.enabled = false;
                animator.SetBool("IsSleep", true); // SLEEP ������ �� IsSleep �ִϸ��̼� Ȱ��ȭ
                animator.SetBool("IsWalk", false);
                anim.SetBool("IsGuard", false);
                anim.SetBool("IsEat", false);
                anim.SetBool("IsInteraction", false);
                anim.SetBool("IsRun", false);
                Debug.Log("IsSleep");
                idleTimer = 0f; // Ÿ�̸� �ʱ�ȭ
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


        if (health <= 0)
        {
            currentState = State.DIE;
            // DIE ���� ����
        }
        if (transform.position.y < 1.91f && IsInWater())
        {

            // y���� 1.91�� ����
            Vector3 newPosition = transform.position;
            newPosition.y = 1.91f;
            transform.position = newPosition;

            // waterplane�� �浹�� ���
            Debug.Log("Waterplane�� ��ҽ��ϴ�!");

            // ������ �������� ȸ����Ű��
            transform.rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);

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
        if (currentState == State.WALK && (!agent.pathPending && agent.remainingDistance < 0.5f))
        {
            if (IsInWater())
            {
                SetRandomDestination();
            }
        }
    }

    void SetRandomDestination()
    {
        
            Vector3 randomDirection = Random.insideUnitSphere * roamingRange;
            randomDirection += transform.position;
            NavMeshHit hit;
            NavMesh.SamplePosition(randomDirection, out hit, roamingRange, 1);
            agent.SetDestination(hit.position);
        
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
