using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DambiCtrl : MonoBehaviour
{

    #region 주행성
    private Transform playerTransform;
    private PlayerCtrl playerCtrl; // 플레이어 컨트롤러 참조
    private Rigidbody rb;

    public float roamingRange = 10.0f;
    public float health = 100.0f; // 동물의 체력
    public float currTime; // 현재 시간을 추적하는 변수

    private NavMeshAgent agent;
    private Animator anim;
    private float originalSpeed;
    private State currentState;
    private float idleTimer = 0f; // IDLE 상태 타이머
    private Animator animator; // 애니메이터 컴포넌트

    private float commRange = 7.0f;
    private float watchRange = 15.0f;

    #region 상태
    private enum State
    {
        WATCH,
        WALK,
        COMM,
        IDLE,
        DIE
       


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
        currentState = State.IDLE;
        
    }
    /*
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
    */

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
                currentState = State.IDLE;
                agent.enabled = false;
                animator.SetBool("IsIdle", true); // SLEEP 상태일 때 IsSleep 애니메이션 활성화
                idleTimer = 0f; // 타이머 초기화
            }
            
        }
        if (distanceToPlayer <= watchRange)
        {

            currentState = State.WATCH;
            transform.LookAt(playerTransform.position);

            if (distanceToPlayer <= commRange)
            {

                currentState = State.COMM;
                anim.SetBool("IsInteraction", true);
            }

            if (PlayerCtrl.moveSpeed > 4.0f)
            {
                currentState = State.WALK;
                anim.SetBool("IsWalk", true);
            }
            

        }
        if (distanceToPlayer > watchRange)
        {
            currentState = State.WALK;
            anim.SetBool("IsWalk", true);
            anim.SetBool("IsInteraction", false);
        }
        /*
        else if (currentState != State.TRACE && currentState != State.ATTACK && currTime > 70.0f)
        {
            currentState = State.SLEEP;
            agent.enabled = false;
            animator.SetBool("IsSleep", true); // SLEEP 상태일 때 IsSleep 애니메이션 활성화
        }
        

        if (currentState == State.SLEEP)
        {
            if (PlayerCtrl.moveSpeed != 5.0f && distanceToPlayer <= chaseRange)
            {
                currentState = State.TRACE;
                agent.enabled = true;
                agent.speed = chaseSpeed;
                animator.SetBool("IsRun", true); // 추적 상태로 변할 때 IsSleep 애니메이션 비활성화
            }
            return;
        }
        

        else if (distanceToPlayer <= chaseRange)
        {
            if (currentState != State.TRACE)
            {
                animator.SetTrigger("IsAngry");
                animator.SetBool("IsRun", true);
                currentState = State.TRACE;
                agent.enabled = true;
                agent.speed = chaseSpeed;
                idleTimer = 0f; // TRACE 상태로 변할 때 타이머 초기화
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
                idleTimer = 0f; // IDLE 상태로 변할 때 타이머 초기화
            }
        }
        */
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
            if (currentState == State.IDLE && (!agent.pathPending && agent.remainingDistance < 0.5f))
        {
            if (IsInWater())
            {
                SetRandomDestination();
            }
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

    /*
    IEnumerator Attack()
    {
        Vector3 currentVelocity = rb.velocity;
        Vector3 currentAngularVelocity = rb.angularVelocity;

        // Rigidbody의 속도와 각속도를 0으로 설정하여 멈춥니다.
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // 2초 동안 기다립니다.
        yield return new WaitForSeconds(2);

        // 저장된 속도와 각속도를 다시 적용합니다.
        rb.velocity = currentVelocity;
        rb.angularVelocity = currentAngularVelocity;
    }
    */

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
