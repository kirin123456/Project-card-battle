using UnityEngine;
using UnityEngine.UI;

public class MapNode : MonoBehaviour
{
    public int floor;          // �� ����� ��
    public int indexInFloor;   // �� �� �ε���
    public Button button;

    private void Awake()
    {
        if (button == null) button = GetComponent<Button>();
    }

    private void OnEnable()
    {
        // �湮�ߴ� ���� ��Ȱ��ȭ
        if (GameManager.instance != null && GameManager.instance.IsVisited(floor, indexInFloor))
        {
            SetInteractable(false);
        }
    }

    private void Start()
    {
        button.onClick.AddListener(OnNodeClicked);
    }

    void OnNodeClicked()
    {
        // MapController�� ���� ��/���� ���η� ���ͷ��� ������.
        MapController.instance.EnterNode(this);
    }

    public void SetInteractable(bool canClick)
    {
        if (button != null) button.interactable = canClick;
        // �ʿ�� ���� ����
        // var img = GetComponent<Image>(); if (img) img.color = canClick ? enabledColor : disabledColor;
    }
}

