using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class QuokkaCtrl : MonoBehaviour
{
    public Transform playerTransform;
    public Rigidbody playerRigidbody; // 플레이어의 Rigidbody
    private Rigidbody rb;
    public float roamingRange = 10.0f;
    public float health = 100.0f; // 동물의 체력
    public float currTime; // 현재 시간을 추적하는 변수

    public float sleepTimerDuration = 70.0f; // SLEEP 상태로 전환되기 전의 타이머 시간
    private float sleepTimer;

    private NavMeshAgent agent;
    private Animator animator; // 애니메이터 컴포넌트
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
            // 여기에서 hit.collider.tag 등을 사용하여 물을 판별할 수 있습니다.
            // 예를 들어, 물의 태그가 "Water"일 경우:
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

        animator = GetComponent<Animator>(); // 애니메이터 컴포넌트 가져오기
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
                // COMM 상태에서의 특정 행동을 여기에 추가
                CheckForSleepState();
                break;
            case State.SLEEP:
                // SLEEP 상태에서의 행동을 여기에 추가
                break;
                // 추가적인 상태 처리
        }
        if (transform.position.y < 1.91f && IsInWater())
        {
            // y값을 1.91로 설정
            Vector3 newPosition = transform.position;
            newPosition.y = 1.91f;
            transform.position = newPosition;
            // waterplane과 충돌한 경우
            Debug.Log("Waterplane에 닿았습니다!");

            // 방향을 무작위로 회전시키기
            transform.rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);

            // 새로운 무작위 방향으로 이동
            Vector3 randomDirection = Random.insideUnitSphere * roamingRange;
            randomDirection += transform.position;

            NavMeshHit hit;

            // 유효한 위치를 찾을 때까지 계속 샘플링
            while (!NavMesh.SamplePosition(randomDirection, out hit, roamingRange, 1))
            {
                randomDirection = Random.insideUnitSphere * roamingRange;
                randomDirection += transform.position;
            }

            // 새로운 방향으로 이동
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

    // CATCH 상태로 전환하는 메소드 (예: 특정 입력에 반응)
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
        // 충돌한 오브젝트가 'weapon' 태그를 가지고 있는지 확인
        if (collision.gameObject.CompareTag("weapon"))
        {
            // TakeDamage 함수 호출
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
            //게임오버
        }

    }
}

