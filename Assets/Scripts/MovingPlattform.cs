using UnityEngine;

public class MovingPlattform : MonoBehaviour
{
    [SerializeField] float moveSpeed = 1f;
    [SerializeField] Transform[] MovePoints;
    private int currentPointIndex = 0;

    private WorldBasedMovingGround movingGround;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.position = MovePoints[currentPointIndex].position;
        movingGround = GetComponentInParent<WorldBasedMovingGround>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Vector2.Distance(transform.position, MovePoints[currentPointIndex].position) < 0.1f)
        {
            currentPointIndex = (currentPointIndex + 1) % MovePoints.Length;
        }

        transform.position = Vector2.MoveTowards(transform.position, MovePoints[currentPointIndex].position, moveSpeed * Time.deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(transform);
            if (movingGround != null)
                movingGround.player = collision.gameObject;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(null);
        }
    }
}
