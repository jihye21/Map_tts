/*
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;

public class News_AI : MonoBehaviour
{

    // GPT-3 API 엔드포인트
    private string aiApiUrl = "https://api.openai.com/v1/engines/davinci/completions";


    // GPT-3 API 키
    private string apiKey = "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";

    // AI에게 보낼 텍스트
    private string promptText = "Tell me about the characteristics of panda";

    public void OnButtonClick()
    {
        StartCoroutine(SendRequestToAI(promptText));
    }
   

    IEnumerator SendRequestToAI(string prompt)
    {
        // JSON 형식의 요청 본문을 생성
        string requestData = "{\"prompt\": \"" + prompt + "\", \"max_tokens\": 60}";
        byte[] bodyRaw = Encoding.UTF8.GetBytes(requestData);

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
*/