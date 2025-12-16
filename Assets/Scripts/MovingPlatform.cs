using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    public float moveSpeed = 2f;
    private Vector3 nextPosition;

    private Transform playerToParent = null;
    private Transform playerToUnparent = null;
    private bool shouldParent = false;
    private bool shouldUnparent = false;

    void Start()
    {
        nextPosition = pointB.position;
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, nextPosition, moveSpeed * Time.deltaTime);
        if (Vector3.Distance(transform.position, nextPosition) < 0.01f)
        {
            nextPosition = (nextPosition == pointA.position) ? pointB.position : pointA.position;
        }
    }

    void LateUpdate()
    {
        // Handle parenting at the end of the frame
        if (shouldParent && playerToParent != null)
        {
            playerToParent.parent = transform;
            shouldParent = false;
            playerToParent = null;
        }

        if (shouldUnparent && playerToUnparent != null)
        {
            playerToUnparent.parent = null;
            shouldUnparent = false;
            playerToUnparent = null;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerToParent = collision.gameObject.transform;
            shouldParent = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerToUnparent = collision.gameObject.transform;
            shouldUnparent = true;
        }
    }
}