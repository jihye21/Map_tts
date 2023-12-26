using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionController : MonoBehaviour
{
    private Collider targetItemCollider;  // 아이템의 Collider 정보 저장

    [SerializeField]
    private float range;  // 아이템 습득이 가능한 최대 거리

    private bool pickupActivated = false;  // 아이템 습득 가능할시 True 

    private RaycastHit hitInfo;  // 충돌체 정보 저장

    [SerializeField]
    private LayerMask layerMask;  // 특정 레이어를 가진 오브젝트에 대해서만 습득할 수 있어야 한다.

    [SerializeField]
    private Text actionText;  // 행동을 보여 줄 텍스트

    [SerializeField]
    private Inventory theInventory;

    void Update()
    {
        CheckItem();
        TryAction();
    }

    private void TryAction()
    {
        if (Input.GetKeyDown(KeyCode.F) && pickupActivated)
        {
            CheckItem();
            CanPickUp();
        }
    }

    private void CheckItem()
    {

        // Raycast 대신 아래 코드로 아이템 감지
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, range, layerMask);

        if (hitColliders.Length > 0)
        {
            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.CompareTag("Item"))
                {
                    targetItemCollider = hitCollider;
                    ItemInfoAppear(targetItemCollider.GetComponent<ItemPickUp>().item);
                    Debug.Log("Item Detected: " + targetItemCollider.name);
                    return;
                }
            }
        }
        else
        {
            targetItemCollider = null;
            ItemInfoDisappear();
        }



    }

    private void ItemInfoAppear(Item item)
    {
        
        pickupActivated = true;
        actionText.gameObject.SetActive(true);
        actionText.text = hitInfo.transform.GetComponent<ItemPickUp>().item.itemName + " 획득 " + "<color=yellow>" + "(F)" + "</color>";
    }

    private void ItemInfoDisappear()
    {
        pickupActivated = false;
        actionText.gameObject.SetActive(false);
    }

    private void CanPickUp()
    {
        if (targetItemCollider != null)
        {
            Item item = targetItemCollider.GetComponent<ItemPickUp>().item;
            Debug.Log(item.itemName + " 획득 했습니다.");
            theInventory.AcquireItem(item);
            Destroy(targetItemCollider.gameObject);
            ItemInfoDisappear();
        }
    }
}

