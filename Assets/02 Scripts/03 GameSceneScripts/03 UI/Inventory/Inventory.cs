using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    private Slot selectedSlot;  // ������ ����
    private Item heldItem;  // ��� �ִ� ������

    public static bool invectoryActivated = false;  // �κ��丮 Ȱ��ȭ ����. true�� �Ǹ� ī�޶� �����Ӱ� �ٸ� �Է��� ���� ���̴�.

    [SerializeField]
    private GameObject go_InventoryBase; // Inventory_Base �̹���
    [SerializeField]
    private GameObject go_SlotsParent;  // Slot���� �θ��� Grid Setting 

    private Slot[] slots;  // ���Ե� �迭

    void Start()
    {
        slots = go_SlotsParent.GetComponentsInChildren<Slot>();

        selectedSlot = null;
        heldItem = null;
    }

    void Update()
    {
        TryOpenInventory();
    }

    private void TryOpenInventory()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            invectoryActivated = !invectoryActivated;

            if (invectoryActivated)
                OpenInventory();
            else
                CloseInventory();

        }
    }

    private void OpenInventory()
    {
        go_InventoryBase.SetActive(true);
    }

    private void CloseInventory()
    {
        go_InventoryBase.SetActive(false);
    }

    public void AcquireItem(Item _item, int _count = 1)
    {
        if (Item.ItemType.Equipment != _item.itemType)
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].item != null)  // null �̶�� slots[i].item.itemName �� �� ��Ÿ�� ���� ����
                {
                    if (slots[i].item.itemName == _item.itemName)
                    {
                        slots[i].SetSlotCount(_count);
                        return;
                    }
                }
            }
        }

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == null)
            {
                slots[i].AddItem(_item, _count);
                return;
            }
        }
    }

    // ���� Ŭ�� �� ȣ���
    public void OnSlotClick(Slot clickedSlot)
    {
        if (selectedSlot == null)
        {
            // ������ ���õ��� ���� ���, ������ �������� ����
            selectedSlot = clickedSlot;
            Debug.Log("click slot");
        }
        else
        {
            // ������ �̹� ���õ� ���, �������� �ű�
            SwapItems(clickedSlot);
        }
    }

    private void SwapItems(Slot clickedSlot)
    {
        // ������ ���԰� ���� ������ �������� ��ȯ
        Item itemToSwap = selectedSlot.GetItem();
        Item clickedItem = clickedSlot.GetItem();

        // ������ ������ ��� ���� ������
        if (itemToSwap != null)
        {
            // ���� ���Կ� ������ �߰�
            clickedSlot.AddItem(itemToSwap);
        }
        else
        {
            // ���� ������ ��� ������
            selectedSlot = null;
        }

        // ������ ���Կ� ������ �߰�
        selectedSlot.AddItem(clickedItem);

        // ���� ����
        selectedSlot = null;
    }

}
