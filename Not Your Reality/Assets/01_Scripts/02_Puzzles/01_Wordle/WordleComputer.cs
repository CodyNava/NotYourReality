using System;
using UnityEngine;

public class WordleComputer : Interactable_Base
{
    private bool _isActive = false;
    [SerializeField] private FirstPersonController player;
    [SerializeField] private MouseLook camera;
    [SerializeField] private GameObject computerCamera;

    public void Update()
    {
        if (_isActive && Input.GetKeyUp(KeyCode.Escape))
        {
            ExitTerminal();
        }
    }

    public override void OnInteract()
    {
        base.OnInteract();
        if (!_isActive)
        {
            EnterTerminal();
        }
        else
        {
            ExitTerminal();
        }
    }

    public void EnterTerminal()
    {
        _isActive = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        computerCamera.SetActive(true);
        /*  player.MoveActive = false;
           player.LookActive = false;*/
    }

    public void ExitTerminal()
    {
        _isActive = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        computerCamera.SetActive(false);
        /* player.MoveActive = true;
         player.LookActive = true;*/
    }
}