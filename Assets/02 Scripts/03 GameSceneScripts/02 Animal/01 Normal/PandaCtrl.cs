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

    private Animator anim; //애니메이터 컴포넌트

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
        RUN,
        GUARD
    }
    #endregion
    
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }
    void Start()
    {
        playerTransform = GameObject.FindWithTag("Player").GetComponent<Transform>();
        agent = GetComponent<NavMeshAgent>();

        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        
        originalSpeed = agent.speed;
        currentState = State.WALK;

    }


    void Update()
    {
        SetRandomDestination();
        currTime += Time.deltaTime;

        float distanceToPlayer = Vector3.Distance(playerTransform.position, transform.position);

        // 낮 시간
        if ((currTime % 100) >= 70)
        {
            // SLEEP 상태
            currentState = State.SLEEP;

            anim.SetBool("IsSleep", true);
            anim.SetBool("IsWalk", false); // IsWalk 비활성화
            agent.enabled = false;
        }
        else if (distanceToPlayer > watchRange)
        {
            currentState = State.WALK;
            agent.enabled =true;
        
            anim.SetBool("IsWalk", true);
            anim.SetBool("IsInteraction", false);
            anim.SetBool("IsGuard", false);
            anim.SetBool("IsRun", false);
            anim.SetBool("IsEat", false);
            anim.SetBool("IsSleep", false);
        }
        else if (distanceToPlayer <= watchRange && PlayerCtrl.moveSpeed < 2.0f)
        {
            // 현재 위치를 저장하고 해당 위치로 고정
            currPosition = transform.position;
            //플레이어 바라보기
            transform.LookAt(playerTransform.position);

            currentState = State.GUARD;
            anim.SetBool("IsGuard", true);

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
        else if (PlayerCtrl.moveSpeed > 2.0f)
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
        else 
        {
            currentState = State.WALK;
            agent.enabled = true;

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
            Debug.Log("타격");
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
            anim.SetBool("IsDie", true);
            //게임오버
        }

    }

    #endregion
}
