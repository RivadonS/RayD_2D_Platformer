using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Parameters")]
    [SerializeField] private float speed;
    [SerializeField] private float jumpPower;

    [Header("Coyote Time")]
    [SerializeField] private float coyoteTime;
    private float coyoteCounter;

    [Header("Multiple Jumps")]
    [SerializeField] private int extraJumps;
    private int jumpCounter;

    [Header("Wall Jumping")]
    [SerializeField] private float wallJumpX;
    [SerializeField] private float wallJumpY;

    [Header("Layers")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;

    [Header("Sounds")]
    [SerializeField] private AudioClip jumpSound;

    private Rigidbody2D body;
    private Animator anim;
    private BoxCollider2D boxCollider;
    private float horizontalInput;

    public bool IsGrounded => Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.down, 0.1f, groundLayer);
    public bool IsOnWall => Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, new Vector2(transform.localScale.x, 0), 0.1f, wallLayer);

    public bool CanAttack => horizontalInput == 0 && IsGrounded && !IsOnWall;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");

        HandleRotation();
        HandleAnimation();
        HandleJumpInput();
        HandleMovementPhysics();
    }

    private void HandleRotation()
    {
        if (horizontalInput > 0.01f)
            transform.localScale = Vector3.one;
        else if (horizontalInput < -0.01f)
            transform.localScale = new Vector3(-1, 1, 1);
    }

    private void HandleAnimation()
    {
        anim.SetBool("run", horizontalInput != 0);
        anim.SetBool("grounded", IsGrounded);
    }

    private void HandleJumpInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            Jump();

        if (Input.GetKeyUp(KeyCode.Space) && body.linearVelocity.y > 0)
            body.linearVelocity = new Vector2(body.linearVelocity.x, body.linearVelocity.y / 2);
    }

    private void HandleMovementPhysics()
    {
        if (IsOnWall)
        {
            body.gravityScale = 0;
            body.linearVelocity = Vector2.zero;
        }
        else
        {
            body.gravityScale = 7;
            body.linearVelocity = new Vector2(horizontalInput * speed, body.linearVelocity.y);

            if (IsGrounded)
            {
                coyoteCounter = coyoteTime;
                jumpCounter = extraJumps;
            }
            else
                coyoteCounter -= Time.deltaTime;
        }
    }

    private void Jump()
    {
        if (coyoteCounter <= 0 && !IsOnWall && jumpCounter <= 0) return;

        if (SoundManager.instance != null)
            SoundManager.instance.PlaySound(jumpSound);

        if (IsOnWall)
            WallJump();
        else
            NormalJump();
    }

    private void NormalJump()
    {
        if (IsGrounded)
        {
            ApplyJumpForce();
        }
        else
        {
            if (coyoteCounter > 0)
            {
                ApplyJumpForce();
            }
            else if (jumpCounter > 0)
            {
                ApplyJumpForce();
                jumpCounter--;
            }
        }
        coyoteCounter = 0;
    }

    private void ApplyJumpForce()
    {
        body.linearVelocity = new Vector2(body.linearVelocity.x, jumpPower);
    }

    private void WallJump()
    {
        body.AddForce(new Vector2(-Mathf.Sign(transform.localScale.x) * wallJumpX, wallJumpY));
    }
}