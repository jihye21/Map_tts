using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Text;

public class CatchCamera : MonoBehaviour
{
    

    public GameObject cameraPanel; // Camera 패널에 대한 참조
    public GameObject animalObject; // 동물 오브젝트에 대한 참조
    public AudioSource audioSource; // 오디오 소스 컴포넌트에 대한 참조

    public GameObject player;
    public Transform playerTransform; // player의 transform을 저장할 변수

    public Text actionText;// text

    public List<string> animalNames = new List<string>(); // 동물 이름을 저장할 리스트
    public static CatchCamera Instance { get; private set; }

    private Camera mainCamera; // 메인 카메라에 대한 참조

    void Awake()
    {
        // 인스턴스 초기화
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            // 이미 인스턴스가 존재하면 현재 스크립트를 파괴
            Destroy(this.gameObject);
            return;
        }
    }

        void Start()
    {
        cameraPanel.SetActive(false);
        mainCamera = Camera.main; // 메인 카메라 할당
        
    }
    void Update()
    {
        // 'T' 키로 Camera 패널 토글
        if (Input.GetKeyDown(KeyCode.T))
        {
            cameraPanel.SetActive(!cameraPanel.activeSelf);
        }

        // 오른쪽 마우스 클릭 감지
        if (Input.GetMouseButtonDown(1) && cameraPanel.activeSelf)
        {
            foreach (GameObject animal in GameObject.FindGameObjectsWithTag("Animal"))
            {
               
                Renderer renderer = animal.GetComponent<Renderer>();
                // "player" 게임 오브젝트의 위치 가져오기
                

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
        // 지정된 시간만큼 대기
        yield return new WaitForSeconds(delay);

        string animalName = animal.name; // 동물의 이름 가져오기

        // 동물 이름을 리스트에 추가
        animalNames.Add(animalName);

        // 감지된 동물 오브젝트 비활성화
        animal.SetActive(false);

        /* 왜  모르겟지
        actionText.text = "포획에 성공하였습니다. SNS를 확인해보세요.";
        actionText.gameObject.SetActive(true); // 텍스트 표시
        */
        // 동물 이름을 AI에게 전송
        StartCoroutine(SendAnimalNameToAI(animalName));
    }

    IEnumerator SendAnimalNameToAI(string animalName)
    {
        Debug.Log(animalName);
            // GPT-3 API 엔드포인트
        string aiApiUrl = "https://api.openai.com/v1/engines/davinci/completions";


        // GPT-3 API 키
        string apiKey = "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";

        // AI에게 보낼 텍스트
        string text = "Tell me about the characteristics of" + animalName;


        // JSON 형식의 요청 본문을 생성
        string requestData = "{\"text\": \"" + text + "\", \"max_tokens\": 60}";
        byte[] bodyRaw = Encoding.UTF8.GetBytes(requestData);
        Debug.Log(requestData);

        // UnityWebRequest를 사용하여 요청을 생성
        using (UnityWebRequest request = new UnityWebRequest(aiApiUrl, "POST"))
        {
            request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", "Bearer " + apiKey);

            // 요청을 보내고 응답을 기다림
            yield return request.SendWebRequest();

            // 에러 체크
            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(request.error);
            }
            else
            {
                // AI의 응답을 처리
                string responseText = request.downloadHandler.text;
                Debug.Log(responseText);
            }
        }
    }

}
