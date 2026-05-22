using UnityEngine;
using UnityEngine.SceneManagement;

/*
 * Nom du script : LoadSpecificScene
 * Auteur : Michael Proulx, David Champagne
 * Date : 03/03/2026
 * Projet : AlphaOmega Inversion - Jeu 2D
 * 
 * Description globale :
 * Ce script permet de charger automatiquement
 * une scène spécifique lorsque le joueur entre
 * dans une zone de collision.
 * 
 * Le système est principalement utilisé pour :
 * - les transitions entre niveaux
 * - les portes de téléportation
 * - les changements de zones
 * 
 * Avant le chargement de la nouvelle scène,
 * les checkpoints sont réinitialisés.
 * 
 * Informations pertinentes :
 * - Le joueur doit posséder le tag "Player".
 * - Le nom de la scène doit être configuré
 *   dans l'inspecteur Unity.
 * - Le booléen isLoading empêche le chargement
 *   multiple de la scène.
 */

public class LoadSpecificScene : MonoBehaviour
{
    /*
     * =========================
     * SECTION : Scène
     * =========================
     */

    // Nom de la scène à charger.
    [Header("Scene")]
    public string sceneName;

    // Vérifie si la scène est déjà en cours de chargement.
    private bool isLoading = false;

    /*
     * Fonction : OnTriggerEnter2D
     * Description :
     * Détecte lorsqu'un objet entre dans
     * la zone Trigger.
     * 
     * Si l'objet est le joueur :
     * - empêche les chargements multiples
     * - réinitialise les checkpoints
     * - charge la nouvelle scène
     */
    private void OnTriggerEnter2D(Collider2D collision)
    {
        /*
         * Vérifie :
         * - que l'objet entrant est le joueur
         * - qu'aucun chargement n'est déjà en cours
         */
        if (collision.CompareTag("Player") && !isLoading)
        {
            // Bloque les chargements supplémentaires.
            isLoading = true;

            // Réinitialise les checkpoints.
            CheckpointManager.ResetCheckpoint();

            // Charge la scène spécifiée.
            SceneManager.LoadScene(sceneName);
        }
    }
}