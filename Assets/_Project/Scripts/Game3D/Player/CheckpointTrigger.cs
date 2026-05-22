using UnityEngine;

// Auteur: David Champagne, Michael Proulx
// Derniere date de modification: 22/05/2026
// Gere l'activation d'un point de controle (checkpoint) lorsque le joueur entre dans la zone.
// Met a jour la position et la rotation de respawn dans le gestionnaire de fin de partie.
public class CheckpointTrigger : MonoBehaviour
{
    [Header("Point exact de respawn")]
    public Transform pointRespawn; // Point ou le joueur va reapparaitre (si non specifie, utilise la position de ce GameObject)

    [Header("Options")]
    public bool utilisableUneSeuleFois = false; // Si vrai, le checkpoint ne pourra plus etre reiveille apres sa premiere activation

    private bool dejaActive = false; // Devient vrai des que le joueur active ce point de controle

    // Detecte quand le joueur entre dans la zone du checkpoint pour mettre a jour son point de respawn
    private void OnTriggerEnter(Collider other)
    {
        // Ignore l'evenement si l'objet qui entre en collision n'est pas le joueur
        if (!other.CompareTag("Player"))
        {
            return;
        }

        // Bloque l'activation si le checkpoint est a usage unique et a deja ete declenche
        if (utilisableUneSeuleFois && dejaActive)
        {
            return;
        }

        // Selectionne le point de respawn dedie, ou utilise la position actuelle du trigger a defaut
        Transform point = pointRespawn != null ? pointRespawn : transform;

        // Transmet les nouvelles coordonnees de respawn au GameOverManager et marque le checkpoint comme active
        if (GameOverManager.instance != null)
        {
            GameOverManager.instance.SetCheckpoint(point.position, point.rotation);
            dejaActive = true;

            Debug.Log("Checkpoint active : " + gameObject.name);
        }
    }
}