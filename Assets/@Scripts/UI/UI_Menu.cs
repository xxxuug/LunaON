using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Menu : UI_Base
{
    [Header("Menu Icon")]
    public Button MenuIcon;
    private RectTransform _menuIconRectTransform;

    [Header("Menu")]
    public GameObject TotalMenu;
    public GameObject MenuPanel;
    private RectTransform _menuPanelRectTransform;
    private float _menuMaxWidth = 130;
    private float _duration = 0.5f;
    private bool _isOpen = false;

    [Header("Touch Out")]
    private GraphicRaycaster _raycaster;
    private EventSystem _eventSystem;

    void Start()
    {
        _menuIconRectTransform = MenuIcon.GetComponent<RectTransform>();
        _menuPanelRectTransform = MenuPanel.GetComponent<RectTransform>();

        _raycaster = FindAnyObjectByType<GraphicRaycaster>();
        _eventSystem = EventSystem.current;

        TotalMenu.SetActive(false);

        MenuIcon.onClick.AddListener(OnClickMenuIcon);
    }

    private void Update()
    {
        if (_isOpen && Input.GetMouseButtonDown(0))
        {
            if (ClickOutsidePanel(Input.mousePosition, MenuPanel))
                CloseMenuPanel();
        }
    }

    void OnClickMenuIcon()
    {
        _isOpen = true;

        _menuIconRectTransform.rotation = Quaternion.Euler(0, 0, 90);

        StopAllCoroutines();
        Invoke(nameof(OpenMenuPanel), 0.1f);
    }

    void OpenMenuPanel()
    {
        TotalMenu.SetActive(true);
        StartCoroutine(SlideMenuPanel(0, _menuMaxWidth));
    }

    public void CloseMenuPanel()
    {
        _isOpen = false;
        _menuIconRectTransform.rotation = Quaternion.identity;

        StopAllCoroutines();
        StartCoroutine(SlideMenuPanel(_menuMaxWidth, 0));
    }

    IEnumerator SlideMenuPanel(float from, float to)
    {
        float elapsed = 0; // 경과시간

        while (elapsed <= _duration)
        {
            float t = elapsed / _duration;
            float width = Mathf.Lerp(from, to, t);

            Vector2 size = _menuPanelRectTransform.sizeDelta;
            size.x = width;
            _menuPanelRectTransform.sizeDelta = size;

            elapsed += Time.deltaTime;
            yield return null;
        }

        Vector2 finalSize = _menuPanelRectTransform.sizeDelta;
        finalSize.x = to;
        _menuPanelRectTransform.sizeDelta = finalSize;

        if (to == 0)
            TotalMenu.SetActive(false);
    }

    bool ClickOutsidePanel(Vector2 pos, GameObject panel)
    {
        PointerEventData pointer = new PointerEventData(_eventSystem);
        pointer.position = pos;

        List<RaycastResult> results = new List<RaycastResult>();
        _raycaster.Raycast(pointer, results);

        foreach (var result in results)
        {
            if (result.gameObject == panel || result.gameObject.transform.IsChildOf(panel.transform))
                return false;
        }
        return true;
    }
}
