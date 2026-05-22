using UnityEngine;

/*
 * Auteur : Michael Proulx, David Champagne
 * Date : 08/03/2026 - Modificaton : 20/05/2026
 * Projet : Alpha Omega Inversion - Jeu 2D
 *
 * Description globale :
 * Ce script place le joueur à la bonne position au début de la scène.
 * Si un checkpoint a déjà été enregistré, le joueur apparaît à ce checkpoint.
 * Sinon, il apparaît à la position de l'objet qui possède ce script.
 *
 * Informations pertinentes :
 * - Le script utilise CheckpointManager pour récupérer la position de réapparition.
 */

public class SpawnPlayer : MonoBehaviour
{
    // Cherche le joueur, récupère la position de spawn, réinitialise sa physique et le place au bon endroit.
    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player == null)
        {
            Debug.LogWarning("Player introuvable dans SpawnPlayer.");
            return;
        }

        Vector3 spawnPos = CheckpointManager.GetSpawnPosition(transform.position);

        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        player.transform.position = spawnPos;

        Physics2D.SyncTransforms();

        Debug.Log("Player placé à : " + spawnPos);
    }
}