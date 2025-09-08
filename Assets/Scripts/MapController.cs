using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapController : MonoBehaviour
{
    public static MapController instance;

    [Header("Map Settings")]
    public int floors = 5;                 // �� ��(����) ��
    public int minNodesPerFloor = 2;       // ���� �ּ� ��� ��
    public int maxNodesPerFloor = 4;       // ���� �ִ� ��� ��
    public float xSpacing = 200f;          // ���� ����
    public float ySpacing = 250f;          // ���� ����(�� �� ����)

    [Header("Prefabs / References")]
    public GameObject nodePrefab;          // ��ư+MapNode ��ũ��Ʈ�� ���� ������
    public GameObject linePrefab;          // Image�� ���� ���� ���� ������
    public RectTransform mapParent;        // ���/������ ���� �θ�(ĵ���� ����)

    private readonly List<List<MapNode>> nodesByFloor = new List<List<MapNode>>();

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        // 0) �ʼ� ���� ���� ����
        if (mapParent == null) { Debug.LogError("[MapController] mapParent ���� �ʿ�"); return; }
        if (nodePrefab == null) { Debug.LogError("[MapController] nodePrefab ���� �ʿ�"); return; }
        if (linePrefab == null) { Debug.LogError("[MapController] linePrefab ���� �ʿ�"); return; }

        // 1) GameManager ����
        if (GameManager.instance == null)
        {
            var go = new GameObject("GameManager");
            go.AddComponent<GameManager>(); // Awake���� instance + DontDestroyOnLoad
        }
        var gm = GameManager.instance;
        gm.maxFloors = floors;
        gm.EnsureInit();

        // 2) �� ���� + ���ἱ ����
        GenerateMap();
        DrawConnections();

        // 3) ���� ������ ������ ��常 Ȱ��ȭ
        RefreshInteractable();
    }

    // -----------------------------
    // A) �� ���� (������ ��� ������ ����)
    // -----------------------------
    private void GenerateMap()
    {
        // ���� �ڽ� ����(����)
        for (int i = mapParent.childCount - 1; i >= 0; i--)
            Destroy(mapParent.GetChild(i).gameObject);

        nodesByFloor.Clear();

        for (int f = 0; f < floors; f++)
        {
            int count = Mathf.Clamp(Random.Range(minNodesPerFloor, maxNodesPerFloor + 1), 1, 99);
            var floorList = new List<MapNode>(count);

            for (int i = 0; i < count; i++)
            {
                var nodeObj = Instantiate(nodePrefab, mapParent);
                var rt = nodeObj.GetComponent<RectTransform>();

                float x = (i - (count - 1) * 0.5f) * xSpacing;
                float y = -f * ySpacing;
                rt.anchoredPosition = new Vector2(x, y);

                var node = nodeObj.GetComponent<MapNode>();
                node.floor = f;
                node.indexInFloor = i;

                floorList.Add(node);
            }

            nodesByFloor.Add(floorList);
        }
    }

    // ----------------------------------------
    // B) ���� ���ἱ �׸��� (�ܼ� ���� 1~2�� ����)
    // ----------------------------------------
    private void DrawConnections()
    {
        for (int f = 0; f < nodesByFloor.Count - 1; f++)
        {
            var upper = nodesByFloor[f];
            var lower = nodesByFloor[f + 1];

            foreach (var from in upper)
            {
                int connections = Random.Range(1, 3); // �� ��尡 ���� ���� 1~2�� ���� ����
                for (int c = 0; c < connections; c++)
                {
                    int toIdx = Random.Range(0, lower.Count);
                    var to = lower[toIdx];

                    DrawLine(from.GetComponent<RectTransform>(), to.GetComponent<RectTransform>());
                }
            }
        }
    }

    private void DrawLine(RectTransform a, RectTransform b)
    {
        var lineObj = Instantiate(linePrefab, mapParent);
        var rt = lineObj.GetComponent<RectTransform>();

        Vector2 pa = a.anchoredPosition;
        Vector2 pb = b.anchoredPosition;

        Vector2 mid = (pa + pb) * 0.5f;
        rt.anchoredPosition = mid;

        float length = Vector2.Distance(pa, pb);
        rt.sizeDelta = new Vector2(length, rt.sizeDelta.y);

        Vector2 dir = pb - pa;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        rt.rotation = Quaternion.Euler(0, 0, angle);
    }

    // -----------------------------------------------------
    // C) ��� ��ư Ȱ��ȭ ���� + �� �̵�(���� ����) ó��
    // -----------------------------------------------------
    public void RefreshInteractable()
    {
        var gm = GameManager.instance;
        int totalFloors = nodesByFloor.Count;

        // ��尡 ���� ������ ����
        if (totalFloors == 0) return;

        // ��� ��ư ���
        foreach (var floorList in nodesByFloor)
            foreach (var n in floorList)
                n.SetInteractable(false);

        // ��� �� �Ϸ� �� ��
        if (gm.IsRunFinished()) return;

        // ���� �� ���� üũ
        if (gm.currentFloor < 0 || gm.currentFloor >= totalFloors)
        {
            Debug.LogWarning($"[MapController] currentFloor {gm.currentFloor} out of range");
            return;
        }

        // ���� ���� ���(�̹� ��ü ��������Ƿ� ǥ�ø� �����ϰ� �ʹٸ� ���⼭ ó��)
        for (int f = 0; f < gm.currentFloor; f++)
        {
            var list = nodesByFloor[f];
            foreach (var n in list) n.SetInteractable(false);
        }

        // ���� ��: �湮���� ���� ��� ���� Ȱ��ȭ
        if (gm.currentFloor == 0)
        {
            foreach (var n in nodesByFloor[0])
                if (!gm.IsVisited(0, n.indexInFloor)) n.SetInteractable(true);

            return;
        }

        // �� �� ��:
        int prev = gm.currentFloor - 1;
        int prevChosen = gm.GetChosenNode(prev);

        // ���� ���� ������ ���� ����� ���ٸ�(������ ����) -> ���� �� ��ü Ȱ��ȭ(�湮�� �� ����)�� ����
        if (prevChosen < 0 || prevChosen >= nodesByFloor[prev].Count)
        {
            EnableWholeCurrentFloorIfSafe(gm.currentFloor);
            return;
        }

        // [���� ����] ���� ���̺��� �������� �ʴ� �����̹Ƿ�,
        // ���� �� ��ü�� �ĺ��� Ȱ��ȭ(�̹� �湮�� ���� ����)
        EnableWholeCurrentFloorIfSafe(gm.currentFloor);
    }

    private void EnableWholeCurrentFloorIfSafe(int floor)
    {
        var gm = GameManager.instance;
        if (floor < 0 || floor >= nodesByFloor.Count) return;

        foreach (var n in nodesByFloor[floor])
        {
            if (!gm.IsVisited(floor, n.indexInFloor))
                n.SetInteractable(true);
        }
    }

    // ��� Ŭ�� �� ȣ�� (MapNode���� ��ư Ŭ������ �����)
    public void EnterNode(MapNode node)
    {
        var gm = GameManager.instance;

        // ���� �� ��常 ���� ���
        if (node.floor != gm.currentFloor) return;

        // �̹� �湮�� ���� ����
        if (gm.IsVisited(node.floor, node.indexInFloor))
        {
            node.SetInteractable(false);
            return;
        }

        // ����/�湮 ��� ����
        gm.SaveChoice(node.floor, node.indexInFloor);

        // battle1~3 �� ���� ���� (�� �̸��� Build Settings�� ��Ȯ�� ���)
        string randomBattle = "Battle " + Random.Range(1, 4);
        SceneManager.LoadScene(randomBattle);
    }

    // ���� ������ MapScene���� �������� �� ȣ���ϰ� �ʹٸ� ���
    public void OnReturnedFromBattle()
    {
        RefreshInteractable();
    }
}
