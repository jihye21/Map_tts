using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BeastCtrl : MonoBehaviour
{
    #region ���༺
    public Transform playerTransform;
    public PlayerCtrl playerCtrl; // �÷��̾� ��Ʈ�ѷ� ����
    public float roamingRange = 10.0f;
    public float chaseRange = 30.0f;
    public float attackRange = 5.0f;
    public float chaseSpeed = 25.0f;
    public float health = 100.0f; // ������ ü��
    public float currTime; // ���� �ð��� �����ϴ� ����

    private NavMeshAgent agent;
    private Animator animator; // �ִϸ����� ������Ʈ
    private float originalSpeed;
    private State currentState;
    private float idleTimer = 0f; // IDLE ���� Ÿ�̸�

    public GameObject objectToSpawn; // ������ ������Ʈ
    public int numberOfObjects = 5; // ������ ����
    public Vector3 center; // ���� ������ �߽�
    public Vector3 size;

    #region ����
    private enum State
    {
        IDLE,
        TRACE,
        ATTACK,
        SLEEP,
        DIE
    }
    #endregion

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>(); // �ִϸ����� ������Ʈ ��������

        SpawnObjects();
        originalSpeed = agent.speed;
        currentState = State.IDLE;
        StartCoroutine(UpdatePath());
    }

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

    #region ���༺
    /*void Update()
    {
        if (currentState == State.DIE) return;

        float distanceToPlayer = Vector3.Distance(playerTransform.position, transform.position);

        if (currTime <= 70.0f)
        {
            if (currentState == State.SLEEP)
            {
                animator.SetBool("IsSleep", true);
                agent.enabled = false;// SLEEP ������ �� IsSleep �ִϸ��̼� Ȱ��ȭ
                if (PlayerCtrl.moveSpeed != 5.0f && distanceToPlayer <= chaseRange)
                {
                    currentState = State.TRACE;
                    agent.enabled = true;
                    agent.speed = chaseSpeed;
                    animator.SetBool("IsSleep", false); // ���� ���·� ���� �� IsSleep �ִϸ��̼� ��Ȱ��ȭ
                }
                return;
            }

            if (currentState == State.IDLE)
            {
                idleTimer += Time.deltaTime;
                agent.enabled = true;
                if (idleTimer >= 10f)
                {
                    currentState = State.SLEEP;
                    agent.enabled = false;
                    animator.SetBool("IsSleep", true); // SLEEP ������ �� IsSleep �ִϸ��̼� Ȱ��ȭ
                    idleTimer = 0f; // Ÿ�̸� �ʱ�ȭ
                }
            }
        }
        else
        {
            if (distanceToPlayer <= attackRange)
            {
                currentState = State.ATTACK;
                agent.speed = chaseSpeed;
            }
            else if (distanceToPlayer <= chaseRange)
            {
                if (currentState != State.TRACE)
                {
                    currentState = State.TRACE;
                    agent.speed = chaseSpeed;
                    idleTimer = 0f; // TRACE ���·� ���� �� Ÿ�̸� �ʱ�ȭ
                }
            }
            else
            {
                if (currentState != State.IDLE)
                {
                    currentState = State.IDLE;
                    agent.enabled = true;
                    agent.speed = originalSpeed;
                    SetRandomDestination();
                    idleTimer = 0f; // IDLE ���·� ���� �� Ÿ�̸� �ʱ�ȭ
                }
            }
        }*/
        #endregion

        void Update()
    {
        if (currentState == State.DIE) return;

        float distanceToPlayer = Vector3.Distance(playerTransform.position, transform.position);

        if (currentState == State.IDLE)
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

        if (distanceToPlayer <= attackRange)
        {
            currentState = State.ATTACK;
            animator.SetBool("IsAttack", true);
            agent.enabled = true;
            agent.speed = chaseSpeed;
        }
        else if (distanceToPlayer <= chaseRange)
        {
            if (currentState != State.TRACE)
            {
                animator.SetBool("IsRun", true);
                currentState = State.TRACE;
                agent.enabled = true;
                agent.speed = chaseSpeed;
                idleTimer = 0f; // TRACE ���·� ���� �� Ÿ�̸� �ʱ�ȭ
            }
        }
        else
        {
            if (currentState != State.IDLE)
            {
                currentState = State.IDLE;
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

        if (currentState == State.IDLE && (!agent.pathPending && agent.remainingDistance < 0.5f))
        {
            SetRandomDestination();
        }
    }

    void SetRandomDestination()
    {
        if (currentState == State.IDLE)
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
        if (collision.gameObject.CompareTag("weapon"))
        {
            // TakeDamage �Լ� ȣ��
            TakeDamage(10.0f);
        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            currentState = State.DIE;
            animator.SetBool("IsDie",true);
            //���ӿ���
        }

    }

    void SpawnObjects()//����
    {
        for (int i = 0; i < numberOfObjects; i++)
        {
            Vector3 pos = center + new Vector3(
                Random.Range(-size.x / 2, size.x / 2),
                Random.Range(-size.y / 2, size.y / 2),
                Random.Range(-size.z / 2, size.z / 2)
            );

            Instantiate(objectToSpawn, pos, Quaternion.identity);
        }
    }


    void OnDrawGizmosSelected() //�������� ǥ��
    {
        Gizmos.color = new Color(1, 0, 0, 0.5f);
        Gizmos.DrawCube(center, size);
    }
    #endregion

}
