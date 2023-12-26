/*
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;

public class News_AI : MonoBehaviour
{

    // GPT-3 API ��������Ʈ
    private string aiApiUrl = "https://api.openai.com/v1/engines/davinci/completions";


    // GPT-3 API Ű
    private string apiKey = "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";

    // AI���� ���� �ؽ�Ʈ
    private string promptText = "Tell me about the characteristics of panda";

    public void OnButtonClick()
    {
        StartCoroutine(SendRequestToAI(promptText));
    }
   

    IEnumerator SendRequestToAI(string prompt)
    {
        // JSON ������ ��û ������ ����
        string requestData = "{\"prompt\": \"" + prompt + "\", \"max_tokens\": 60}";
        byte[] bodyRaw = Encoding.UTF8.GetBytes(requestData);

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
*/