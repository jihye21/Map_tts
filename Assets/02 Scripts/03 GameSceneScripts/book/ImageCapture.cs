using System;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class ImageCapture : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            CaptureAndUpload();
            Debug.LogError("capture");
        }
    }

    void CaptureAndUpload()
    {
        StartCoroutine(UploadImage());
        Debug.LogError("start coroutine");
    }

    IEnumerator UploadImage()
    {
        string serverURL = "http://2223.savethetint.64bit.kr/upload-image.php";

        // Capture the screen as a texture
        Texture2D screenTexture = ScreenCapture.CaptureScreenshotAsTexture();

        // Convert the texture to PNG bytes
        byte[] imageData = screenTexture.EncodeToPNG();

        /*
        // Generate a unique filename based on timestamp
        string fileName = "screenshot_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".png";
        
        // Create a WWWForm
        WWWForm form = new WWWForm();
        // 파일 이름을 서버에 보내기
        form.AddField("fileName", fileName);
        */
        // Create a UnityWebRequest and set the method to POST
        UnityWebRequest www = UnityWebRequest.Post(serverURL, new WWWForm());

        // Add image data to the request
        www.SetRequestHeader("Content-Type", "image/png");
        www.uploadHandler = new UploadHandlerRaw(imageData);
        www.uploadHandler.contentType = "image/png";



        // Send the request and wait for the result
        yield return www.SendWebRequest();

        // Check for errors
        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Failed to upload image: " + www.error);
        }
        else
        {

            // Server response text contains the file path
            string serverResponse = www.downloadHandler.text;
            Debug.Log("Server response: " + serverResponse);

            // Server response code and length
            Debug.Log("Server response code: " + www.responseCode);
            Debug.Log("Server response text: " + www.downloadHandler.text);

            // Now you can use the downloaded data as needed
            Debug.Log("Image uploaded successfully!");



        }
    }
}
