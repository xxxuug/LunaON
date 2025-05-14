using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BossRoom : MonoBehaviour
{
    [Header("Object")]
    public Transform LeftGate;
    public Transform RightGate;
    public float duration = 1.5f;

    [Header("Camera")]
    public Camera ShowCamera;
    private Camera _playerCam;
    private PlayerController _playerController;
    private int _playTime = 1;

    [Header("Show")]
    public TMP_Text RedDragonTitle;
    public DragonController DragonController;
    public Button ExitButton;

    [Header("Boss Status")]
    public GameObject HP;
    public Image HpBar;

    void Start()
    {
        // 문 열리는 연출
        StartCoroutine(OpenDoor());

        // 카메라 참조
        GameObject player = GameObject.FindWithTag("Player");
        _playerCam = player.GetComponentInChildren<Camera>();
        _playerController = player.GetComponent<PlayerController>();

        // 글씨 없애기
        RedDragonTitle.gameObject.SetActive(false);
        // HP바 없애기
        HP.SetActive(false);
        // 나가기 버튼 클릭
        ExitButton.onClick.AddListener(OnClickExitButton);
    }

    private void Update()
    {
        if (DragonController != null && HP.activeSelf)
        {
            float hp = DragonController.CurrentHP / DragonController.MaxHP;
            HpBar.fillAmount = hp;
        }
    }

    private IEnumerator OpenDoor()
    {
        //Debug.Log("코루틴 시작");

        float elapsed = 0f;

        Quaternion leftStart = LeftGate.rotation;
        Quaternion leftEnd = Quaternion.Euler(0, -101, 0);

        Quaternion rightStart = RightGate.rotation;
        Quaternion rightEnd = Quaternion.Euler(0, 101, 0);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            LeftGate.rotation = Quaternion.Slerp(leftStart, leftEnd, t);
            RightGate.rotation = Quaternion.Slerp(rightStart, rightEnd, t);

            yield return null;
        }

        LeftGate.rotation = leftEnd;
        RightGate.rotation = rightEnd;

        //Debug.Log("문 열림 완료");
    }

    void ShowDragonBossEnterScene()
    {
        // 플레이어가 드래곤과의 거리가 50일 때
        // 카메라 좌표 x = -157.5, y = 5, z = 22 고정시킨 후
        // 카메라 좌표 x = -157.5, y = 10, z = 45 로 가면서 줌인 효과
        // 일정시간 후 다시 플레이어 원래 카메라 좌표로 돌아오기
        _playerCam.enabled = false;
        ShowCamera.enabled = true;

        StartCoroutine(ZoomInBoss());
    }

    IEnumerator TypingBossTitleEffect(string message, float speed = 0.05f)
    {
        RedDragonTitle.text = "";
        foreach (char c in message)
        {
            RedDragonTitle.text += c;
            yield return new WaitForSeconds(speed);
        }
    }

    IEnumerator ZoomInBoss()
    {
        _playerController.enabled = false;
        Vector3 startPos = ShowCamera.transform.position; // -157.5, 5, 22
        Vector3 targetPos = new Vector3(-157.5f, 10, 45);

        float duration = 2f;
        float time = 0;

        if (DragonController != null && !DragonController.IsScreaming)
        {
            DragonController.IsPlayerNear = true;
            DragonController.IsScreaming = true;
            DragonController.Scream();
        }

        while (time < duration)
        {
            ShowCamera.transform.position = Vector3.Lerp(startPos, targetPos, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        // 제목 활성화 & 카메라 최종 목적지 도달
        ShowCamera.transform.position = targetPos;
        RedDragonTitle.gameObject.SetActive(true);
        // 타자기 효과
        yield return StartCoroutine(TypingBossTitleEffect("레드 드래곤", 0.08F));

        // 대기 시간
        yield return new WaitForSeconds(3f);

        // 정상화
        ShowCamera.enabled = false;
        _playerCam.enabled = true;
        RedDragonTitle.gameObject.SetActive(false);
        _playerController.enabled = true;
        HP.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (_playTime == 1)
            {
                ShowDragonBossEnterScene();
                _playTime--;
            }
        }
    }

    void OnClickExitButton()
    {
        SceneManager.LoadScene("Main");
    }
}
