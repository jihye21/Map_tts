using UnityEngine;

public class ItemSlot : MonoBehaviour
{
    public Item item;  // ������ ������ ������ ���� (������ Ŭ������ �������� �̸�, ������, ���� ���� �����մϴ�)

    public void SetItem(Item newItem)
    {
        // �������� ���Կ� ��ġ�ϴ� �Լ�
        item = newItem;
        // ���⼭ UI ������Ʈ ���� �߰� �۾��� ������ �� �ֽ��ϴ�.
    }

    public void ClearSlot()
    {
        // ���� ����
        item = null;
        // ���⼭ UI ������Ʈ ���� �߰� �۾��� ������ �� �ֽ��ϴ�.
    }
}
