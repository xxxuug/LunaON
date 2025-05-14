using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class DialogEntry
{
    //public string NPCName;
    //public int ID;
    public string Key;
    [TextArea]
    public string[] DialogContent;
}

public class TalkManager : Singleton<TalkManager>
{
    public Transform Player;
    public TMP_Text NPCText;
    public GameObject TalkUI;
    public bool IsTalking = false;

    public List<DialogEntry> DialogList;

    // ��ȭ ��ũ��Ʈ ����� ��ųʸ�
    private Dictionary<string, string[]> _dialogDict = new Dictionary<string, string[]>();
    // �� ��° ��ȭ���� Ȯ���ϴ� ��ȭ ȸ���� ���� ��ųʸ�
    private Dictionary<string, Dictionary<int, int>> _npcQuestDialogProgress = new();

    private string _nearNPC = "";
    public string NearNPC => _nearNPC;
    private bool _isTyping = false;
    private Coroutine typingCoroutine;
    private int _currentDialogPage = 0;
    private string _currentDialogKey = "";

    private PlayerController _playerController;

    protected override void Initialize()
    {
        base.Initialize();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Start()
    {
        if (NPCText == null) return;
        NPCText.text = "";
        // ������ �� ��Ȱ��ȭ
        TalkUI.SetActive(false);
        BuildDialogDict();

        //_playerController = Player.GetComponent<PlayerController>();
    }

    void Update()
    {
        // ����ȭ �ʿ� �� ���� �б�� ������ ������
        InteractionNPC();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        var playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            Player = playerObj.transform;
            _playerController = Player.GetComponent<PlayerController>();
        }
        else
        {
            Debug.LogWarning("Player not found in scene!");
        }
    }

    void BuildDialogDict()
    {
        //  �ߺ� NPC �̸��� ������ �� �ִ� ���� ID ü�赵 ����ϸ� ����.
        foreach (var entry in DialogList)
        {
            //_dialogDict[entry.NPCName] = entry.DialogContent;
            //string key = entry.NPCName + "_" + entry.ID;
            _dialogDict[entry.Key] = entry.DialogContent;
        }
    }

