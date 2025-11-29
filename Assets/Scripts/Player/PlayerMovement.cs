using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed;
    private Rigidbody2D rb;
    private Animator anim;

    private void Awake()
    {
        //grab references from game object
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        rb.linearVelocity = new Vector2(horizontalInput * speed ,rb.linearVelocity.y);

        //flip player when move left-right
        if (horizontalInput > 0.01f)
        {
            transform.localScale = Vector3.one;
        } else if (horizontalInput < -0.01f)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, speed);
        }

        //Set animator parameters
        anim.SetBool("run", horizontalInput != 0);
    }
}
