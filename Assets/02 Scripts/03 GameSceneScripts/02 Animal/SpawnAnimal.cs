using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnAnimal : MonoBehaviour
{
    #region �ͼ� ����
    public GameObject Tiger;
    public GameObject Gollila;
    public GameObject Bear;
    #endregion

    /*#region ���������� ����
    public GameObject a;
    public GameObject b;
    public GameObject c;
    public GameObject d;
    public GameObject e;
    public GameObject f;
    public GameObject g;
    public GameObject h;
    public GameObject i;
    public GameObject j;
    public GameObject k;
    #endregion

    #region �Ŀ� ����
    public GameObject Pig;
    public GameObject Cow;
    public GameObject Fish;
    public GameObject Apple;
    #endregion*/    

    private Vector3 spawnArea;

    void SpawnRandomAnimal()
    {
        // ���� ��ġ ����
        Vector3 randomPosition = new Vector3(
            Random.Range(-spawnArea.x / 2, spawnArea.x / 2),
            Random.Range(-spawnArea.y / 2, spawnArea.y / 2),
            Random.Range(-spawnArea.z / 2, spawnArea.z / 2)
        ) + transform.position; // ���� ������Ʈ�� ��ġ�� �������� ��


        // ���� �ν��Ͻ� ����
        //Instantiate(animalPrefab, randomPosition, Quaternion.identity);
    }

}
