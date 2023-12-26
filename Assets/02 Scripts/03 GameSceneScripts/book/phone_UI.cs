using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class phone_UI : MonoBehaviour
{
    public Button questButton; // 퀘스트 아이콘
    public GameObject phone;  //스마트폰 이미지
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

    public GameObject[] animalObjects; // 활성화할 동물 오브젝트 배열

    void Start()
    {
        // 시작 시 타겟 버튼을 숨깁니다.
        questButton.gameObject.SetActive(false);
        phone.SetActive(false);
        questPopup.SetActive(false);
        instagram.gameObject.SetActive(false);
        home.gameObject.SetActive(true);
        feedPopup.SetActive(false);
        profilePopup.SetActive(false);
        back.SetActive(false);

        // CatchCamera 스크립트의 animalNames 리스트를 확인하고, 해당 이름과 동일한 오브젝트 활성화
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

    // 버튼 클릭 시 호출될 메서드
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
            // animalName과 일치하는 오브젝트를 찾습니다.
            GameObject matchingAnimal = System.Array.Find(animalObjects, animal => animal.name == animalNames);

            if (matchingAnimal != null)
            {
                // animalName과 일치하는 오브젝트가 있을 경우 활성화합니다.
                matchingAnimal.SetActive(true);
                Debug.Log("matchingAnimal active: " + animalNames+ matchingAnimal.activeSelf);
            }

        }

        // animalNames에 없는 모든 오브젝트를 비활성화합니다.
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
        
        
        // CatchCamera 스크립트의 animalNames 리스트를 확인하고, 해당 이름과 동일한 오브젝트 활성화
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
