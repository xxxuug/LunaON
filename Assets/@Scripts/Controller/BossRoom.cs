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
        // �� ������ ����
        StartCoroutine(OpenDoor());

        // ī�޶� ����
        GameObject player = GameObject.FindWithTag("Player");
        _playerCam = player.GetComponentInChildren<Camera>();
        _playerController = player.GetComponent<PlayerController>();

        // �۾� ���ֱ�
        RedDragonTitle.gameObject.SetActive(false);
        // HP�� ���ֱ�
        HP.SetActive(false);
        // ������ ��ư Ŭ��
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
        //Debug.Log("�ڷ�ƾ ����");

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

        //Debug.Log("�� ���� �Ϸ�");
    }

    void ShowDragonBossEnterScene()
    {
        // �÷��̾ �巡����� �Ÿ��� 50�� ��
        // ī�޶� ��ǥ x = -157.5, y = 5, z = 22 ������Ų ��
        // ī�޶� ��ǥ x = -157.5, y = 10, z = 45 �� ���鼭 ���� ȿ��
        // �����ð� �� �ٽ� �÷��̾� ���� ī�޶� ��ǥ�� ���ƿ���
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

        // ���� Ȱ��ȭ & ī�޶� ���� ������ ����
        ShowCamera.transform.position = targetPos;
        RedDragonTitle.gameObject.SetActive(true);
        // Ÿ�ڱ� ȿ��
        yield return StartCoroutine(TypingBossTitleEffect("���� �巡��", 0.08F));

        // ��� �ð�
        yield return new WaitForSeconds(3f);

        // ����ȭ
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
