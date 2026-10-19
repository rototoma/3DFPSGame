using System;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    private void Awake()
    {
        LockCursor();
    }

    public void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}