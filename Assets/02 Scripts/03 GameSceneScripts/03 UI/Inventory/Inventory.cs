using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    private Slot selectedSlot;  // 선택한 슬롯
    private Item heldItem;  // 들고 있는 아이템

    public static bool invectoryActivated = false;  // 인벤토리 활성화 여부. true가 되면 카메라 움직임과 다른 입력을 막을 것이다.

    [SerializeField]
    private GameObject go_InventoryBase; // Inventory_Base 이미지
    [SerializeField]
    private GameObject go_SlotsParent;  // Slot들의 부모인 Grid Setting 

    private Slot[] slots;  // 슬롯들 배열

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
                if (slots[i].item != null)  // null 이라면 slots[i].item.itemName 할 때 런타임 에러 나서
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

    // 슬롯 클릭 시 호출됨
    public void OnSlotClick(Slot clickedSlot)
    {
        if (selectedSlot == null)
        {
            // 슬롯이 선택되지 않은 경우, 선택한 슬롯으로 설정
            selectedSlot = clickedSlot;
            Debug.Log("click slot");
        }
        else
        {
            // 슬롯이 이미 선택된 경우, 아이템을 옮김
            SwapItems(clickedSlot);
        }
    }

    private void SwapItems(Slot clickedSlot)
    {
        // 선택한 슬롯과 현재 슬롯의 아이템을 교환
        Item itemToSwap = selectedSlot.GetItem();
        Item clickedItem = clickedSlot.GetItem();

        // 선택한 슬롯이 비어 있지 않으면
        if (itemToSwap != null)
        {
            // 현재 슬롯에 아이템 추가
            clickedSlot.AddItem(itemToSwap);
        }
        else
        {
            // 현재 슬롯이 비어 있으면
            selectedSlot = null;
        }

        // 선택한 슬롯에 아이템 추가
        selectedSlot.AddItem(clickedItem);

        // 선택 해제
        selectedSlot = null;
    }

}
