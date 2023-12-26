using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCtrl : MonoBehaviour
{
    public float currTime;//현재 시간 변수

    #region 이동 변수
    private float h = 0.0f;
    private float v = 0.0f;

    private Transform tr;
    private Animator anim;
    private Rigidbody rb;

    static public float moveSpeed = 3.0f;
    private float normalSpeed = 3.0f;
    private float runSpeed = 5.0f;
    private float walkSpeed = 1.0f; 
    public float rotSpeed = 50.0f;
    private float jumpForce = 3.0f; // 점프 힘
    private bool isGrounded;
    #endregion

    #region 스테이터스 바 UI

    public Image healthBarImage; // HP 바로 사용될 Image
    public float maxHealth = 100f; // 최대 HP
    private float currentHealth; // 현재 HP
    

    // 허기와 갈증 변수
    public float maxHunger = 100f;
    public float currentHunger;
    public Image hungerBarImage;

    public float maxThirst = 100f;
    public float currentThirst;
    public Image thirstBarImage;

    // 허기와 갈증이 감소하는 주기와 양
    //public float decreaseInterval = 5f; // 5초마다
    public float decreaseAmount = 5f; // 5씩 감소

    #endregion

    #region 업데이트 hp바
    void UpdateHealthBar()
    {
        float fillAmount = currentHealth / maxHealth; // fillAmount 계산
        healthBarImage.fillAmount = fillAmount; // Image의 fillAmount 업데이트
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage; // 데미지만큼 현재 체력 감소
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // 체력이 0 이하로 떨어지지 않도록 함
        UpdateHealthBar(); // 체력바 업데이트

        if(currentHealth == 0)
        {
            Debug.Log("gameOver");
        }
    }
    #endregion

    public Text bedNearbyText; // 침대 텍스트 변수 선언
    private Vector3 savedPosition;//현재 위치 저장
    private Quaternion savedRotation; // 현재 회전을 저장할 변수

    void Start()
    {
        bedNearbyText.gameObject.SetActive(false);//침대 텍스트 숨기기

        tr = GetComponent<Transform>();
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        currentHealth = maxHealth; // 시작 시 현재 HP를 최대치로 설정

        
        // 초기 허기와 갈증을 최대치로 설정
        currentHunger = maxHunger;
        currentThirst = maxThirst;

        // 코루틴 시작
        StartCoroutine(DecreaseHungerAndThirst());

        
        StartCoroutine(DecreaseHealth());
        
    }

    void Update()
    {
        currTime += Time.deltaTime;//현재 시간
        #region 이동 구현
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");

        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            //rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            anim.SetBool("IsJump", true);
            //anim.SetBool("IsWalk", true);
        }


        if (Input.GetKey(KeyCode.LeftShift))
            moveSpeed = runSpeed;
        else if (Input.GetKey(KeyCode.LeftControl))
            moveSpeed = walkSpeed;
        else
            moveSpeed = normalSpeed;

        Vector3 moveDir = (Vector3.forward * v) + (Vector3.right * h);

        tr.Translate(moveDir.normalized * Time.deltaTime * moveSpeed, Space.Self);
        tr.Rotate(Vector3.up * Time.deltaTime * rotSpeed * Input.GetAxis("Mouse X"));

        PlayerAnim(h, v);

        #endregion

       
    }
    
    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
            anim.SetBool("IsWalk", false);
            anim.SetBool("IsBackWalk", false);
        }
    }

