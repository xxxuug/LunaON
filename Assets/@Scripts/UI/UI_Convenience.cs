using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Convenience : UI_Base
{
    [Header("Cursor UI")]
    public Image CursorOff;
    public TMP_Text CursorOnOff;

    void Start()
    {
        CursorOff.gameObject.SetActive(true);
        CursorOnOff.text = "OFF";
        CursorOnOff.outlineColor = Color.red;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            GameManager.Instance.IsCursorUnlock = !GameManager.Instance.IsCursorUnlock;

            if (GameManager.Instance.IsCursorUnlock)
            {
                GameManager.Instance.UnlockCursor();
                CursorOff.gameObject.SetActive(false);
                CursorOnOff.text = "ON";
                CursorOnOff.outlineColor = new Color(0, 255, 161);
            }
            else
            {
                GameManager.Instance.LockCursor();
                CursorOff.gameObject.SetActive(true);
                CursorOnOff.text = "OFF";
                CursorOnOff.outlineColor = Color.red;
            }
        }
    }
}
