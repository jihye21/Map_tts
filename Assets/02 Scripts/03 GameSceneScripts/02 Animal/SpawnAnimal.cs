using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnAnimal : MonoBehaviour
{
    #region 맹수 변수
    public GameObject Tiger;
    public GameObject Gollila;
    public GameObject Bear;
    #endregion

    /*#region 멸종위기종 변수
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

    #region 식용 변수
    public GameObject Pig;
    public GameObject Cow;
    public GameObject Fish;
    public GameObject Apple;
    #endregion*/    

    private Vector3 spawnArea;

    void SpawnRandomAnimal()
    {
        // 랜덤 위치 생성
        Vector3 randomPosition = new Vector3(
            Random.Range(-spawnArea.x / 2, spawnArea.x / 2),
            Random.Range(-spawnArea.y / 2, spawnArea.y / 2),
            Random.Range(-spawnArea.z / 2, spawnArea.z / 2)
        ) + transform.position; // 현재 오브젝트의 위치를 기준으로 함


        // 동물 인스턴스 생성
        //Instantiate(animalPrefab, randomPosition, Quaternion.identity);
    }

}
