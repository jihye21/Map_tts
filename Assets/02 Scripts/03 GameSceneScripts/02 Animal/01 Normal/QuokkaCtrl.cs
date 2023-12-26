using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class QuokkaCtrl : MonoBehaviour
{
    public Transform playerTransform;
    public Rigidbody playerRigidbody; // �÷��̾��� Rigidbody
    private Rigidbody rb;
    public float roamingRange = 10.0f;
    public float health = 100.0f; // ������ ü��
    public float currTime; // ���� �ð��� �����ϴ� ����

    public float sleepTimerDuration = 70.0f; // SLEEP ���·� ��ȯ�Ǳ� ���� Ÿ�̸� �ð�
    private float sleepTimer;

    private NavMeshAgent agent;
    private Animator animator; // �ִϸ����� ������Ʈ
    private float originalSpeed;
    private State currentState;

    private Animator anim;

    public float watchRange = 15.0f;
    public float commRange = 7.0f;

    private enum State
    {
        IDLE,
        RUN,
        WATCH,
        COMM,
        SLEEP,
        CATCH,
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


        originalSpeed = agent.speed;
        currentState = State.IDLE;

    }

    void Update()
    {
        sleepTimer -= Time.deltaTime;

        if (currentState == State.DIE) return;

        switch (currentState)
        {
            case State.IDLE:
                WanderRandomly();
                CheckForSleepState();
                CheckForWatchState();
                break;
            case State.WATCH:
                CheckForRunState();
                CheckForSleepState();
                break;
            case State.RUN:
                CheckForCommState();
                CheckForSleepState();
                break;
            case State.COMM:
                // COMM ���¿����� Ư�� �ൿ�� ���⿡ �߰�
                CheckForSleepState();
                break;
            case State.SLEEP:
                // SLEEP ���¿����� �ൿ�� ���⿡ �߰�
                break;
                // �߰����� ���� ó��
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


    void WanderRandomly()
    {
        SetRandomDestination();
    }

    void CheckForSleepState()
    {
        if (sleepTimer <= 0)
        {
            if (Vector3.Distance(transform.position, playerTransform.position) > 20.0f)
            {
                currentState = State.SLEEP;
                StartCoroutine(ResetToSleepAfterIdle());
            }
            else if (playerRigidbody.velocity.magnitude > 5.0f)
            {
                currentState = State.WATCH;
            }
        }
    }

    IEnumerator ResetToSleepAfterIdle()
    {
        yield return new WaitForSeconds(10.0f);
        currentState = State.SLEEP;
    }

    void CheckForWatchState()
    {
        if (Vector3.Distance(transform.position, playerTransform.position) <= watchRange)
        {
            currentState = State.WATCH;
        }
    }

    void CheckForRunState()
    {
        if (playerRigidbody.velocity.magnitude > 5.0f)
        {
            currentState = State.RUN;
        }
    }

    void CheckForCommState()
    {
        if (Vector3.Distance(transform.position, playerTransform.position) <= commRange)
        {
            currentState = State.COMM;
        }
    }

    // CATCH ���·� ��ȯ�ϴ� �޼ҵ� (��: Ư�� �Է¿� ����)
    public void TriggerCatchState()
    {
        currentState = State.CATCH;
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
            animator.SetBool("IsDie", true);
            //���ӿ���
        }

    }
}

