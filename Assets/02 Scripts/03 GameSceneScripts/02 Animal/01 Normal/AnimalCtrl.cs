using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AnimalCtrl : MonoBehaviour
{
    public Transform playerTransform;
    public Rigidbody playerRigidbody; // 플레이어의 Rigidbody
    public float roamingRange = 10.0f;
    public float health = 100.0f; // 동물의 체력
    public float currTime; // 현재 시간을 추적하는 변수

    public float sleepTimerDuration = 70.0f; // SLEEP 상태로 전환되기 전의 타이머 시간
    private float sleepTimer;

    private NavMeshAgent agent;
    private Animator animator; // 애니메이터 컴포넌트
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
        animator = GetComponent<Animator>(); // 애니메이터 컴포넌트 가져오기

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
                // COMM 상태에서의 특정 행동을 여기에 추가
                CheckForSleepState();
                break;
            case State.SLEEP:
                // SLEEP 상태에서의 행동을 여기에 추가
                break;
                // 추가적인 상태 처리
        }
    }

    #region 야행성
    /*public float watchRange = 20.0f;
    public float commRange = 7.0f;
    public float nightDuration = 70.0f; // 밤 시간 지속 기간
    private float nightTimer; // 밤 시간 카운트다운 타이머
    public Transform playerTransform; // 플레이어의 Transform
    public Rigidbody playerRigidbody; // 플레이어의 Rigidbody

    private enum State { IDLE, WATCH, RUN, COMM, CATCH, SLEEP }
    private State currentState = State.SLEEP;

    void Start()
    {
        nightTimer = nightDuration;
    }

    void Update()
    {
        // 타이머 업데이트
        nightTimer -= Time.deltaTime;

        if (nightTimer <= 0)
        {
            // 낮 시간 동안의 상태 변화
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
                    // COMM 상태에서의 특정 행동을 여기에 추가
                    break;
                case State.SLEEP:
                    CheckForWakeUp();
                    break;
                // 추가적인 상태 처리
            }
        }
        else
        {
            // 밤 시간 동안 SLEEP 상태 유지
            currentState = State.SLEEP;
        }
    }

    void WanderRandomly()
    {
        // IDLE 상태에서의 랜덤한 위치 이동 로직
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
        // 낮이 되면 SLEEP 상태에서 벗어나는 로직 (예: IDLE 상태로 전환)
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
