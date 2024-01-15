using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TigerCtrl : MonoBehaviour
{

    #region 변수 선언
    private Transform playerTransform;
    private NavMeshAgent agent;
    private Animator anim;
    private Animator animator; // 애니메이터 컴포넌트
    private Rigidbody rb;
    //거리
    public float roamingRange = 10.0f;
    public float chaseRange = 20.0f;
    public float attackRange = 5.0f;

    //속도
    private float originalSpeed;
    public float chaseSpeed = 25.0f;

    //시간
    private float idleTimer = 0f; // IDLE 상태 타이머
    private float traceTimer = 0f;
    public float currTime;
    
    //기타
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

        //변수 값 넣어주기
        originalSpeed = agent.speed;
        currentState = State.WALK;
        canTrace = true;

        //함수 실행
        StartCoroutine(UpdatePath());
    }

    //추적&공격 상태일 때 플레이어를 쫓기
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

        //낮 시간
        if ((currTime % 100) >= 70)
        {
            //SLEEP 상태
            currentState = State.SLEEP;
            if (currentState == State.SLEEP)
            {
                anim.SetBool("IsSleep", true);
                anim.SetBool("IsWalk", false); // IsWalk 비활성화
                agent.enabled = false;

                //플레이어가 빠른 속도로 chaseRange에 들어왔을 때
                if (canTrace && PlayerCtrl.moveSpeed != 5.0f && distanceToPlayer <= chaseRange)
                {
                    //추적
                    traceTimer += Time.deltaTime;

                    currentState = State.TRACE;
                    agent.enabled = true;
                    agent.speed = chaseSpeed;

                    anim.SetBool("IsRun", true);
                    anim.SetBool("IsSleep", false);
                    anim.SetBool("IsWalk", false);

                    //타이머가 5에 도달하면 IDLE 상태로 전환
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
                    idleTimer = 0f;// 추적 상태로 변할 때 IsSleep 애니메이션 비활성화
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
                    anim.SetBool("IsSleep", true); // SLEEP 상태일 때 IsSleep 애니메이션 활성화
                    anim.SetBool("IsWalk", false);
                    agent.enabled = false;
                    idleTimer = 0f; // 타이머 초기화
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
                    idleTimer = 0f; // TRACE 상태로 변할 때 타이머 초기화
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
                    idleTimer = 0f; // IDLE 상태로 변할 때 타이머 초기화
                }
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

        void SetRandomDestination()
        {
            // 현재 위치와 목적지 사이의 거리를 계산
            float distanceToDestination = Vector3.Distance(transform.position, agent.destination);
            /*
                Vector3 randomDirection = Random.insideUnitSphere * roamingRange;
                randomDirection += transform.position;
                NavMeshHit hit;
               
                NavMesh.SamplePosition(randomDirection, out hit, roamingRange, 5);
                agent.SetDestination(hit.position);
            */

            // 임계값보다 거리가 가까우면 새로운 목적지 설정
            if (distanceToDestination < 2.0f) // 임계값은 상황에 맞게 조정
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