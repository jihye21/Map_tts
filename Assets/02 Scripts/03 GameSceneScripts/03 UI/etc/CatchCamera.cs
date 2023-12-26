using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Text;

public class CatchCamera : MonoBehaviour
{
    

    public GameObject cameraPanel; // Camera �гο� ���� ����
    public GameObject animalObject; // ���� ������Ʈ�� ���� ����
    public AudioSource audioSource; // ����� �ҽ� ������Ʈ�� ���� ����

    public GameObject player;
    public Transform playerTransform; // player�� transform�� ������ ����

    public Text actionText;// text

    public List<string> animalNames = new List<string>(); // ���� �̸��� ������ ����Ʈ
    public static CatchCamera Instance { get; private set; }

    private Camera mainCamera; // ���� ī�޶� ���� ����

    void Awake()
    {
        // �ν��Ͻ� �ʱ�ȭ
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            // �̹� �ν��Ͻ��� �����ϸ� ���� ��ũ��Ʈ�� �ı�
            Destroy(this.gameObject);
            return;
        }
    }

        void Start()
    {
        cameraPanel.SetActive(false);
        mainCamera = Camera.main; // ���� ī�޶� �Ҵ�
        
    }
    void Update()
    {
        // 'T' Ű�� Camera �г� ���
        if (Input.GetKeyDown(KeyCode.T))
        {
            cameraPanel.SetActive(!cameraPanel.activeSelf);
        }

        // ������ ���콺 Ŭ�� ����
        if (Input.GetMouseButtonDown(1) && cameraPanel.activeSelf)
        {
            foreach (GameObject animal in GameObject.FindGameObjectsWithTag("Animal"))
            {
               
                Renderer renderer = animal.GetComponent<Renderer>();
                // "player" ���� ������Ʈ�� ��ġ ��������
                

                float distanceToPlayer = Vector3.Distance(player.transform.position, animal.transform.position);

                if (renderer != null && distanceToPlayer <= 5)
                {
                    
                        Debug.Log(animal.name );
                        audioSource.Play();
                        StartCoroutine(DisappearAfterDelay(animal, 1.0f));
                    
                }
            }
           
            
        }

    }


    IEnumerator DisappearAfterDelay(GameObject animal, float delay)
    {
        // ������ �ð���ŭ ���
        yield return new WaitForSeconds(delay);

        string animalName = animal.name; // ������ �̸� ��������

        // ���� �̸��� ����Ʈ�� �߰�
        animalNames.Add(animalName);

        // ������ ���� ������Ʈ ��Ȱ��ȭ
        animal.SetActive(false);

        /* ��  �𸣰���
        actionText.text = "��ȹ�� �����Ͽ����ϴ�. SNS�� Ȯ���غ�����.";
        actionText.gameObject.SetActive(true); // �ؽ�Ʈ ǥ��
        */
        // ���� �̸��� AI���� ����
        StartCoroutine(SendAnimalNameToAI(animalName));
    }

    IEnumerator SendAnimalNameToAI(string animalName)
    {
        Debug.Log(animalName);
            // GPT-3 API ��������Ʈ
        string aiApiUrl = "https://api.openai.com/v1/engines/davinci/completions";


        // GPT-3 API Ű
        string apiKey = "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";

        // AI���� ���� �ؽ�Ʈ
        string text = "Tell me about the characteristics of" + animalName;


        // JSON ������ ��û ������ ����
        string requestData = "{\"text\": \"" + text + "\", \"max_tokens\": 60}";
        byte[] bodyRaw = Encoding.UTF8.GetBytes(requestData);
        Debug.Log(requestData);

        // UnityWebRequest�� ����Ͽ� ��û�� ����
        using (UnityWebRequest request = new UnityWebRequest(aiApiUrl, "POST"))
        {
            request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", "Bearer " + apiKey);

            // ��û�� ������ ������ ��ٸ�
            yield return request.SendWebRequest();

            // ���� üũ
            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(request.error);
            }
            else
            {
                // AI�� ������ ó��
                string responseText = request.downloadHandler.text;
                Debug.Log(responseText);
            }
        }
    }

}
