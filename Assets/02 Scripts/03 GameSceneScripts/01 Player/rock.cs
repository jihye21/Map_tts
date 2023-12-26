using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rock : MonoBehaviour
{
    // ���� ȿ�� ��ƼŬ�� ������ ����
    public GameObject expEffect;

    // ������ ����� ���� ����
    public GameObject itemPrefab;

    // ������Ʈ�� ������ ����
    private Transform tr;
    private Rigidbody rb;
    // �Ѿ� ���� Ƚ���� ������ų ����
    private int hitCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        tr = GetComponent<Transform>();
        rb = GetComponent<Rigidbody>();
    }

    // �浹 �� �߻��ϴ� �ݹ� �Լ�
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
        // ���� ȿ�� ��ƼŬ ����
        GameObject exp = Instantiate(expEffect, tr.position, Quaternion.identity);
        // ���� ȿ�� ��ƼŬ 5�� �� ����
        Destroy(exp, 2.0f);
        // Rigidbody ������Ʈ�� mass�� 1.0���� �����ؼ� ���Ը� ������ ��
        rb.mass = 0.5f;
        // ���� �ڱ�ġ�� ���� ����
        rb.AddForce(Vector3.up * 500.0f);
        // 3�� �Ŀ� �巳�� ����
        Destroy(gameObject, 3.0f);

        if (itemPrefab != null)
        {
            // ������ ���� ��ġ�� �ణ ���� ����
            Vector3 spawnPosition = new Vector3(tr.position.x, 0.5f, tr.position.z);
            Instantiate(itemPrefab, spawnPosition, Quaternion.identity);
        }
    }
}
