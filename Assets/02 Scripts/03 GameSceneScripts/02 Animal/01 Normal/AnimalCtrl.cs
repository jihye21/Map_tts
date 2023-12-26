using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AnimalCtrl : MonoBehaviour
{
    public Transform playerTransform;
    public Rigidbody playerRigidbody; // �÷��̾��� Rigidbody
    public float roamingRange = 10.0f;
    public float health = 100.0f; // ������ ü��
    public float currTime; // ���� �ð��� �����ϴ� ����

    public float sleepTimerDuration = 70.0f; // SLEEP ���·� ��ȯ�Ǳ� ���� Ÿ�̸� �ð�
    private float sleepTimer;

    private NavMeshAgent agent;
    private Animator animator; // �ִϸ����� ������Ʈ
    private float originalSpeed;
    private State currentState;

    public float watchRange = 20.0f;
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

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>(); // �ִϸ����� ������Ʈ ��������

        currentState = State.IDLE;
        sleepTimer = sleepTimerDuration;
    }

    void Update()
    {
        sleepTimer -= Time.deltaTime;

        if (currentState == State.DIE) return;

        switch (currentState)
        {
            case State.IDLE:
                SetRandomDestination();
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
    }

    #region ���༺
    /*public float watchRange = 20.0f;
    public float commRange = 7.0f;
    public float nightDuration = 70.0f; // �� �ð� ���� �Ⱓ
    private float nightTimer; // �� �ð� ī��Ʈ�ٿ� Ÿ�̸�
    public Transform playerTransform; // �÷��̾��� Transform
    public Rigidbody playerRigidbody; // �÷��̾��� Rigidbody

    private enum State { IDLE, WATCH, RUN, COMM, CATCH, SLEEP }
    private State currentState = State.SLEEP;

    void Start()
    {
        nightTimer = nightDuration;
    }

    void Update()
    {
        // Ÿ�̸� ������Ʈ
        nightTimer -= Time.deltaTime;

        if (nightTimer <= 0)
        {
            // �� �ð� ������ ���� ��ȭ
            switch (currentState)
            {
                case State.IDLE:
                    WanderRandomly();
                    CheckForWatchState();
                    break;
                case State.WATCH:
                    CheckForRunState();
                    break;
                case State.RUN:
                    CheckForCommState();
                    break;
                case State.COMM:
                    // COMM ���¿����� Ư�� �ൿ�� ���⿡ �߰�
                    break;
                case State.SLEEP:
                    CheckForWakeUp();
                    break;
                // �߰����� ���� ó��
            }
        }
        else
        {
            // �� �ð� ���� SLEEP ���� ����
            currentState = State.SLEEP;
        }
    }

    void WanderRandomly()
    {
        // IDLE ���¿����� ������ ��ġ �̵� ����
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

    void CheckForWakeUp()
    {
        // ���� �Ǹ� SLEEP ���¿��� ����� ���� (��: IDLE ���·� ��ȯ)
        currentState = State.IDLE;
    }*/
    #endregion


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
