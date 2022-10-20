using UnityEngine;

public class PlayerController : PhysicsObject
{
    // The movement speed of the player
    [SerializeField] private float moveSpeed = 8f;
    // The initial jump speed of the player
    [SerializeField] private float jumpHeight = 5f;
    // The jump cancel modifier applied to the velocity if the player cancels the jump early
    [SerializeField] private float jumpCancelModifier = 0.5f;

    // The property for accessing the player input
    public IPlayerInput PlayerInput { get; set; }

    // The sprite renderer component of the player
    private SpriteRenderer _spriteRenderer;

    private void Awake()    
    {
        PlayerInput = new PlayerInput();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    protected override void ComputeVelocity()
    {
        Vector2 movement = Vector2.zero;
        
        // Get the directional user input
        movement.x = PlayerInput.Horizontal;

        // Handle jump
        HandleJump();
        
        // Flip the sprite if necessary
        FlipSprite(movement.x);

        // Calculate the _targetVelocity 
        _targetVelocity = movement * moveSpeed;
    }

    // Handles the jump
    private void HandleJump()
    {
        if (PlayerInput.JumpButtonDown && _isGrounded)
        {
            _velocity.y = Mathf.Sqrt(2f * (-Physics2D.gravity.y) * gravityModifier * jumpHeight);
        }
        else if (PlayerInput.JumpButtonUp)
        {
            // Apply the jumpCancelModifier to the velocity if y-component of velocity > 0
            if (_velocity.y > 0)
            {
                _velocity.y *= jumpCancelModifier;
            }
        }
    }

    // Flips the sprite depending on the parameter xMovement
    private void FlipSprite(float xMovement)
    {
        bool flipSprite = (_spriteRenderer.flipX ? (xMovement > 0.01f) : (xMovement< 0.01f));
        if (flipSprite)
        {
            _spriteRenderer.flipX = !_spriteRenderer.flipX;
        }
    }
    
}