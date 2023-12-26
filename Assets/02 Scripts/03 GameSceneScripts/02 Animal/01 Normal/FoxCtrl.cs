using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FoxCtrl : MonoBehaviour
{

    #region 주행성
    private Transform playerTransform;
    private PlayerCtrl playerCtrl; // 플레이어 컨트롤러 참조
    private Rigidbody rb;

    public float roamingRange = 100.0f;
    public float health = 100.0f; // 동물의 체력
    public float currTime; // 현재 시간을 추적하는 변수

    private NavMeshAgent agent;
    private Animator animator; // 애니메이터 컴포넌트
    private float originalSpeed;
    private State currentState;
    private float idleTimer = 0f; // IDLE 상태 타이머


    private Animator anim;
    private float commRange = 7.0f;
    private float watchRange = 15.0f;

    private Vector3 currPosition;

    #region 상태
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
                animator.SetBool("IsSleep", true); // SLEEP 상태일 때 IsSleep 애니메이션 활성화
                animator.SetBool("IsWalk", false);
                anim.SetBool("IsGuard", false);
                anim.SetBool("IsEat", false);
                anim.SetBool("IsInteraction", false);
                anim.SetBool("IsRun", false);
                Debug.Log("IsSleep");
                idleTimer = 0f; // 타이머 초기화
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
                    // 현재 위치를 저장하고 해당 위치로 고정
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
            // DIE 상태 로직
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
        // 충돌한 오브젝트에 Rigidbody가 있는지 확인
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            // 오브젝트의 속도와 각속도를 0으로 설정
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
        // 충돌한 오브젝트가 'weapon' 태그를 가지고 있는지 확인
        if (collision.gameObject.CompareTag("Weapon"))
        {
            // TakeDamage 함수 호출
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
            //게임오버
        }

    }

    #endregion
}
