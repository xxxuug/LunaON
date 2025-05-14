using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Setting : UI_Base
{
    [Header("Common")] // 공통적인 요소
    public GameObject Panel; // 정보, 설정, 나가기 눌렀을 때 뜨는 패널
    public GameObject Button; // 정보, 설정, 나가기 눌렀을 때 뜨는 버튼
    public TMP_Text Content; // 정보, 설정, 나가기 눌렀을 때 뜨는 내용
    public Button BackButton; // 정보, 설정, 나가기 눌렀을 때 뜨는 뒤로가기 버튼
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
        // info 눌렀을 때 패널 위치 조정하기 위해서
        _rectTransform = Panel.GetComponent<RectTransform>();
        _backButtonTransform = BackButton.GetComponent<RectTransform>();
        _menu = GetComponent<UI_Menu>();

        // 시작할 때 패널, 버튼 비활성화
        Panel.SetActive(false);
        Button.SetActive(false);

        // 뒤로가기 버튼 클릭
        BackButton.onClick.AddListener(OnBackButtonClick);

        // Info 버튼 클릭
        InfoButton.onClick.AddListener(OnInfoButtonClick);
        SettingButton.onClick.AddListener(OnSettingButtonClick);
        ExitButton.onClick.AddListener(OnExitButtonClick);
    }

    // Info 버튼 클릭했을 때
    void OnInfoButtonClick()
    {
        Panel.SetActive(true); // 패널 활성화
        _menu.CloseMenuPanel();

        // y 위치 0 정중앙 조정
        PanelPosY(0);
        // height 크기 조정
        PanelHeight(_defaultHeight);
        // 뒤로가기 버튼 위치 조정
        BackButtonPosY(_defaultBackPosY);

        Content.text = "이 게임은 3D RPG 입니다.";
    }

    // 설정 버튼 클릭했을 때
    void OnSettingButtonClick()
    {
        Panel.SetActive(true);
        _menu.CloseMenuPanel();

        // y 위치 0 정중앙 조정
        PanelPosY(0);
        // height 크기 조정
        PanelHeight(800);
        // 뒤로가기 버튼 위치 조정
        BackButtonPosY(370);
    }

    // 나가기 버튼 클릭했을 때
    void OnExitButtonClick()
    {
        Panel.SetActive(true);
        Button.SetActive(true);
        _menu.CloseMenuPanel();

        // y 좌표 위치 조정
        PanelPosY(63.2f);
        // height 크기 조정
        PanelHeight(_defaultHeight);
        // 뒤로가기 버튼 위치 조정
        BackButtonPosY(_defaultBackPosY);

        Content.text = "정말 게임을 종료하시겠습니까?";
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

    // 뒤로가기 버튼 클릭했을 때
    void OnBackButtonClick()
    {
        Panel.SetActive(false);
        Button.SetActive(false);
    }
    
    // 패널 y좌표 위치 조정 함수
    void PanelPosY(float posY)
    {
        Vector2 pos = _rectTransform.anchoredPosition;
        pos.y = posY;
        _rectTransform.anchoredPosition = pos;
    }

    // 패널 높이 조정 함수
    void PanelHeight(float height)
    {
        Vector2 h = _rectTransform.sizeDelta;
        h.y = height;
        _rectTransform.sizeDelta = h;
    }

    // 뒤로가기 버튼 y좌표 조정 함수
    void BackButtonPosY(float posY)
    {
        Vector2 pos = _backButtonTransform.anchoredPosition;
        pos.y = posY;
        _backButtonTransform.anchoredPosition = pos;
    }
}
