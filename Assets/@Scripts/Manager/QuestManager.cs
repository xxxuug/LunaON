using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class QuestData
{
    public int ID;
    public string Title;
    public string Content;
    public int RequireLevel;

    public string TargetEnemyName;
    public int TargetKillCount;
    public int CurrentKillCount;

    public float RewardExp;
    public int RewardGold;

    public int StartNPCID;
    public int EndNPCID;
    public int NextQuestID = -1;
}

public class QuestManager : Singleton<QuestManager>
{
    [Header("UI QUEST")]
    public UI_Quest UIQuest;

    [Header("����/����")]
    public GameObject ResponseUI;
    public Button Yes;
    public Button No;
    public bool BlockInput = false;

    [Header("����Ʈ ������")]
    public List<QuestData> QuestDataList = new List<QuestData>();
    public int _currentQuestID = -1;
    public event Action OnQuestUpdate;

    private List<int> _activeQuestID = new();
    private List<int> _completeQuestID = new();

    public QuestData GetQuestByID(int id) => QuestDataList.Find(q => q.ID == id);
    public bool IsAccepted(int id) => _activeQuestID.Contains(id);
    public bool IsCompleted(int id) => _completeQuestID.Contains(id);

    private void Start()
    {
        ResponseUI.SetActive(false);

        Yes.onClick.RemoveAllListeners();
        Yes.onClick.AddListener(InputYes);

        No.onClick.RemoveAllListeners();
        No.onClick.AddListener(InputNo);
    }

    #region Yes Or No
    public void YesOrNo(List<int> questIds)
    {
        foreach (int id in questIds)
        {
            if (!IsAccepted(id) && !IsCompleted(id))
            {
                _currentQuestID = id;
                BlockandCursor();
                ResponseUI.SetActive(true);
                return;
            }
        }
    }

    public void InputYes()
    {
        var quest = GetQuestByID(_currentQuestID);
        if (quest != null)
        {
            TalkManager.Instance.SetDialogProgress(quest.StartNPCID.ToString(), quest.ID, 1);
        }

        TalkManager.Instance.EndDialog();
        OriginalStatus();
        ResponseUI.SetActive(false);

        UIQuest.ShowQuestNotice(_currentQuestID);

        Debug.Log("����Ʈ ����!");
    }

    public void InputNo()
    {
        var quest = GetQuestByID(_currentQuestID);
        if (quest != null)
        {
            TalkManager.Instance.SetDialogProgress(quest.StartNPCID.ToString(), quest.ID, 0);
        }

        TalkManager.Instance.EndDialog();
        OriginalStatus();
        ResponseUI.SetActive(false);
        Debug.Log("����Ʈ ����!");
    }

    // �Է� ���� Ŀ�� ���̰�
    void BlockandCursor()
    {
        BlockInput = true;
        GameManager.Instance.UnlockCursor();
    }

    // �Է� Ǯ�� Ŀ�� �� ���̰� -> ���� ���´��
    void OriginalStatus()
    {
        BlockInput = false;
        GameManager.Instance.LockCursor();
    }
    #endregion

    // ����Ʈ Ÿ�� ų �Լ�
    public void AddKillCount(string enemyName)
    {
        foreach (var id in _activeQuestID)
        {
            var quest = GetQuestByID(id);

            if (quest == null)
            {
                Debug.LogWarning($"[AddKillCount] ID {id} �� �ش��ϴ� ����Ʈ�� QuestDataList�� �����ϴ�.");
                continue;
            }

            if (quest.TargetEnemyName != enemyName) continue;

            quest.CurrentKillCount++;
            Debug.Log($"[����Ʈ ����] {quest.Title} : {quest.CurrentKillCount}/{quest.TargetKillCount}");
            OnQuestUpdate?.Invoke();
        }
    }

    public void AcceptQuest(int id)
    {
        if (!_activeQuestID.Contains(id))
            _activeQuestID.Add(id);

        OnQuestUpdate?.Invoke();
    }

    public void CompleteQuest(int id)
    {
        if (_activeQuestID.Contains(id))
            _activeQuestID.Remove(id);
        if (!_completeQuestID.Contains(id))
            _completeQuestID.Add(id);

        var quest = GetQuestByID(id);
        GameManager.Instance.AddExp(quest.RewardExp);
        GameManager.Instance.PlayerInfo.Gold += quest.RewardGold;

        if (quest.NextQuestID != -1)
        {
            // ���� ����Ʈ �ڵ� ����
            AcceptQuest(quest.NextQuestID);
            _currentQuestID = quest.NextQuestID;
        }
        else
        {
            _currentQuestID = -1; // ���̻� ����Ʈ ������ ���� �ʱ�ȭ
        }

        OnQuestUpdate?.Invoke();
        Debug.Log($"[ {quest.Title} ] ����Ʈ �Ϸ�! ���� : + {quest.RewardGold} G, + EXP {quest.RewardExp}");

    }

    public QuestData GetCurrentQuest()
    {
        return QuestDataList.Find(q => q.ID == _currentQuestID);
    }
}
