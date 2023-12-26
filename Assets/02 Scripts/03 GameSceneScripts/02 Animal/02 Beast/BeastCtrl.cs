using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BeastCtrl : MonoBehaviour
{
    #region 주행성
    public Transform playerTransform;
    public PlayerCtrl playerCtrl; // 플레이어 컨트롤러 참조
    public float roamingRange = 10.0f;
    public float chaseRange = 30.0f;
    public float attackRange = 5.0f;
    public float chaseSpeed = 25.0f;
    public float health = 100.0f; // 동물의 체력
    public float currTime; // 현재 시간을 추적하는 변수

    private NavMeshAgent agent;
    private Animator animator; // 애니메이터 컴포넌트
    private float originalSpeed;
    private State currentState;
    private float idleTimer = 0f; // IDLE 상태 타이머

    public GameObject objectToSpawn; // 스폰할 오브젝트
    public int numberOfObjects = 5; // 스폰할 개수
    public Vector3 center; // 스폰 영역의 중심
    public Vector3 size;

    #region 상태
    private enum State
    {
        IDLE,
        TRACE,
        ATTACK,
        SLEEP,
        DIE
    }
    #endregion

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>(); // 애니메이터 컴포넌트 가져오기

        SpawnObjects();
        originalSpeed = agent.speed;
        currentState = State.IDLE;
        StartCoroutine(UpdatePath());
    }

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

    #region 야행성
    /*void Update()
    {
        if (currentState == State.DIE) return;

        float distanceToPlayer = Vector3.Distance(playerTransform.position, transform.position);

        if (currTime <= 70.0f)
        {
            if (currentState == State.SLEEP)
            {
                animator.SetBool("IsSleep", true);
                agent.enabled = false;// SLEEP 상태일 때 IsSleep 애니메이션 활성화
                if (PlayerCtrl.moveSpeed != 5.0f && distanceToPlayer <= chaseRange)
                {
                    currentState = State.TRACE;
                    agent.enabled = true;
                    agent.speed = chaseSpeed;
                    animator.SetBool("IsSleep", false); // 추적 상태로 변할 때 IsSleep 애니메이션 비활성화
                }
                return;
            }

            if (currentState == State.IDLE)
            {
                idleTimer += Time.deltaTime;
                agent.enabled = true;
                if (idleTimer >= 10f)
                {
                    currentState = State.SLEEP;
                    agent.enabled = false;
                    animator.SetBool("IsSleep", true); // SLEEP 상태일 때 IsSleep 애니메이션 활성화
                    idleTimer = 0f; // 타이머 초기화
                }
            }
        }
        else
        {
            if (distanceToPlayer <= attackRange)
            {
                currentState = State.ATTACK;
                agent.speed = chaseSpeed;
            }
            else if (distanceToPlayer <= chaseRange)
            {
                if (currentState != State.TRACE)
                {
                    currentState = State.TRACE;
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
                    agent.speed = originalSpeed;
                    SetRandomDestination();
                    idleTimer = 0f; // IDLE 상태로 변할 때 타이머 초기화
                }
            }
        }*/
        #endregion

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
                currentState = State.SLEEP;
                agent.enabled = false;
                animator.SetBool("IsSleep", true); // SLEEP 상태일 때 IsSleep 애니메이션 활성화
                idleTimer = 0f; // 타이머 초기화
            }
        }
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

        if (distanceToPlayer <= attackRange)
        {
            currentState = State.ATTACK;
            animator.SetBool("IsAttack", true);
            agent.enabled = true;
            agent.speed = chaseSpeed;
        }
        else if (distanceToPlayer <= chaseRange)
        {
            if (currentState != State.TRACE)
            {
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

        if (health <= 0)
        {
            currentState = State.DIE;
            // DIE 상태 로직
        }

        if (currentState == State.IDLE && (!agent.pathPending && agent.remainingDistance < 0.5f))
        {
            SetRandomDestination();
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
            animator.SetBool("IsDie",true);
            //게임오버
        }

    }

    void SpawnObjects()//스폰
    {
        for (int i = 0; i < numberOfObjects; i++)
        {
            Vector3 pos = center + new Vector3(
                Random.Range(-size.x / 2, size.x / 2),
                Random.Range(-size.y / 2, size.y / 2),
                Random.Range(-size.z / 2, size.z / 2)
            );

            Instantiate(objectToSpawn, pos, Quaternion.identity);
        }
    }


    void OnDrawGizmosSelected() //스폰영역 표시
    {
        Gizmos.color = new Color(1, 0, 0, 0.5f);
        Gizmos.DrawCube(center, size);
    }
    #endregion

}
