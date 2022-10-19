using UnityEngine;

public class PlayerController : PhysicsObject
{
    // The movement speed of the player
    [SerializeField] private float moveSpeed = 8f;
    // The initial jump speed of the player
    [SerializeField] private float jumpHeight = 5f;
    // The jump cancel modifier applied to the velocity if the player cancels the jump early
    [SerializeField] private float jumpCancelModifier = 0.5f;

    // The sprite renderer component of the player
    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    protected override void ComputeVelocity()
    {
        Vector2 movement = Vector2.zero;
        
        // Get the user input
        movement.x = Input.GetAxis("Horizontal");

        // Handle jump input
        if (Input.GetButtonDown("Jump") && _isGrounded)
        {
            _velocity.y = Mathf.Sqrt(2f * (-Physics2D.gravity.y) * gravityModifier * jumpHeight);
        }
        else if (Input.GetButtonUp("Jump"))
        {
            // Apply the jumpCancelModifier to the velocity if y-component of velocity > 0
            if (_velocity.y > 0)
            {
                _velocity.y *= jumpCancelModifier;
            }
        }
        
        // Flip the sprite if necessary
        bool flipSprite = (_spriteRenderer.flipX ? (movement.x > 0.01f) : (movement.x < 0.01f));
        if (flipSprite)
        {
            _spriteRenderer.flipX = !_spriteRenderer.flipX;
        }
        
        // Calculate the _targetVelocity 
        _targetVelocity = movement * moveSpeed;
    }
    
}
