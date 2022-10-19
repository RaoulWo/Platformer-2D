using System.Collections.Generic;
using UnityEngine;

public class PhysicsObject : MonoBehaviour
{
    // The gravity modifier affecting the gameObject
    [SerializeField] protected float gravityModifier = 1f;
    // The minimum ground normal value to determine whether the gameObject is grounded or not
    [SerializeField] protected float minGroundNormalY = 0.65f;

    // The rigidbody 2D component of the gameObject
    protected Rigidbody2D _rigidbody;
    // The incoming input from outside the class
    protected Vector2 _targetVelocity;
    // The velocity used to calculate the position of the gameObject
    protected Vector2 _velocity;
    // The ground normal used to determine movement on slopes
    protected Vector2 _groundNormal;
    // The contactFilter of the gameObject
    protected ContactFilter2D _contactFilter;
    // The array holding all raycast hit 2D objects 
    protected RaycastHit2D[] _hitBuffer = new RaycastHit2D[16];
    // The list holding only the raycast hit 2D objects that collided with the ray cast
    protected List<RaycastHit2D> _hitBufferList = new List<RaycastHit2D>(16);
    // The boolean to determine whether the player is grounded or not
    protected bool _isGrounded;

    // The minimum move distance in order to update position
    protected const float _minMoveDistance = 0.001f;
    // The padding to ensure that the gameObject doesn't pass into other colliders
    protected const float _shellRadius = 0.01f;

    private void OnEnable()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        // Don't check collisions against triggers
        _contactFilter.useTriggers = false;
        // Sets the layer mask filter property of the gameObject
        _contactFilter.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer));
        // Enables filtering the contact results by layer mask
        _contactFilter.useLayerMask = true;
    }

    private void Update()
    {
        // Reset the targetVelocity from previous frame
        _targetVelocity = Vector2.zero;
        // Compute the targetVelocity
        ComputeVelocity();
    }

    private void FixedUpdate()
    {
        // _isGrounded is considered false until a collision has registered in the frame
        _isGrounded = false;
        
        // Add gravity to _velocity
        _velocity += Physics2D.gravity * (gravityModifier * Time.deltaTime);
        // Add user input to x-component of _velocity
        _velocity.x = _targetVelocity.x;

        // Calculate the position delta
        Vector2 deltaPosition = _velocity * Time.deltaTime;
        // Store the direction to move along the ground (perpendicular to _groundNormal)
        Vector2 moveAlongGround = new Vector2(_groundNormal.y, -_groundNormal.x);

        // Calculate the x movement direction
        Vector2 movement = moveAlongGround * deltaPosition.x;
        // Handle movement in the x-direction
        Move(movement, false);
        
        // Calculate the y movement direction
        movement = Vector2.up * deltaPosition.y;
        // Handle movement in the y-direction
        Move(movement, true);
    }

    // Movement in the x-direction and in the y-direction will be addressed in separate function calls,
    // handling slopes is easier by handling the x-direction first then the y-direction
    void Move(Vector2 movement, bool isYMovement)
    {
        float distance = movement.magnitude;

        if (distance > _minMoveDistance)
        {
            // Calculate the cast distance 
            float castDistance = distance + _shellRadius;
            // Cast the Collider2D shapes in the movement direction
            int count = _rigidbody.Cast(movement, _contactFilter, _hitBuffer, castDistance);

            // Copy only the indices of _hitBuffer to _hitBufferList that have collided with something
            _hitBufferList.Clear();
            for (int i = 0; i < count; i++)
            {
                _hitBufferList.Add(_hitBuffer[i]);
            }
            
            // Compare the normal of the RaycastHit2D objects with minGroundNormalY to determine
            // whether the gameObject is grounded or not
            for (int i = 0; i < _hitBufferList.Count; i++)
            {
                Vector2 currentNormal = _hitBufferList[i].normal;
                if (currentNormal.y > minGroundNormalY)
                {
                    _isGrounded = true;

                    if (isYMovement)
                    {
                        // Store the currentNormal
                        _groundNormal = currentNormal;
                        currentNormal.x = 0f;
                    }
                }
                
                float projection = Vector2.Dot(_velocity, currentNormal);
                if (projection < 0f)
                {
                    // Cancel out velocity that would be stopped by colliding with slopes
                    _velocity -= projection * currentNormal;
                }

                // Check if distance in _hitBufferList is < _shellRadius, if it is use the 
                // shell size instead in order to prevent getting stuck in other colliders
                float modifiedDistance = _hitBufferList[i].distance - _shellRadius;
                distance = modifiedDistance < distance ? modifiedDistance : distance;
            }
        }
        
        _rigidbody.position += movement.normalized * distance;
    }

    protected virtual void ComputeVelocity()
    {
        
    }
    
}
