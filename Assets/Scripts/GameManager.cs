using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public int currentFloor = 0;   // ������ ������ ��
    public int maxFloors = 5;      // �� ���� ��

    // ������ � ��带 �����ߴ��� (-1=����)
    public List<int> chosenNodePerFloor = new List<int>();

    // �湮�� ��� ����: "floor:index" ���ڿ��� Ű�� ����ϴ�.
    private HashSet<string> visitedNodeKeys = new HashSet<string>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void EnsureInit()
    {
        if (chosenNodePerFloor.Count != maxFloors)
        {
            chosenNodePerFloor.Clear();
            for (int i = 0; i < maxFloors; i++) chosenNodePerFloor.Add(-1);
        }
    }

    public void AdvanceFloor()
    {
        currentFloor = Mathf.Min(currentFloor + 1, maxFloors);
    }

    public bool IsRunFinished()
    {
        return currentFloor >= maxFloors;
    }

    public void SaveChoice(int floor, int indexInFloor)
    {
        EnsureInit();
        if (floor >= 0 && floor < chosenNodePerFloor.Count)
        {
            chosenNodePerFloor[floor] = indexInFloor;
        }
        MarkVisited(floor, indexInFloor);
    }

    public int GetChosenNode(int floor)
    {
        EnsureInit();
        if (floor < 0 || floor >= chosenNodePerFloor.Count) return -1;
        return chosenNodePerFloor[floor];
    }

    public void MarkVisited(int floor, int indexInFloor)
    {
        visitedNodeKeys.Add($"{floor}:{indexInFloor}");
    }

    public bool IsVisited(int floor, int indexInFloor)
    {
        return visitedNodeKeys.Contains($"{floor}:{indexInFloor}");
    }
}
