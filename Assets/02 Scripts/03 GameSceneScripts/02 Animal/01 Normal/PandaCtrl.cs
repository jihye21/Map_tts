using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PandaCtrl : MonoBehaviour
{

    #region 주행성
    private Transform playerTransform;
    private PlayerCtrl playerCtrl; // 플레이어 컨트롤러 참조
    private Rigidbody rb;
    

    public float roamingRange = 10.0f;
    public float health = 100.0f; // 동물의 체력
    public float currTime; // 현재 시간을 추적하는 변수

    private NavMeshAgent agent;
    
    private float originalSpeed; //현 속도
    private State currentState; //현상태
    private float idleTimer = 0f; // IDLE 상태 타이머

    private Animator animator; // 애니메이터 컴포넌트
    private Animator anim; //22
    private float commRange = 7.0f; //교감 범위
    private float watchRange = 10.0f; //시야 범위

    private Vector3 currPosition;//현재 위치

    public LayerMask groundLayer; // 땅을 나타내는 레이어
    public float raycastLength = 10.0f; // 레이캐스트 길이

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
    void AlignObjectToGround()
    {
        RaycastHit hit;
        // 오브젝트 아래로 레이캐스트를 발사
        if (Physics.Raycast(transform.position, -Vector3.up, out hit, raycastLength, groundLayer))
        {
            Vector3 targetPosition = hit.point; // 땅과 충돌한 지점
            transform.position = targetPosition;

            Quaternion targetRotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
            transform.rotation = targetRotation;
        }
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
        AlignObjectToGround();
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

            if (idleTimer >= 10.0f && currTime > 70.0f)
            {
                Debug.Log("sleep time");
                currentState = State.SLEEP;
                agent.enabled = false;
                currPosition = transform.position;
                animator.SetBool("IsSleep", true); // SLEEP 상태일 때 IsSleep 애니메이션 활성화
                animator.SetBool("IsWalk", false);
                anim.SetBool("IsGuard", false);
                anim.SetBool("IsEat", false);
                anim.SetBool("IsInteraction", false);
                anim.SetBool("IsRun", false);
                Debug.Log("IsSleep");

                if ((currTime % 100) < 70)//낮에 일어나기
                {
                    idleTimer = 0f; // 타이머 초기화
                    Debug.Log("wake up");
                    agent.enabled = true;
                }
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
                Debug.Log("a");
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


        
    }

    
    void SetRandomDestination()
    {
        float distanceToDestination = Vector3.Distance(transform.position, agent.destination);
        /*
        Vector3 randomDirection = Random.insideUnitSphere * roamingRange;
        randomDirection += transform.position;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, roamingRange, 1);
        agent.SetDestination(hit.position);
        */

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
            animator.SetBool("IsDie", true);
            //게임오버
        }

    }

    #endregion
}
