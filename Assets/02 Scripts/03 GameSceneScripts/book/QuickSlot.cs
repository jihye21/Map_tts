using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public Transform quickSlot;  // QuickSlot UI ��� (������ ���� �κ�)
    public Transform inventory;  // �κ��丮 UI ���

    private GameObject selectedSlot;  // ������ ����
    private GameObject heldItem;      // ��� �ִ� ������

    void Start()
    {
        selectedSlot = null;
        heldItem = null;
    }

    public void OnSlotClick(GameObject Qslot)
    {
        if (selectedSlot == null)
        {
            // ������ ���õ��� ���� ���, ������ �������� ����
            selectedSlot = Qslot;
        }
        else
        {
            // ������ �̹� ���õ� ���, �������� �ű�
            SwapItems(Qslot);
        }
    }

    private void SwapItems(GameObject Qslot)
    {
        // ������ ���԰� ���� ������ �̹��� �� �������� ��ȯ
        Image selectedImage = selectedSlot.GetComponent<Image>();
        Image slotImage = Qslot.GetComponent<Image>();

        Sprite tempSprite = selectedImage.sprite;
        selectedImage.sprite = slotImage.sprite;
        slotImage.sprite = tempSprite;

        // �������� �ű�� ���� ����
        Transform tempTransform = heldItem.transform.parent;  // �������� �θ� ����
        heldItem.transform.SetParent(Qslot.transform, false); // �������� ���ο� ���Կ� ��ġ
        Qslot.GetComponentInChildren<ItemSlot>().transform.SetParent(tempTransform, false); // ���� ���Կ� �������� ��ġ

        // ���� ����
        selectedSlot = null;
    }
}
