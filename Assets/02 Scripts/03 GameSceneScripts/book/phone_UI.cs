using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class phone_UI : MonoBehaviour
{
    public Button questButton; // ����Ʈ ������
    public GameObject phone;  //����Ʈ�� �̹���
    public Button instagram; //instagram
    public Button home; //home img
    public GameObject questPopup;
    public GameObject instaPopup;
    public GameObject feedPopup;
    public GameObject profilePopup;
    private int clickCount = 0;
    private int questCount = 0;
    private int instaCount = 0;
    public GameObject back;
    public Button send;
    public Button breeding;

    public GameObject[] animalObjects; // Ȱ��ȭ�� ���� ������Ʈ �迭

    void Start()
    {
        // ���� �� Ÿ�� ��ư�� ����ϴ�.
        questButton.gameObject.SetActive(false);
        phone.SetActive(false);
        questPopup.SetActive(false);
        instagram.gameObject.SetActive(false);
        home.gameObject.SetActive(true);
        feedPopup.SetActive(false);
        profilePopup.SetActive(false);
        back.SetActive(false);

        // CatchCamera ��ũ��Ʈ�� animalNames ����Ʈ�� Ȯ���ϰ�, �ش� �̸��� ������ ������Ʈ Ȱ��ȭ
        foreach (string animalNames in CatchCamera.Instance.animalNames)
        {
            foreach (GameObject animal in animalObjects)
            {
                if (animal.name == animalNames)
                {
                    animal.SetActive(false);
                }
            }
        }
    }

    // ��ư Ŭ�� �� ȣ��� �޼���
    public void OnButtonClick()
    {
        clickCount++;

        if (clickCount % 2 == 1)
        {
            questButton.gameObject.SetActive(true);
            phone.SetActive(true);
            instagram.gameObject.SetActive(true);
            
            feedPopup.SetActive(false);
            profilePopup.SetActive(false);
        }
        else if (clickCount % 2 == 0)
        {
            questButton.gameObject.SetActive(false);
            phone.SetActive(false);
            instagram.gameObject.SetActive(false);
           
            feedPopup.SetActive(false);
            profilePopup.SetActive(false);
        }
       
    }
    public void OnQuestClick()
    {
            questPopup.SetActive(true);
            questButton.gameObject.SetActive(false);
            instagram.gameObject.SetActive(false);
    }

    public void OnInstaClick()
    {
        
        instaPopup.SetActive(true);
        feedPopup.SetActive(true);
        profilePopup.SetActive(false);

        foreach (string animalNames in CatchCamera.Instance.animalNames)
        {
            Debug.Log("animalNames");
            // animalName�� ��ġ�ϴ� ������Ʈ�� ã���ϴ�.
            GameObject matchingAnimal = System.Array.Find(animalObjects, animal => animal.name == animalNames);

            if (matchingAnimal != null)
            {
                // animalName�� ��ġ�ϴ� ������Ʈ�� ���� ��� Ȱ��ȭ�մϴ�.
                matchingAnimal.SetActive(true);
                Debug.Log("matchingAnimal active: " + animalNames+ matchingAnimal.activeSelf);
            }

        }

        // animalNames�� ���� ��� ������Ʈ�� ��Ȱ��ȭ�մϴ�.
        foreach (GameObject animal in animalObjects)
        {
            if (!CatchCamera.Instance.animalNames.Contains(animal.name))
            {
                Debug.Log("false"+ animal);
                animal.SetActive(false);
            }
        }
    }
    /*
    public void ActivateAnimals()
    {
        
        
        // CatchCamera ��ũ��Ʈ�� animalNames ����Ʈ�� Ȯ���ϰ�, �ش� �̸��� ������ ������Ʈ Ȱ��ȭ
        foreach (string animalNames in CatchCamera.Instance.animalNames)
        {
            Debug.Log("animalNames");
            GameObject matchingAnimal = System.Array.Find(animalObjects, animal => animal.name == animalNames);
            if (matchingAnimal != null)
            {
                Debug.Log(matchingAnimal);
                matchingAnimal.SetActive(true);
                Debug.Log("matchingAnimal active: " + matchingAnimal.activeSelf);

            }
        }
        
    }*/


    public void OnProfile1Click()
    {
        back.SetActive(true);
        instaPopup.SetActive(true);
        feedPopup.SetActive(false);
        profilePopup.SetActive(true);
    }

    public void OnBackClick()
    {
        instaPopup.SetActive(true);
        feedPopup.SetActive(true);
        profilePopup.SetActive(true);
    }

    public void OnHomeClick()
    {
        questButton.gameObject.SetActive(true);
        phone.SetActive(true);
        instagram.gameObject.SetActive(true);
        questPopup.SetActive(false);
        feedPopup.SetActive(false);
        profilePopup.SetActive(false);

    }


}
