using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Tilemaps;
using static UnityEngine.InputSystem.InputAction;
public class CharacterController : WorldListener
{
    private float movementInput;
    [SerializeField] private float movementSpeed;
    [SerializeField] float jumpForce;
    [SerializeField] int airJumps = 1;
    private int airJumpsLeft;
    private PostitionState state = PostitionState.Air;
    private bool noMoveInput;
    [SerializeField] Transform groundCheckPoint;
    [SerializeField] Transform leftCheckPoint;
    [SerializeField] Transform rightCheckPoint;
    [SerializeField] LayerMask groundLayer;

    private bool PlayerPaused;

    [SerializeField] Camera mainCamera;
    private Rigidbody2D rb;

    private Vector3Int currentPosition;
    private Vector3Int checkpointPosition;
    private Tilemap m_Tilemap;
    public Tilemap Tilemap
    {
        get { if (m_Tilemap == null) m_Tilemap = FindAnyObjectByType<Tilemap>(); return m_Tilemap; }
    }

    #region Inputs
    public void Movement(CallbackContext ctx)
    {
        movementInput = ctx.ReadValue<float>();
    }
    public void Jump(CallbackContext ctx)
    {
        if (ctx.performed)
        {
            switch (state)
            {
                case PostitionState.Air:
                    if (airJumpsLeft > 0)
                    {
                        rb.linearVelocity = new Vector2(rb.linearVelocityX, 0);
                        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                        airJumpsLeft--;
                    }
                    return;
                case PostitionState.WallL:
                    rb.linearVelocity = Vector2.zero;
                    rb.AddForce(Vector2.right * jumpForce / 2, ForceMode2D.Impulse);
                    rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                    state = PostitionState.Air;
                    noMoveInput = true;
                    return;
                case PostitionState.WallR:
                    rb.linearVelocity = Vector2.zero;
                    rb.AddForce(Vector2.left * jumpForce / 2, ForceMode2D.Impulse);
                    rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                    state = PostitionState.Air;
                    noMoveInput = true;
                    return;
                case PostitionState.Grounded:
                    rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                    state = PostitionState.Air;
                    return;
            }
        }
    }
    public void ToggleMap(CallbackContext ctx)
    {
        if (ctx.performed)
            WorldManager.Instance.SwitchWorld();
    }
    #endregion

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PlayerPaused = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerPaused) return;
        if (MoveCheck())
            transform.Translate(movementSpeed * Time.deltaTime * new Vector2(movementInput, 0));
        currentPosition = Tilemap.WorldToCell(transform.position);
        state = PostitionState.Air;
        if (!GroundCheck())
            WallCheck();
        else
        {
            airJumpsLeft = airJumps;
            noMoveInput = false;
        }

    }

    private bool GroundCheck()
    {
        //isGrounded = Physics2D.OverlapCircle(groundCheckPoint.position, 0.5f, groundLayer);
        if (Physics2D.OverlapBox(groundCheckPoint.position, new Vector2(0.95f, 0.05f), 0f, groundLayer))
        {
            state = PostitionState.Grounded;
            return true;
        }
        return false;
    }

    private void WallCheck()
    {
        if (Physics2D.OverlapBox(leftCheckPoint.position, new Vector2(0.05f, 0.9f), 0f, groundLayer))
            state = PostitionState.WallL;
        if (Physics2D.OverlapBox(rightCheckPoint.position, new Vector2(0.05f, 0.9f), 0f, groundLayer))
            state = PostitionState.WallR;
    }

    private bool MoveCheck()
    {
        if (noMoveInput) return false;
        WallCheck();
        return !(state == PostitionState.WallR && movementInput > 0 || state == PostitionState.WallL && movementInput < 0);

    }

    //Gizmos are used to visualize things in the editor. This method is called when the object is selected in the editor.
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        //Gizmos.DrawWireSphere(groundCheckPoint.position, 0.5f);
        Gizmos.DrawCube(groundCheckPoint.position, new Vector2(0.95f, 0.05f));

        Gizmos.color = Color.yellow;
        Gizmos.DrawCube(leftCheckPoint.position, new Vector2(0.05f, 0.9f));
        Gizmos.DrawCube(rightCheckPoint.position, new Vector2(0.05f, 0.9f));

    }

    public override void OnWorldSwitched(WorldType newWorld)
    {
        switch (newWorld)
        {
            case WorldType.Light:
                mainCamera.backgroundColor = Color.white;
                break;
            case WorldType.Dark:
                mainCamera.backgroundColor = Color.black;
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("CheckPoint"))
        {
            checkpointPosition = Tilemap.WorldToCell(collision.transform.position);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Damage"))
        {
            transform.position = Tilemap.GetCellCenterWorld(checkpointPosition);
            rb.linearVelocity = Vector2.zero;
        }
    }
}

public enum PostitionState
{
    Grounded,
    WallL,
    WallR,
    Air
}