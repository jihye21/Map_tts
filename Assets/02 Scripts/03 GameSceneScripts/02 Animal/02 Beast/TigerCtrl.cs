using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class TigerCtrl : MonoBehaviour
{
    // 변수 선언
    private Transform playerTransform;
    private NavMeshAgent agent;
    private Animator anim;
    //private Animator animator; 중복
    private Rigidbody rb;

    // 거리
    public float roamingRange = 10.0f;
    public float chaseRange = 20.0f;
    public float attackRange = 5.0f;

    // 속도
    private float originalSpeed;
    public float chaseSpeed = 25.0f;

    // 시간
    private float idleTimer = 0f;
    private float traceTimer = 0f;
    public float currTime;

    // 기타
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

    // 초기화
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

    // 추적 & 공격 상태일 때 플레이어를 쫓기
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

        // 죽은 상태이면 더 이상 진행하지 않음
        if (currentState == State.DIE)
            return;

        float distanceToPlayer = Vector3.Distance(playerTransform.position, transform.position);


        // 낮 시간
        if ((currTime % 100) >= 70)
        {
            // SLEEP 상태
            currentState = State.SLEEP;

            anim.SetBool("IsSleep", true);
            anim.SetBool("IsWalk", false); // IsWalk 비활성화
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
                anim.SetBool("IsWalk", true); // IsWalk 비활성화
                anim.SetBool("IsAttack", false);
                anim.SetBool("IsStun", false);
                anim.SetBool("IsRun", false);

            }

            if (PlayerCtrl.moveSpeed != 5.0f && distanceToPlayer <= chaseRange)
            {
                // 플레이어가 빠른 속도로 chaseRange에 들어왔을 때
                // 추적
                traceTimer += Time.deltaTime;

                currentState = State.TRACE;
                agent.enabled = true;
                agent.speed = chaseSpeed;

                anim.SetBool("IsRun", true);
                anim.SetBool("IsSleep", false);
                anim.SetBool("IsWalk", false);

                // 타이머가 5에 도달하면 IDLE 상태로 전환
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
                idleTimer = 0f; // 추적 상태로 변할 때 IsSleep 애니메이션 비활성화
            }
            if (canTrace && distanceToPlayer <= chaseRange)
            {
                if (currentState != State.TRACE)
                {
                    anim.SetBool("IsRun", true);
                    currentState = State.TRACE;
                    traceTimer = 5.0f;
                    agent.speed = chaseSpeed;
                    idleTimer = 0f; // TRACE 상태로 변할 때 타이머 초기화
                }
            }
            if (distanceToPlayer <= attackRange)
            {
                currentState = State.ATTACK;
                anim.SetBool("IsAttack", true);
                agent.speed = chaseSpeed;
            }
        }

        // Sleep 상태가 설정되었을 때 이후의 조건을 확인하지 않도록 하기 위해 else 문을 사용하여 조건 검사를 종료합니다.



    }

    void SetRandomDestination()
    {
        // 현재 위치와 목적지 사이의 거리 계산
        float distanceToDestination = Vector3.Distance(transform.position, agent.destination);

        // 임계값보다 거리가 가까우면 새로운 목적지 설정
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
        // 충돌한 오브젝트에 Rigidbody가 있는지 확인
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            // 오브젝트의 속도와 각속도를 0으로 설정
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        if (collision.gameObject.CompareTag("BULLET"))
        {
            Debug.Log("멸종위기종 때리면 감옥가요");
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
            // DIE 상태 로직
        }
        // 총에 맞았을 때 FAINT 상태 로직
        // currentState = State.FAINT;
    }
}
