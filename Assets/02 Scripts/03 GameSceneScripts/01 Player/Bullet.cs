using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float damage = 20.0f;    // 총알의 파괴력
    public float force = 1500.0f;   // 총알 발사 힘
    private Rigidbody rb;           // Rigidbidy 컴포넌트

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // 총알의 전진 방향으로 힘(Force)을 가한다.
        rb.AddForce(transform.forward * force);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
