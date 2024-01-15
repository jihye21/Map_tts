using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TigerCtrl : MonoBehaviour
{

    #region ���� ����
    private Transform playerTransform;
    private NavMeshAgent agent;
    private Animator anim;
    private Animator animator; // �ִϸ����� ������Ʈ
    private Rigidbody rb;
    //�Ÿ�
    public float roamingRange = 10.0f;
    public float chaseRange = 20.0f;
    public float attackRange = 5.0f;

    //�ӵ�
    private float originalSpeed;
    public float chaseSpeed = 25.0f;

    //�ð�
    private float idleTimer = 0f; // IDLE ���� Ÿ�̸�
    private float traceTimer = 0f;
    public float currTime;
    
    //��Ÿ
    private bool canTrace = true;
    public float health = 100.0f;
    private State currentState;


    #endregion

    private enum State
    {
        IDLE,
        TRACE,
        ATTACK,
        SLEEP,
        WALK,
        DIE
    }
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

        //���� �� �־��ֱ�
        originalSpeed = agent.speed;
        currentState = State.WALK;
        canTrace = true;

        //�Լ� ����
        StartCoroutine(UpdatePath());
    }

    //����&���� ������ �� �÷��̾ �ѱ�
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

    void Update()
    {
        SetRandomDestination();
        currTime += Time.deltaTime;
        if (currentState == State.DIE) return;

        float distanceToPlayer = Vector3.Distance(playerTransform.position, transform.position);

        //�� �ð�
        if ((currTime % 100) >= 70)
        {
            //SLEEP ����
            currentState = State.SLEEP;
            if (currentState == State.SLEEP)
            {
                anim.SetBool("IsSleep", true);
                anim.SetBool("IsWalk", false); // IsWalk ��Ȱ��ȭ
                agent.enabled = false;

                //�÷��̾ ���� �ӵ��� chaseRange�� ������ ��
                if (canTrace && PlayerCtrl.moveSpeed != 5.0f && distanceToPlayer <= chaseRange)
                {
                    //����
                    traceTimer += Time.deltaTime;

                    currentState = State.TRACE;
                    agent.enabled = true;
                    agent.speed = chaseSpeed;

                    anim.SetBool("IsRun", true);
                    anim.SetBool("IsSleep", false);
                    anim.SetBool("IsWalk", false);

                    //Ÿ�̸Ӱ� 5�� �����ϸ� IDLE ���·� ��ȯ
                    if (traceTimer >= 1.0f)
                    {
                        agent.speed = originalSpeed;
                        canTrace = false;

                        Debug.Log("Finish");
                        anim.SetBool("IsWalk", true);
                        anim.SetBool("IsRun", false);
                        anim.SetBool("IsSleep", false);

                        if (traceTimer >= 10.0f)
                        {
                            traceTimer = 0.0f;
                            //agent.ResetPath();
                            canTrace = true;
                        }
                    }
                    idleTimer = 0f;// ���� ���·� ���� �� IsSleep �ִϸ��̼� ��Ȱ��ȭ
                }
                return;
            }
            /*
            if (currentState == State.IDLE)
            {
                idleTimer += Time.deltaTime;
                if (idleTimer <= 10f)
                {
                    currentState = State.SLEEP;
                    anim.SetBool("IsSleep", true); // SLEEP ������ �� IsSleep �ִϸ��̼� Ȱ��ȭ
                    anim.SetBool("IsWalk", false);
                    agent.enabled = false;
                    idleTimer = 0f; // Ÿ�̸� �ʱ�ȭ
                }
            }
            */
        }
        else
        {
            if (distanceToPlayer <= attackRange)
            {
                currentState = State.ATTACK;
                anim.SetBool("IsAttack", true);
                agent.speed = chaseSpeed;
            }
            else if (canTrace && distanceToPlayer <= chaseRange)
            {
                if (currentState != State.TRACE)
                {
                    anim.SetBool("IsRun", true);
                    currentState = State.TRACE;
                    traceTimer = 5.0f;
                    agent.speed = chaseSpeed;
                    idleTimer = 0f; // TRACE ���·� ���� �� Ÿ�̸� �ʱ�ȭ
                }
            }
            else
            {
                if (currentState != State.IDLE)
                {
                    currentState = State.IDLE;
                    agent.speed = originalSpeed;
                    anim.SetBool("IsWalk", true);
                    SetRandomDestination();
                    idleTimer = 0f; // IDLE ���·� ���� �� Ÿ�̸� �ʱ�ȭ
                }
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

                if (currentState == State.IDLE && (!agent.pathPending && agent.remainingDistance < 0.5f))
                {
                    if (IsInWater())
                    {
                        SetRandomDestination();
                    }
                }
            }
        }

        void SetRandomDestination()
        {
            // ���� ��ġ�� ������ ������ �Ÿ��� ���
            float distanceToDestination = Vector3.Distance(transform.position, agent.destination);
            /*
                Vector3 randomDirection = Random.insideUnitSphere * roamingRange;
                randomDirection += transform.position;
                NavMeshHit hit;
               
                NavMesh.SamplePosition(randomDirection, out hit, roamingRange, 5);
                agent.SetDestination(hit.position);
            */

            // �Ӱ谪���� �Ÿ��� ������ ���ο� ������ ����
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
            // DIE ���� ����
        }
        // �ѿ� �¾��� �� FAINT ���� ����
        // currentState = State.FAINT;
    }
}