    public void InteractionNPC()
    {
        // ���� �ִ� ��� NPCDistance ������Ʈ ã��
        GameObject[] allNPCs = GameObject.FindGameObjectsWithTag("NPC");

        foreach (GameObject npc in allNPCs)
        {
            NPCDistance dist = npc.GetComponent<NPCDistance>();
            dist.Player = Player;

            if (dist.IsDistanceToPlayer())
            {
                _nearNPC = npc.name;

                // �� ������ ��� ������ ���¿��� �����̽��ٸ� ���� ���
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    if (QuestManager.Instance.BlockInput) return;

                    // ���� ���� ��ȭ���� �ƴ϶��
                    if (!IsTalking)
                    {
                        StartDialog();

                    }
                    // �̹� ��ȭ���̾��ٸ�
                    else
                    {
                        if (_isTyping)
                        {
                            StopCoroutine(typingCoroutine);
                            CompleteCurrentLine();
                        }
                        else
                        {
                            // ���� �� ���
                            ShowNextLine();
                        }
                    }
                }
                else if (Input.GetKeyDown(KeyCode.Escape))
                {
                    EndDialog();
                }
                break;
            }
        }
    }

    int GetDialogProgress(string npcName, int questId)
    {
        if (!_npcQuestDialogProgress.ContainsKey(npcName))
            _npcQuestDialogProgress[npcName] = new();

        if (!_npcQuestDialogProgress[npcName].ContainsKey(questId))
            _npcQuestDialogProgress[npcName][questId] = 0;

        return _npcQuestDialogProgress[npcName][questId];
    }

    public void SetDialogProgress(string npcName, int questId, int progress)
    {
        if (!_npcQuestDialogProgress.ContainsKey(npcName))
            _npcQuestDialogProgress[npcName] = new();

        _npcQuestDialogProgress[npcName][questId] = progress;
    }

    void StartDialog()
    {
        // �÷��̾� ��ó�� �ִ� _nearNPC�� �̸��� ���� GameObject�� ������ ã�Ƽ� QuestNPC ��ũ��Ʈ�� ������.
        GameObject npc = GameObject.Find(_nearNPC);
        var questNPC = npc?.GetComponent<QuestNPC>(); // ?�� NULL üũ. ������ NULL ��ȯ
        int currentNPCID = questNPC != null ? questNPC.NPCID : 0;

        if (questNPC == null)
        {
            TryStartFallbackDialog();
            return;
        }

        foreach (int id in questNPC.QuestID)
        {
            var quest = QuestManager.Instance.GetQuestByID(id);
            if (quest == null || QuestManager.Instance.IsCompleted(id)) continue;

            //int progress = GetDialogProgress(_nearNPC, id);
            //string key = _nearNPC + "_" + GetDialogProgress(_nearNPC, id);

            bool isAccepted = QuestManager.Instance.IsAccepted(id);
            bool isCompleted = QuestManager.Instance.IsCompleted(id);
            bool isRequireLevel = GameManager.Instance.PlayerInfo.Level >= quest.RequireLevel;

            if (!isAccepted && !isCompleted)
            {
                if (quest.StartNPCID == currentNPCID && isRequireLevel)
                {
                    SetDialogProgress(_nearNPC, id, 0);
                    _currentDialogKey = _nearNPC + "_" + id + "_0";
                    StartDialogFromKey(_currentDialogKey);
                    return;
                }
            }
            else if (isAccepted)
            {
                bool isComplete = (quest.TargetKillCount > 0)
                    ? quest.CurrentKillCount >= quest.TargetKillCount && isRequireLevel
                    : isRequireLevel;

                if (quest.EndNPCID == currentNPCID && isComplete)
                {
                    SetDialogProgress(_nearNPC, id, 2);
                    QuestManager.Instance.CompleteQuest(id);

                    if (quest.NextQuestID > 0)
                    {
                        QuestManager.Instance.AcceptQuest(quest.NextQuestID);
                    }

                    _currentDialogKey = _nearNPC + "_" + id + "_2";
                    StartDialogFromKey(_currentDialogKey);
                    return;
                }
                else if (quest.StartNPCID == currentNPCID || quest.EndNPCID == currentNPCID)
                {
                    SetDialogProgress(_nearNPC, id, 1);
                    _currentDialogKey = _nearNPC + "_" + id + "_1";
                    StartDialogFromKey(_currentDialogKey);
                    return;
                }
            }
        }
        TryStartFallbackDialog();
    }

    void TryStartFallbackDialog()
    {
        string fallbackKey = _nearNPC + "_99";
        if (_dialogDict.ContainsKey(fallbackKey))
        {
            _currentDialogKey = fallbackKey;
            StartDialogFromKey(fallbackKey);
        }
        else
        {
            Debug.LogWarning($"[��ȭ ����] Ű {fallbackKey} ����");
            EndDialog();
        }
    }
    //    string fallbackKey = _nearNPC + "_99";
    //    if (_dialogDict.ContainsKey(fallbackKey))
    //    {
    //        _currentDialogKey = fallbackKey;
    //        StartDialogFromKey(fallbackKey);
    //    }
    //    else
    //    {
    //        Debug.Log($"[��ȭ ����] Ű {fallbackKey} �� _dialogDict �� ����");
    //        EndDialog();
    //    }
    //}

    void StartDialogFromKey(string key)
    {
        Debug.Log($"[��� Ű üũ] ���� Ű : {key}");
        if (!_dialogDict.ContainsKey(key) || _dialogDict[key].Length == 0)
        {
            TryStartFallbackDialog();
            return;
        }

        //_currentDialogKey = key;
        IsTalking = true;
        NPCText.text = "";
        TalkUI.SetActive(true);

        _currentDialogPage = 0;
        typingCoroutine = StartCoroutine(TypingEffect(_dialogDict[key][_currentDialogPage]));
    }

    void ShowNextLine()
    {
        //string key = _nearNPC + "_" + _currentDialogPage;
        //var pages = _dialogDict.ContainsKey(key) ? _dialogDict[key] : null;

        //if (pages == null) return;

        if (!_dialogDict.ContainsKey(_currentDialogKey)) return;

        string[] lines = _dialogDict[_currentDialogKey];
        _currentDialogPage++;

        if (_currentDialogPage >= lines.Length)
        {
            GameObject npcObj = GameObject.Find(_nearNPC);
            QuestNPC questNPC = npcObj.GetComponent<QuestNPC>();

            if (questNPC != null && _currentDialogKey.EndsWith("_0"))
            {
                QuestManager.Instance.YesOrNo(questNPC.QuestID);
                return;
            }

            EndDialog();
        }
        else
        {
            NPCText.text = "";
            typingCoroutine = StartCoroutine(TypingEffect(lines[_currentDialogPage]));
        }
    }

    public void EndDialog()
    {
        IsTalking = false;
        TalkUI.SetActive(false);
        NPCText.text = "";
        _playerController.enabled = true;
    }

    //public void SetNPCProgress(string npcName, int progress)
    //{
    //    if (_npcDialogProgress.ContainsKey(npcName))
    //        _npcDialogProgress[npcName] = progress;
    //    else
    //        _npcDialogProgress.Add(npcName, progress);

    //}

    IEnumerator TypingEffect(string text)
    {
        _isTyping = true;
        NPCText.text = "";

        foreach (char letter in text.ToCharArray())
        {
            NPCText.text += letter;
            yield return new WaitForSeconds(0.05f);
        }

        _isTyping = false;
    }

    void CompleteCurrentLine()
    {
        //string key = _nearNPC + "_" + _currentDialogPage;
        //if (!_dialogDict.ContainsKey(key)) return;

        if (!_dialogDict.ContainsKey(_currentDialogKey)) return;

        string[] lines = _dialogDict[_currentDialogKey];

        if (_currentDialogPage >= lines.Length) return;

        string fullText = lines[_currentDialogPage];
        NPCText.text = fullText;
        _isTyping = false;
    }
}
