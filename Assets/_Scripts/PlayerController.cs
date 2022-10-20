using System.Collections;
using UnityEngine;

public class PlayerController : PhysicsObject
{
    // The movement speed of the player
    [SerializeField] private float moveSpeed = 8f;
    // The initial jump speed of the player
    [SerializeField] private float jumpHeight = 5f;
    // The jump cancel modifier applied to the velocity if the player cancels the jump early
    [SerializeField] private float jumpCancelModifier = 0.5f;
    // The dash speed of the player
    [SerializeField] private float dashSpeed = 32f;
    // The cooldown timer of the dash
    [SerializeField] private float dashCooldown = 2f;
    // The duration of each dash
    [SerializeField] private float dashDuration = 0.25f;

    // The property for accessing the player input
    public IPlayerInput PlayerInput { get; set; }

    // The sprite renderer component of the player
    private SpriteRenderer _spriteRenderer;
    // The boolean for determining the direction the player is facing
    private bool _isFacingRight;
    // The time used to track the dashCooldown
    private float _dashTime;
    // The time used to track the dashDuration
    private float _currentDashDuration;


    private void Awake()    
    {
        // Get player components
        PlayerInput = new PlayerInput();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        
        // Set the player to face right
        _isFacingRight = true;
        
        // Initialize the time to track the dashCooldown
        _dashTime = Time.time;
    }

    protected override void ComputeVelocity()
    {
        Vector2 movement = Vector2.zero;
        
        // Get the directional user input
        movement.x = PlayerInput.Horizontal;

        // Flip the sprite if necessary
        FlipSprite(movement.x);

        // Handle jump
        HandleJump();

        // Handle dash/air dash
        HandleDash();

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

    // Handles the players dash/air-dash inputs
    private void HandleDash()
    {
        if (PlayerInput.DashButtonDown && Time.time > _dashTime)
        {
            if (!_isGrounded && !_canAirDash)
            {
                return;
            }

            StartCoroutine(_isFacingRight ? Dash(Vector2.right) : Dash(Vector2.left));
        }
    }

    // Dashes in the given direction
    IEnumerator Dash(Vector2 direction)
    {
        // Reset the cooldown
        _dashTime = Time.time + dashCooldown;
        // Store the dashDuration
        _currentDashDuration = dashDuration;

        while (_currentDashDuration > 0f)
        {
            // Lower the timer in each frame
            _currentDashDuration -= Time.deltaTime;

            // Set the velocity of the rigidbody 2D
            _rigidbody.velocity = direction * dashSpeed;
            
            yield return null;
        }

        // Reset the velocity of the rigidbody
        _rigidbody.velocity = Vector2.zero;
    }

    // Flips the sprite depending on the parameter xMovement
    private void FlipSprite(float xMovement)
    {
        bool flipSprite = (_spriteRenderer.flipX ? (xMovement > 0.01f) : (xMovement < -0.01f));
        if (flipSprite)
        {
            // Flip the sprite
            _spriteRenderer.flipX = !_spriteRenderer.flipX;
            // Change to boolean value of _isFacingRight
            _isFacingRight = !_isFacingRight;
        }
    }
    
}