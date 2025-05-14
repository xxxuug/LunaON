using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_Status : UI_Base
{
    [Header("�÷��̾� ����")]
    public TMP_Text Level;
    public Image HP;
    public TMP_Text HPText;
    public Image EXP;

    protected override void Initialize()
    {
        base.Initialize();
        DontDestroyOnLoad(gameObject);

        //HP = GetComponent<Image>();
        //EXP = GetComponent<Image>();

        //Debug.Log("UI���� GameManager �ؽ�: " + GameManager.Instance.GetHashCode());

        SceneManager.sceneLoaded += OnSceneLoaded;
        GameManager.Instance.OnPlayerInfoChanged += UIUpdate;
        UIUpdate();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (GameManager.Instance != null)
        {
            Debug.Log("Boss �� �ε� ����, GameManager�� �ٽ� ����!");
            GameManager.Instance.OnPlayerInfoChanged -= UIUpdate;
            GameManager.Instance.OnPlayerInfoChanged += UIUpdate;
            UIUpdate();
        }
    }

    void UIUpdate()
    {
        var playerInfo = GameManager.Instance.PlayerInfo;

        Level.text = $"{playerInfo.Level}";

        HP.fillAmount = (float)playerInfo.CurrentHp / playerInfo.MaxHp;
        HPText.text = $"{playerInfo.CurrentHp}";
        EXP.fillAmount = (float)playerInfo.CurrentExp / playerInfo.MaxExp;
    }

    void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnPlayerInfoChanged -= UIUpdate;
        }

        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
