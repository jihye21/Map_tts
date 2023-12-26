using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public Transform quickSlot;  // QuickSlot UI 요소 (슬롯을 갖는 부분)
    public Transform inventory;  // 인벤토리 UI 요소

    private GameObject selectedSlot;  // 선택한 슬롯
    private GameObject heldItem;      // 들고 있는 아이템

    void Start()
    {
        selectedSlot = null;
        heldItem = null;
    }

    public void OnSlotClick(GameObject Qslot)
    {
        if (selectedSlot == null)
        {
            // 슬롯이 선택되지 않은 경우, 선택한 슬롯으로 설정
            selectedSlot = Qslot;
        }
        else
        {
            // 슬롯이 이미 선택된 경우, 아이템을 옮김
            SwapItems(Qslot);
        }
    }

    private void SwapItems(GameObject Qslot)
    {
        // 선택한 슬롯과 현재 슬롯의 이미지 및 아이템을 교환
        Image selectedImage = selectedSlot.GetComponent<Image>();
        Image slotImage = Qslot.GetComponent<Image>();

        Sprite tempSprite = selectedImage.sprite;
        selectedImage.sprite = slotImage.sprite;
        slotImage.sprite = tempSprite;

        // 아이템을 옮기고 슬롯 비우기
        Transform tempTransform = heldItem.transform.parent;  // 아이템의 부모를 저장
        heldItem.transform.SetParent(Qslot.transform, false); // 아이템을 새로운 슬롯에 배치
        Qslot.GetComponentInChildren<ItemSlot>().transform.SetParent(tempTransform, false); // 이전 슬롯에 아이템을 배치

        // 선택 해제
        selectedSlot = null;
    }
}
