using UnityEngine;

/*
 * Nom du script : Checkpoint
 * Auteur : Michael Proulx, David Champagne
 * Date : 05/03/2026 - Modification 20/05/2026
 * Projet : Alpha Omega Inversion - Jeu 2D
 *
 * Description globale :
 * Ce script permet de créer un point de checkpoint dans un niveau 2D.
 * Lorsque le joueur entre dans la zone de déclenchement du checkpoint,
 * sa position de réapparition est enregistrée dans le CheckpointManager.
 *
 * Informations pertinentes :
 * - Le GameObject qui possède ce script doit avoir un Collider2D avec "Is Trigger" activé.
 * - Le joueur doit avoir le tag "Player".
 * - Si un respawnPoint est assigné, cette position sera utilisée.
 * - Si aucun respawnPoint n'est assigné, la position du checkpoint lui-même sera utilisée.
 */

public class Checkpoint : MonoBehaviour
{
    // Point exact où le joueur doit réapparaître après avoir activé ce checkpoint.
    [Header("Point exact où le joueur doit respawn")]
    public Transform respawnPoint;

    /*
     * Fonction : OnTriggerEnter2D
     * Description :
     * Fonction appelée automatiquement par Unity lorsqu'un Collider2D entre
     * dans la zone de déclenchement du checkpoint.
     *
     * Paramètre :
     * collision : Collider2D de l'objet qui entre dans la zone du checkpoint.
     */
    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (!collision.CompareTag("Player"))
        {
            return;
        }

        // Si un point de respawn précis est assigné, il devient le nouveau checkpoint.
        if (respawnPoint != null)
        {
            CheckpointManager.SetCheckpoint(respawnPoint.position);
        }
        // Sinon, la position du GameObject contenant ce script devient le checkpoint.
        else
        {
            CheckpointManager.SetCheckpoint(transform.position);
        }
    }
}