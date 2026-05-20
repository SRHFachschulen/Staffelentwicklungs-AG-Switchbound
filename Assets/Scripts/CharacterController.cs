using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEngine.InputSystem.InputAction;
public class CharacterController : MonoBehaviour
{
    private Vector2 movementInput;
    [SerializeField] private float movementSpeed;
    private bool PlayerPaused;
    [SerializeField] float jumpForce;
    private bool isGrounded = true;
    [SerializeField] LayerMask groundLayer;
    private Vector3Int currentPosition;
    private Vector3 lastPosition;
    private Tilemap m_Tilemap;

    [SerializeField] Camera mainCamera;
    [SerializeField] GameObject lightMap;
    [SerializeField] GameObject darkMap;
    private Rigidbody2D rb;
    private bool isDarkMapActive = true;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    public Tilemap Tilemap
    {
        get { if (m_Tilemap == null) m_Tilemap = FindAnyObjectByType<Tilemap>(); return m_Tilemap; }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PlayerPaused = false;
    }
    public void Movement(CallbackContext ctx)
    {
        movementInput = ctx.ReadValue<Vector2>();
    }
    public void Jump(CallbackContext ctx)
    {
        if (isGrounded)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            isGrounded = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerPaused) return;
        transform.Translate(movementSpeed * Time.deltaTime * new Vector2(movementInput.x, 0));
        currentPosition = Tilemap.WorldToCell(transform.position);

        GroundCheck();
    }

    private void GroundCheck()
    {
        isGrounded = Physics2D.OverlapCircle(transform.position, 0.09f, groundLayer);
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 0.09f);
    }

    public void ToggleMap()
    {
        isDarkMapActive = !isDarkMapActive;
        lightMap.SetActive(!isDarkMapActive);
        darkMap.SetActive(isDarkMapActive);
        if (isDarkMapActive)
        {
            mainCamera.backgroundColor = Color.black;
        }
        else
        {
            mainCamera.backgroundColor = Color.white;
        }
    }
}
