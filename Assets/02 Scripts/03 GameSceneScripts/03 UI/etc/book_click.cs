using UnityEngine;
using UnityEngine.UI;

public class Book_Click : MonoBehaviour
{
    public GameObject button;
    public GameObject l_btn;
    public GameObject r_btn;
    public RawImage bookImage;
    public Texture[] textures;

    private int currentBookIndex = 0;
    private int clickCount = 0;

    void Start()
    {
        // �ؽ�ó �迭�� ����ִ��� Ȯ��
        if (textures != null && textures.Length > 0)
        {
            // �ʱ� �ؽ�ó ����
            bookImage.texture = textures[0];
            bookImage.gameObject.SetActive(false);
            l_btn.gameObject.SetActive(false);
            r_btn.gameObject.SetActive(false);
        }
        else
        {
            //Ȯ��
            Debug.LogError("Textures array is not properly initialized.");
        }
    }

    public void OnClick()
    {
        //Ȯ��
        Debug.Log("Button Clicked! Current Index: " + currentBookIndex);

        clickCount++;

        if (clickCount % 2 == 1)
        {
            bookImage.gameObject.SetActive(true);
            l_btn.gameObject.SetActive(true);
            r_btn.gameObject.SetActive(true);
        }
        else if(clickCount % 2 == 0)
        {
            bookImage.gameObject.SetActive(false);
            l_btn.gameObject.SetActive(false);
            r_btn.gameObject.SetActive(false);
        }
    }

    public void OnClick_r_btn()
    {
        currentBookIndex = (currentBookIndex + 1) % textures.Length;
        bookImage.texture = textures[currentBookIndex];
        //Ȯ��
        Debug.Log("Button Clicked! Current Index: " + currentBookIndex);
    }


    public void OnClick_l_btn()
    {
            currentBookIndex = (currentBookIndex - 1) % textures.Length;
            bookImage.texture = textures[currentBookIndex];
            //Ȯ��
            Debug.Log("Button Clicked! Current Index: " + currentBookIndex);
    }


}
