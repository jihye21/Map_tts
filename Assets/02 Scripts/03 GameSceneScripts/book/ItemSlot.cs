using UnityEngine;

public class ItemSlot : MonoBehaviour
{
    public Item item;  // 아이템 정보를 저장할 변수 (아이템 클래스는 아이템의 이름, 아이콘, 유형 등을 저장합니다)

    public void SetItem(Item newItem)
    {
        // 아이템을 슬롯에 배치하는 함수
        item = newItem;
        // 여기서 UI 업데이트 등의 추가 작업을 수행할 수 있습니다.
    }

    public void ClearSlot()
    {
        // 슬롯 비우기
        item = null;
        // 여기서 UI 업데이트 등의 추가 작업을 수행할 수 있습니다.
    }
}
