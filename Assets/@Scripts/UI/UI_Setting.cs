using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Setting : UI_Base
{
    [Header("Common")] // �������� ���
    public GameObject Panel; // ����, ����, ������ ������ �� �ߴ� �г�
    public GameObject Button; // ����, ����, ������ ������ �� �ߴ� ��ư
    public TMP_Text Content; // ����, ����, ������ ������ �� �ߴ� ����
    public Button BackButton; // ����, ����, ������ ������ �� �ߴ� �ڷΰ��� ��ư
    private RectTransform _rectTransform;
    private RectTransform _backButtonTransform;
    private UI_Menu _menu;
    private float _defaultHeight = 200;
    private float _defaultBackPosY = 68;

    [Header("Info")]
    public Button InfoButton;

    [Header("Setting")]
    public Button SettingButton;

    [Header("Exit")]
    public Button ExitButton;
    public Button YesButton;
    public Button NoButton;

    void Start()
    {
        // info ������ �� �г� ��ġ �����ϱ� ���ؼ�
        _rectTransform = Panel.GetComponent<RectTransform>();
        _backButtonTransform = BackButton.GetComponent<RectTransform>();
        _menu = GetComponent<UI_Menu>();

        // ������ �� �г�, ��ư ��Ȱ��ȭ
        Panel.SetActive(false);
        Button.SetActive(false);

        // �ڷΰ��� ��ư Ŭ��
        BackButton.onClick.AddListener(OnBackButtonClick);

        // Info ��ư Ŭ��
        InfoButton.onClick.AddListener(OnInfoButtonClick);
        SettingButton.onClick.AddListener(OnSettingButtonClick);
        ExitButton.onClick.AddListener(OnExitButtonClick);
    }

    // Info ��ư Ŭ������ ��
    void OnInfoButtonClick()
    {
        Panel.SetActive(true); // �г� Ȱ��ȭ
        _menu.CloseMenuPanel();

        // y ��ġ 0 ���߾� ����
        PanelPosY(0);
        // height ũ�� ����
        PanelHeight(_defaultHeight);
        // �ڷΰ��� ��ư ��ġ ����
        BackButtonPosY(_defaultBackPosY);

        Content.text = "�� ������ 3D RPG �Դϴ�.";
    }

    // ���� ��ư Ŭ������ ��
    void OnSettingButtonClick()
    {
        Panel.SetActive(true);
        _menu.CloseMenuPanel();

        // y ��ġ 0 ���߾� ����
        PanelPosY(0);
        // height ũ�� ����
        PanelHeight(800);
        // �ڷΰ��� ��ư ��ġ ����
        BackButtonPosY(370);
    }

    // ������ ��ư Ŭ������ ��
    void OnExitButtonClick()
    {
        Panel.SetActive(true);
        Button.SetActive(true);
        _menu.CloseMenuPanel();

        // y ��ǥ ��ġ ����
        PanelPosY(63.2f);
        // height ũ�� ����
        PanelHeight(_defaultHeight);
        // �ڷΰ��� ��ư ��ġ ����
        BackButtonPosY(_defaultBackPosY);

        Content.text = "���� ������ �����Ͻðڽ��ϱ�?";
        YesButton.onClick.AddListener(OnYesButtonClick);
        NoButton.onClick.AddListener(OnNoButtonClick);
    }

    void OnYesButtonClick()
    {
        Application.Quit();
    }

    void OnNoButtonClick()
    {
        OnBackButtonClick();
    }

    // �ڷΰ��� ��ư Ŭ������ ��
    void OnBackButtonClick()
    {
        Panel.SetActive(false);
        Button.SetActive(false);
    }
    
    // �г� y��ǥ ��ġ ���� �Լ�
    void PanelPosY(float posY)
    {
        Vector2 pos = _rectTransform.anchoredPosition;
        pos.y = posY;
        _rectTransform.anchoredPosition = pos;
    }

    // �г� ���� ���� �Լ�
    void PanelHeight(float height)
    {
        Vector2 h = _rectTransform.sizeDelta;
        h.y = height;
        _rectTransform.sizeDelta = h;
    }

    // �ڷΰ��� ��ư y��ǥ ���� �Լ�
    void BackButtonPosY(float posY)
    {
        Vector2 pos = _backButtonTransform.anchoredPosition;
        pos.y = posY;
        _backButtonTransform.anchoredPosition = pos;
    }
}
