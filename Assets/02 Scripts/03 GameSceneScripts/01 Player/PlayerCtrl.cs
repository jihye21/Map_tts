using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCtrl : MonoBehaviour
{
    public float currTime;//���� �ð� ����

    #region �̵� ����
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
    private float jumpForce = 3.0f; // ���� ��
    private bool isGrounded;
    #endregion

    #region �������ͽ� �� UI

    public Image healthBarImage; // HP �ٷ� ���� Image
    public float maxHealth = 100f; // �ִ� HP
    private float currentHealth; // ���� HP
    

    // ���� ���� ����
    public float maxHunger = 100f;
    public float currentHunger;
    public Image hungerBarImage;

    public float maxThirst = 100f;
    public float currentThirst;
    public Image thirstBarImage;

    // ���� ������ �����ϴ� �ֱ�� ��
    //public float decreaseInterval = 5f; // 5�ʸ���
    public float decreaseAmount = 5f; // 5�� ����

    #endregion

    #region ������Ʈ hp��
    void UpdateHealthBar()
    {
        float fillAmount = currentHealth / maxHealth; // fillAmount ���
        healthBarImage.fillAmount = fillAmount; // Image�� fillAmount ������Ʈ
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage; // ��������ŭ ���� ü�� ����
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // ü���� 0 ���Ϸ� �������� �ʵ��� ��
        UpdateHealthBar(); // ü�¹� ������Ʈ

        if(currentHealth == 0)
        {
            Debug.Log("gameOver");
        }
    }
    #endregion

    public Text bedNearbyText; // ħ�� �ؽ�Ʈ ���� ����
    private Vector3 savedPosition;//���� ��ġ ����
    private Quaternion savedRotation; // ���� ȸ���� ������ ����

    void Start()
    {
        bedNearbyText.gameObject.SetActive(false);//ħ�� �ؽ�Ʈ �����

        tr = GetComponent<Transform>();
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        currentHealth = maxHealth; // ���� �� ���� HP�� �ִ�ġ�� ����

        
        // �ʱ� ���� ������ �ִ�ġ�� ����
        currentHunger = maxHunger;
        currentThirst = maxThirst;

        // �ڷ�ƾ ����
        StartCoroutine(DecreaseHungerAndThirst());

        
        StartCoroutine(DecreaseHealth());
        
    }

    void Update()
    {
        currTime += Time.deltaTime;//���� �ð�
        #region �̵� ����
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

#region ħ�� ��ȣ�ۿ�
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bed") && (currTime % 100) >= 70)
        {
            bedNearbyText.text = "'F'�� ���� ���� �� �� �ֽ��ϴ�."; // �ؽ�Ʈ ����
            bedNearbyText.gameObject.SetActive(true); // �ؽ�Ʈ ǥ��
            
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Bed") && Input.GetKeyDown(KeyCode.F))
        {
            savedPosition = transform.position; // ���� ������Ʈ�� ��ġ�� savedPosition�� ����
            savedRotation = transform.rotation; // ���� ������Ʈ�� ȸ���� savedRotation�� ����
            StartCoroutine(SleepRoutine());
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Bed"))
        {
            bedNearbyText.gameObject.SetActive(false);
            // �޽��� �����
        }
    }
    IEnumerator SleepRoutine()
    {

        bedNearbyText.text = "���� ��";
        // ����
        
        transform.position = new Vector3(172.064f, 22.448f, 269.675f);//�������� ��ġ
        transform.rotation = Quaternion.Euler(-85.971f, 248.9f, -334.937f); //�������� ����
        rb.isKinematic = true;//���������� ���� �̵� �� ȸ�� ����'^'
        yield return new WaitForSeconds(5); // 5�ʰ� ���

        // �����
        bedNearbyText.text = "�Ͼ���ϴ�";
        transform.position = savedPosition;// �ڱ� �� ��ġ�� �̵�
        transform.rotation = savedRotation;//�ڱ� �� ������ �̵�
        rb.isKinematic = false; // ���� ������ ���� �̵� �� ȸ�� ���

        yield return new WaitForSeconds(5);
        bedNearbyText.gameObject.SetActive(false);
    }
    #endregion



    IEnumerator DecreaseHungerAndThirst()
        {
            while (true)
            {
                yield return new WaitForSeconds(20);

                // ���� ���� ����
                currentHunger = Mathf.Max(currentHunger - decreaseAmount, 0);
                currentThirst = Mathf.Max(currentThirst - decreaseAmount, 0);

                // UI ������Ʈ
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
        // �浹�� ������Ʈ�� 'Beast' �±׸� ������ �ִ��� Ȯ��
        if (collision.gameObject.CompareTag("Beast"))
        {
            // ������� 10 ���ҽ�Ŵ
            TakeDamage(10f);

            Slow();

            // ������� 0 ������ ��� ó���� ���� (�ɼ�)
            if (currentHealth <= 0)
            {
                // ������� 0 ������ �� ������ ����
                Debug.Log("Player is out of health!");
            }
            if (currentHunger > 70 && currentHealth <= 100)
            {
                StartCoroutine(RecoverHealthAndHunger(10)); // �ڷ�ƾ ����
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
            yield return new WaitForSeconds(1); // 1�� ���
            currentHunger -= 1; // ��� ����
            currentHealth += 1; // ü�� ����

            UpdateHealthBar(); // ü�� �� ������Ʈ
            UpdateHungerBar(); // ��� �� ������Ʈ
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

        // Rigidbody�� �ӵ��� ���ӵ��� 0���� �����Ͽ� ����ϴ�.
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // 2�� ���� ��ٸ��ϴ�.
        yield return new WaitForSeconds(2);

        // ����� �ӵ��� ���ӵ��� �ٽ� �����մϴ�.
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
