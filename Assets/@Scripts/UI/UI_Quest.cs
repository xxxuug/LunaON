using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UI_Quest : UI_Base
{
    [Header("Äù½ºÆ® ÆË¾÷Ã¢")]
    public GameObject QuestUI;
    public TMP_Text QuestTitle;
    public TMP_Text QuestContent;
    public Button QuestCancelButton;
    public Button QuestExpansionButton;

    #region UI_Base
    private void Awake()
    {
        Initialize();

        int targetWIdth = 1920;
        int targetHeight = 1080;

        Screen.SetResolution(targetWIdth, targetHeight, false);
    }

    private void SetCanvas()
    {
        Canvas canvas = gameObject.GetOrAddComponent<Canvas>();
        if (canvas != null)
        {
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.overrideSorting = true;
        }
        CanvasScaler canvasScaler = gameObject.GetOrAddComponent<CanvasScaler>();
        if (canvasScaler != null)
        {
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = new Vector2(1920, 1080);
        }
    }
    #endregion

    protected override void Initialize()
    {
        base.Initialize();
        SetCanvas();

        QuestUI.SetActive(false);
        QuestExpansionButton.gameObject.SetActive(false);

        QuestCancelButton.onClick.RemoveAllListeners();
        QuestCancelButton.onClick.AddListener(OnQuestCancelButtonClick);
        QuestExpansionButton.onClick.RemoveAllListeners();
        QuestExpansionButton.onClick.AddListener(OnQuestExpansionButtonClick);
    }

    private void OnEnable()
    {
        QuestManager.Instance.OnQuestUpdate += UpdateQuestUI;
        UpdateQuestUI();
    }

    private void OnDisable()
    {
        QuestManager.Instance.OnQuestUpdate -= UpdateQuestUI;
    }

    public void ShowQuestNotice(int questId)
    {
        QuestUI.SetActive(true);
        QuestManager.Instance.AcceptQuest(questId);
        UpdateQuestUI();
    }

    void UpdateQuestUI()
    {
        var quest = QuestManager.Instance.GetCurrentQuest();

        if (quest == null)
        {
            QuestUI.SetActive(false);
            return;
        }

        QuestTitle.text = quest.Title;
        QuestContent.text = $"{quest.TargetEnemyName}   {quest.CurrentKillCount} / {quest.TargetKillCount}";
    }

    // Äù½ºÆ® ´Ý±â ¹öÆ° ´­·¶À» ¶§
    void OnQuestCancelButtonClick()
    {
        QuestUI.SetActive(false);
        QuestExpansionButton.gameObject.SetActive(true);
    }

    // Äù½ºÆ® È®Àå ¹öÆ° ´­·¶À» ¶§
    void OnQuestExpansionButtonClick()
    {
        ShowQuestNotice(QuestManager.Instance._currentQuestID);
    }
}
