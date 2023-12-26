using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rock : MonoBehaviour
{
    // 폭발 효과 파티클을 연결할 변수
    public GameObject expEffect;

    // 아이템 드랍을 위한 변수
    public GameObject itemPrefab;

    // 컴포넌트를 저장할 변수
    private Transform tr;
    private Rigidbody rb;
    // 총알 맞은 횟수를 누적시킬 변수
    private int hitCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        tr = GetComponent<Transform>();
        rb = GetComponent<Rigidbody>();
    }

    // 충돌 시 발생하는 콜백 함수
    void OnCollisionEnter(Collision coll)
    {
        if (coll.collider.CompareTag("BULLET"))
        {
            if(++hitCount == 3)
            {
                Exprock();
            }
        }
    }

    // Update is called once per frame
    void Exprock()
    {
        // 폭발 효과 파티클 생성
        GameObject exp = Instantiate(expEffect, tr.position, Quaternion.identity);
        // 폭발 효과 파티클 5초 후 제거
        Destroy(exp, 2.0f);
        // Rigidbody 컴포넌트의 mass를 1.0으로 수정해서 무게를 가볍게 함
        rb.mass = 0.5f;
        // 위로 솟구치는 힘을 가함
        rb.AddForce(Vector3.up * 500.0f);
        // 3초 후에 드럼통 제거
        Destroy(gameObject, 3.0f);

        if (itemPrefab != null)
        {
            // 아이템 생성 위치를 약간 높게 조정
            Vector3 spawnPosition = new Vector3(tr.position.x, 0.5f, tr.position.z);
            Instantiate(itemPrefab, spawnPosition, Quaternion.identity);
        }
    }
}
