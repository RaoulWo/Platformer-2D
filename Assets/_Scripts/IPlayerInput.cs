public interface IPlayerInput
{
    // The property for the vertical player input
    public float Vertical { get; }

    // The property for the horizontal player input
    public float Horizontal { get; }

    // The property for when the jump button is pressed down
    public bool JumpButtonDown { get; }
    
    // The property for when the jump button is released
    public bool JumpButtonUp { get; }
    
    // The property for when the dash button is pressed down
    public bool DashButtonDown { get; }
}
