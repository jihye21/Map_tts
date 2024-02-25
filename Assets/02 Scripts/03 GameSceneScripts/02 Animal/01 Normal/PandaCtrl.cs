using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PandaCtrl : MonoBehaviour
{

    #region ���༺
    private Transform playerTransform;
    private PlayerCtrl playerCtrl; // �÷��̾� ��Ʈ�ѷ� ����
    private Rigidbody rb;
    

    public float roamingRange = 10.0f;
    public float health = 100.0f; // ������ ü��
    public float currTime; // ���� �ð��� �����ϴ� ����

    private NavMeshAgent agent;
    
    private float originalSpeed; //�� �ӵ�
    private State currentState; //������

    private Animator anim; //�ִϸ����� ������Ʈ

    private float commRange = 7.0f; //���� ����
    private float watchRange = 10.0f; //�þ� ����

    private Vector3 currPosition;//���� ��ġ

    public LayerMask groundLayer; // ���� ��Ÿ���� ���̾�
    public float raycastLength = 10.0f; // ����ĳ��Ʈ ����

    #region ����
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

        // �� �ð�
        if ((currTime % 100) >= 70)
        {
            // SLEEP ����
            currentState = State.SLEEP;

            anim.SetBool("IsSleep", true);
            anim.SetBool("IsWalk", false); // IsWalk ��Ȱ��ȭ
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
            // ���� ��ġ�� �����ϰ� �ش� ��ġ�� ����
            currPosition = transform.position;
            //�÷��̾� �ٶ󺸱�
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
        
        if (distanceToDestination < 2.0f) // �Ӱ谪�� ��Ȳ�� �°� ����
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
        // �浹�� ������Ʈ�� Rigidbody�� �ִ��� Ȯ��
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            // ������Ʈ�� �ӵ��� ���ӵ��� 0���� ����
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
        if (collision.gameObject.CompareTag("BULLET"))
        {
            Debug.Log("Ÿ��");
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
            //���ӿ���
        }

    }

    #endregion
}
