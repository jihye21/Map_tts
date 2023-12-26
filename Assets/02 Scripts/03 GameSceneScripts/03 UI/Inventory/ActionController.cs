using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionController : MonoBehaviour
{
    private Collider targetItemCollider;  // �������� Collider ���� ����

    [SerializeField]
    private float range;  // ������ ������ ������ �ִ� �Ÿ�

    private bool pickupActivated = false;  // ������ ���� �����ҽ� True 

    private RaycastHit hitInfo;  // �浹ü ���� ����

    [SerializeField]
    private LayerMask layerMask;  // Ư�� ���̾ ���� ������Ʈ�� ���ؼ��� ������ �� �־�� �Ѵ�.

    [SerializeField]
    private Text actionText;  // �ൿ�� ���� �� �ؽ�Ʈ

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

        // Raycast ��� �Ʒ� �ڵ�� ������ ����
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
        actionText.text = hitInfo.transform.GetComponent<ItemPickUp>().item.itemName + " ȹ�� " + "<color=yellow>" + "(F)" + "</color>";
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
            Debug.Log(item.itemName + " ȹ�� �߽��ϴ�.");
            theInventory.AcquireItem(item);
            Destroy(targetItemCollider.gameObject);
            ItemInfoDisappear();
        }
    }
}