#region 침대 상호작용
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bed") && (currTime % 100) >= 70)
        {
            bedNearbyText.text = "'F'를 눌러 잠을 잘 수 있습니다."; // 텍스트 설정
            bedNearbyText.gameObject.SetActive(true); // 텍스트 표시
            
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Bed") && Input.GetKeyDown(KeyCode.F))
        {
            savedPosition = transform.position; // 현재 오브젝트의 위치를 savedPosition에 저장
            savedRotation = transform.rotation; // 현재 오브젝트의 회전을 savedRotation에 저장
            StartCoroutine(SleepRoutine());
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Bed"))
        {
            bedNearbyText.gameObject.SetActive(false);
            // 메시지 숨기기
        }
    }
    IEnumerator SleepRoutine()
    {

        bedNearbyText.text = "숙면 중";
        // 눕기
        
        transform.position = new Vector3(172.064f, 22.448f, 269.675f);//누워잇을 위치
        transform.rotation = Quaternion.Euler(-85.971f, 248.9f, -334.937f); //누워잇을 각도
        rb.isKinematic = true;//물리엔진에 의한 이동 및 회전 방지'^'
        yield return new WaitForSeconds(5); // 5초간 대기

        // 깨어나기
        bedNearbyText.text = "일어났습니다";
        transform.position = savedPosition;// 자기 전 위치로 이동
        transform.rotation = savedRotation;//자기 전 각도로 이동
        rb.isKinematic = false; // 물리 엔진에 의한 이동 및 회전 허용

        yield return new WaitForSeconds(5);
        bedNearbyText.gameObject.SetActive(false);
    }
    #endregion



    IEnumerator DecreaseHungerAndThirst()
        {
            while (true)
            {
                yield return new WaitForSeconds(20);

                // 허기와 갈증 감소
                currentHunger = Mathf.Max(currentHunger - decreaseAmount, 0);
                currentThirst = Mathf.Max(currentThirst - decreaseAmount, 0);

                // UI 업데이트
                UpdateHungerBar();
                UpdateThirstBar();
            }
    
        }

    void UpdateHungerBar()
    {
        hungerBarImage.fillAmount = currentHunger / maxHunger;
    }

    void UpdateThirstBar()
    {
        thirstBarImage.fillAmount = currentThirst / maxThirst;
    }

    void OnCollisionEnter(Collision collision)
    {
        // 충돌한 오브젝트가 'Beast' 태그를 가지고 있는지 확인
        if (collision.gameObject.CompareTag("Beast"))
        {
            // 생명력을 10 감소시킴
            TakeDamage(10f);

            Slow();

            // 생명력이 0 이하인 경우 처리할 로직 (옵션)
            if (currentHealth <= 0)
            {
                // 생명력이 0 이하일 때 수행할 동작
                Debug.Log("Player is out of health!");
            }
            if (currentHunger > 70 && currentHealth <= 100)
            {
                StartCoroutine(RecoverHealthAndHunger(10)); // 코루틴 시작
            }
        }

        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            anim.SetBool("IsJump",false);
        }
    }
    IEnumerator RecoverHealthAndHunger(int damageAmount)
    {
        for (int Dam = damageAmount; Dam > 0; Dam--)
        {
            yield return new WaitForSeconds(1); // 1초 대기
            currentHunger -= 1; // 허기 감소
            currentHealth += 1; // 체력 증가

            UpdateHealthBar(); // 체력 바 업데이트
            UpdateHungerBar(); // 허기 바 업데이트
        }
    }

    IEnumerator DecreaseHealth()
    {

        while (true)
        {
            yield return new WaitForSeconds(5);

            if (currentHunger == 0 || currentThirst == 0)
            {
                currentHealth = Mathf.Max(currentHealth - decreaseAmount, 0);
                UpdateHealthBar();
            }
        }
    }

    IEnumerator Slow()
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

    void PlayerAnim(float h, float v)
    {
        if (v >= 0.1f)
        {
            anim.SetBool("IsWalk", true);
            //anim.SetBool("IsJump", false);
            //anim.SetBool("IsSitWalk", false);
            if (Input.GetKey(KeyCode.C))
            {
                anim.SetBool("IsWalk", false);
                //anim.SetBool("IsSitWalk", true);
            }
        }
        else if (v <= -0.1f)
        {
            anim.SetBool("IsBackWalk", true);
            //anim.SetBool("IsJump", false);
            //anim.SetBool("IsSitWalk", false);
            if (Input.GetKey(KeyCode.C))
            {
                anim.SetBool("IsBackWalk", false);
               // anim.SetBool("IsSitWalk", true);
            }
        }
        else if (h >= 0.1f)
        {
            anim.SetBool("IsLeftWalk", true);
            //anim.SetBool("IsJump", false);
           //anim.SetBool("IsSitWalk", false);
            if (Input.GetKey(KeyCode.C))
            {
                anim.SetBool("IsLeftWalk", false);
                //anim.SetBool("IsSitWalk", true);
            }
        }
        else if (h <= -0.1f)
        {
            anim.SetBool("IsRightWalk", true);
            //anim.SetBool("IsJump", false);
           //anim.SetBool("IsSitWalk", false);
            if (Input.GetKey(KeyCode.C))
            {
                anim.SetBool("IsRightWalk", false);
               // anim.SetBool("IsSitWalk", true);
            }
        }
        else
        {
            anim.SetBool("IsWalk", false);
            anim.SetBool("IsBackWalk", false);
            anim.SetBool("IsIdle", true);
            anim.SetBool("IsRightWalk", false);
            anim.SetBool("IsLeftWalk", false);
            anim.SetBool("IsJump", false);
            anim.SetBool("IsSit", false);
            //anim.SetBool("IsSitWalk", false);
        }
    }
}
