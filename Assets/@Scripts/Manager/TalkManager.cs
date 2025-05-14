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

    // 대화 스크립트 저장용 딕셔너리
    private Dictionary<string, string[]> _dialogDict = new Dictionary<string, string[]>();
    // 몇 번째 대화인지 확인하는 대화 회차별 저장 딕셔너리
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
        // 시작할 때 비활성화
        TalkUI.SetActive(false);
        BuildDialogDict();

        //_playerController = Player.GetComponent<PlayerController>();
    }

    void Update()
    {
        // 최적화 필요 시 일정 분기로 나눠도 괜찮음
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
        //  중복 NPC 이름을 방지할 수 있는 유일 ID 체계도 고려하면 좋음.
        foreach (var entry in DialogList)
        {
            //_dialogDict[entry.NPCName] = entry.DialogContent;
            //string key = entry.NPCName + "_" + entry.ID;
            _dialogDict[entry.Key] = entry.DialogContent;
        }
    }

    public void InteractionNPC()
    {
        // 씬에 있는 모든 NPCDistance 오브젝트 찾기
        GameObject[] allNPCs = GameObject.FindGameObjectsWithTag("NPC");

        foreach (GameObject npc in allNPCs)
        {
            NPCDistance dist = npc.GetComponent<NPCDistance>();
            dist.Player = Player;

            if (dist.IsDistanceToPlayer())
            {
                _nearNPC = npc.name;

                // 이 조건을 모두 충족한 상태에서 스페이스바를 누를 경우
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    if (QuestManager.Instance.BlockInput) return;

                    // 만약 현재 대화중이 아니라면
                    if (!IsTalking)
                    {
                        StartDialog();

                    }
                    // 이미 대화중이었다면
                    else
                    {
                        if (_isTyping)
                        {
                            StopCoroutine(typingCoroutine);
                            CompleteCurrentLine();
                        }
                        else
                        {
                            // 다음 줄 출력
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
        // 플레이어 근처에 있는 _nearNPC의 이름을 가진 GameObject를 씬에서 찾아서 QuestNPC 스크립트를 가져옴.
        GameObject npc = GameObject.Find(_nearNPC);
        var questNPC = npc?.GetComponent<QuestNPC>(); // ?는 NULL 체크. 없으면 NULL 반환
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
            Debug.LogWarning($"[대화 실패] 키 {fallbackKey} 없음");
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
    //        Debug.Log($"[대화 실패] 키 {fallbackKey} 가 _dialogDict 에 없음");
    //        EndDialog();
    //    }
    //}

    void StartDialogFromKey(string key)
    {
        Debug.Log($"[대사 키 체크] 현재 키 : {key}");
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
