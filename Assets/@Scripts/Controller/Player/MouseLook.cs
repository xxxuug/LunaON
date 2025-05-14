using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public float mouseSensitivity = 100f;

    public Transform playerBody;

    private PlayerController playerController;
    private bool _canRotate = true;
    private float _rotX = 0f;

    void Start()
    {
        //Cursor.lockState = CursorLockMode.Locked;
        playerController = playerBody.GetComponent<PlayerController>();

        if (!GameManager.Instance.IsCursorUnlock)
        {
            GameManager.Instance.LockCursor();
        }
    }


    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        _rotX -= mouseY;
        _rotX = Mathf.Clamp(_rotX, -90f, 90f);

        if (playerController.IsDead && _canRotate && QuestManager.Instance.BlockInput == true)
        {
            Invoke(nameof(LockRotate), 0.5f);
        }
        
        if (_canRotate && !GameManager.Instance.IsCursorUnlock)
        {
            transform.localRotation = Quaternion.Euler(_rotX, 0, 0);
            playerBody.Rotate(Vector3.up * mouseX);
        }
    }

    void LockRotate()
    {
        _canRotate = false;
    }
}
