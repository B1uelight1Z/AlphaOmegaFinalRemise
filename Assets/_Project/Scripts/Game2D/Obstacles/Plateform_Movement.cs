using UnityEngine;
using System.Collections;

public class Plateform_Movement : MonoBehaviour
{
    public float speed;
    public Transform[] waypoints;
    public float waitTime = 2f;

    private int destPoint = 0;
    private Vector3 lastPosition;

    void Start()
    {
        if (waypoints.Length == 0) return;
        lastPosition = transform.position;
        StartCoroutine(MovePlatform());
    }

    IEnumerator MovePlatform()
    {
        while (true)
        {
            Vector3 target = waypoints[destPoint].position;

            while (Vector3.Distance(transform.position, target) > 0.01f)
            {
                Vector3 newPos = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
                Vector3 delta = newPos - transform.position;
                transform.position = newPos;

                // Déplacer le joueur via Rigidbody2D si dessus de la plateforme
                Collider2D[] hits = Physics2D.OverlapBoxAll(
                    transform.position,
                    GetComponent<Collider2D>().bounds.size,
                    0f
                );
                foreach (Collider2D hit in hits)
                {
                    if (hit.CompareTag("Player"))
                    {
                        Rigidbody2D rb = hit.attachedRigidbody;
                        if (rb != null)
                        {
                            rb.position += new Vector2(delta.x, delta.y); // Déplacement exact
                        }
                    }
                }

                yield return null;
            }

            // Pause sur le waypoint
            yield return new WaitForSeconds(waitTime);

            destPoint = (destPoint + 1) % waypoints.Length;
        }
    }
}