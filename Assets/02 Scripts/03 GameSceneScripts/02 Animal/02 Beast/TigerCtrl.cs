using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class TigerCtrl : MonoBehaviour
{
    // ���� ����
    private Transform playerTransform;
    private NavMeshAgent agent;
    private Animator anim;
    //private Animator animator; �ߺ�
    private Rigidbody rb;

    // �Ÿ�
    public float roamingRange = 10.0f;
    public float chaseRange = 20.0f;
    public float attackRange = 5.0f;

    // �ӵ�
    private float originalSpeed;
    public float chaseSpeed = 25.0f;

    // �ð�
    private float idleTimer = 0f;
    private float traceTimer = 0f;
    public float currTime;

    // ��Ÿ
    private bool canTrace = true;
    public float health = 100.0f;
    private State currentState;

    private enum State
    {
        IDLE,
        TRACE,
        ATTACK,
        SLEEP,
        WALK,
        DIE
    }

    // �ʱ�ȭ
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Start()
    {
        playerTransform = GameObject.FindWithTag("Player").GetComponent<Transform>();
        agent = GetComponent<NavMeshAgent>();
        //animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();

        originalSpeed = agent.speed;
        currentState = State.WALK;
        canTrace = true;

        StartCoroutine(UpdatePath());
    }

    // ���� & ���� ������ �� �÷��̾ �ѱ�
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

        // ���� �����̸� �� �̻� �������� ����
        if (currentState == State.DIE)
            return;

        float distanceToPlayer = Vector3.Distance(playerTransform.position, transform.position);


        // �� �ð�
        if ((currTime % 100) >= 70)
        {
            // SLEEP ����
            currentState = State.SLEEP;

            anim.SetBool("IsSleep", true);
            anim.SetBool("IsWalk", false); // IsWalk ��Ȱ��ȭ
            anim.SetBool("IsAttack", false);
            anim.SetBool("IsStun", false);
            anim.SetBool("IsRun", false);

            agent.enabled = false;
        }
        else
        {
            if (PlayerCtrl.moveSpeed < 5.0f && distanceToPlayer > chaseRange)
            {
                currentState = State.WALK;
                agent.enabled = true;

                anim.SetBool("IsSleep", false);
                anim.SetBool("IsWalk", true); // IsWalk ��Ȱ��ȭ
                anim.SetBool("IsAttack", false);
                anim.SetBool("IsStun", false);
                anim.SetBool("IsRun", false);

            }

            if (PlayerCtrl.moveSpeed != 5.0f && distanceToPlayer <= chaseRange)
            {
                // �÷��̾ ���� �ӵ��� chaseRange�� ������ ��
                // ����
                traceTimer += Time.deltaTime;

                currentState = State.TRACE;
                agent.enabled = true;
                agent.speed = chaseSpeed;

                anim.SetBool("IsRun", true);
                anim.SetBool("IsSleep", false);
                anim.SetBool("IsWalk", false);

                // Ÿ�̸Ӱ� 5�� �����ϸ� IDLE ���·� ��ȯ
                if (traceTimer >= 5.0f)
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
                        // agent.ResetPath();
                        canTrace = true;
                    }
                }
                idleTimer = 0f; // ���� ���·� ���� �� IsSleep �ִϸ��̼� ��Ȱ��ȭ
            }
            if (canTrace && distanceToPlayer <= chaseRange)
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
            if (distanceToPlayer <= attackRange)
            {
                currentState = State.ATTACK;
                anim.SetBool("IsAttack", true);
                agent.speed = chaseSpeed;
            }
        }

        // Sleep ���°� �����Ǿ��� �� ������ ������ Ȯ������ �ʵ��� �ϱ� ���� else ���� ����Ͽ� ���� �˻縦 �����մϴ�.



    }

    void SetRandomDestination()
    {
        // ���� ��ġ�� ������ ������ �Ÿ� ���
        float distanceToDestination = Vector3.Distance(transform.position, agent.destination);

        // �Ӱ谪���� �Ÿ��� ������ ���ο� ������ ����
        if (distanceToDestination < 2.0f)
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
            // DIE ���� ����
        }
        // �ѿ� �¾��� �� FAINT ���� ����
        // currentState = State.FAINT;
    }
}
