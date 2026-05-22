using UnityEngine;

public class DoorElevator : MonoBehaviour
{
    public Transform closedPosition;
    public Transform openPosition;
    public float speed = 5f;

    private bool isDoorClosed = true; // État actuel de la porte

    // Appelée par le bouton
    public void ToggleDoor()
    {
        isDoorClosed = !isDoorClosed; // Inverse la vérité des portes
    }

    public void AlwayClosed()
    {
        isDoorClosed = true; // Mets la porte toujours à true
    }

    void Update()
    {
        // Déplacement de la porte
        if (isDoorClosed)
        {
            transform.position = Vector2.MoveTowards(transform.position, closedPosition.position, speed * Time.deltaTime);
        }
        else
        {
            transform.position = Vector2.MoveTowards(transform.position, openPosition.position, speed * Time.deltaTime);
        }
    }
}