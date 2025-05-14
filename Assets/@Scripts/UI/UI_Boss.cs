using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_Boss : UI_Base
{
    public GameObject BossUI;
    public Button BossMenu;
    public Button BossIcon;
    public GameObject ClickOutline;
    public GameObject RedDragon;
    public Button Exit;
    public Button BossEnter;

    [Header("Effect")]
    public GameObject PotalEffect1;
    public GameObject PotalEffect2;
    public PlayerController Player;

    void Start()
    {
        // Common UI Canvas에서 BossMenu 찾기
        if (BossMenu == null)
        {
            var canvas = GameObject.Find("Common UI Canvas(Clone)");
            if (canvas != null )
            {
                Transform boss = canvas.transform.Find("MENU UI/Menu/Menu Icon/Middle Menu/Boss - Button");
                if (boss != null)
                {
                    BossMenu = boss.GetComponent<Button>();
                }

            }
        }

        BossUI.SetActive(false);

        BossMenu.onClick.AddListener(OnClickBossMenu);
        BossIcon.onClick.AddListener(OnClickBossIcon);
        Exit.onClick.AddListener(OnClickExitButton);
        BossEnter.onClick.AddListener(OnClickBossEnter);
    }

    void OnClickBossMenu()
    {
        BossUI.SetActive(true);
        ClickOutline.SetActive(false);
        RedDragon.SetActive(false);
    }

    void OnClickBossIcon()
    {
        ClickOutline.SetActive(true);
        RedDragon.SetActive(true);
    }

    void OnClickBossEnter()
    {
        BossUI.SetActive(false);
        ShowPotalEffect();
        StartCoroutine(EnterBossRooom());
    }

    void OnClickExitButton()
    {
        BossUI.SetActive(false);
    }

    void ShowPotalEffect()
    {
        Time.timeScale = 0;
        Instantiate(PotalEffect1, Player.transform.position, Quaternion.identity, Player.transform);
        Instantiate(PotalEffect2, Player.transform.position, Quaternion.identity, Player.transform);
    }

    IEnumerator EnterBossRooom()
    {
        yield return new WaitForSecondsRealtime(3f);
        SceneManager.LoadScene("Boss");
        Time.timeScale = 1;
    }
}
