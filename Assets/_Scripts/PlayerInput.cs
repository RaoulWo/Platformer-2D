using UnityEngine;

public class PlayerInput : IPlayerInput
{
    public float Vertical => Input.GetAxis("Vertical");
    public float Horizontal => Input.GetAxis("Horizontal");
    public bool JumpButtonDown => Input.GetButtonDown("Jump");
    public bool JumpButtonUp => Input.GetButtonUp("Jump");
    public bool DashButtonDown => Input.GetKeyDown(KeyCode.X);
}