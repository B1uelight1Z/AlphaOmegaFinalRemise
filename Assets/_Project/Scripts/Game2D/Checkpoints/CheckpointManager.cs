using UnityEngine;
using UnityEngine.SceneManagement;

/*
 * Nom du script : CheckpointManager
 * Auteur : Michael Proulx, David Champagne
 * Date : 04/03/2026 - Modification 20/05/2026
 * Projet : Alpha Omega Inversion - Jeu 2D
 * 
 * Description globale :
 * Ce script gère la position du dernier checkpoint activé par le joueur.
 * Il permet d'enregistrer une position de réapparition, de la récupérer
 * et de réinitialiser le checkpoint lorsque nécessaire.
 * 
 * Informations pertinentes :
 * - Les variables sont statiques afin que la position du checkpoint puisse être
 *   utilisée facilement par d'autres scripts.
 * - Le checkpoint est réinitialisé lorsque la scène active est "MainMenu".
 */

public class CheckpointManager : MonoBehaviour
{
    // Dernière position de checkpoint enregistrée.
    public static Vector3 lastCheckpointPosition;

    // Indique si un checkpoint a déjà été activé par le joueur.
    public static bool hasCheckpoint = false;

    /*
     * Fonction : Awake
     * Description :
     * Fonction appelée automatiquement par Unity lorsque l'objet est chargé.
     * Elle vérifie si la scène actuelle est le MainMenu. Si oui, elle réinitialise
     * les informations du checkpoint.
     */
    private void Awake()
    {
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            ResetCheckpoint();
        }
    }

    /*
     * Fonction : SetCheckpoint
     * Description :
     * Enregistre une nouvelle position de checkpoint.
     */
    public static void SetCheckpoint(Vector3 position)
    {
        lastCheckpointPosition = position;
        hasCheckpoint = true;

        Debug.Log("Checkpoint enregistré : " + position);
    }

    /*
     * Fonction : GetSpawnPosition
     * Description :
     * Retourne la position où le joueur doit apparaître.
     */
    public static Vector3 GetSpawnPosition(Vector3 defaultPosition)
    {
        if (hasCheckpoint)
        {
            return lastCheckpointPosition;
        }

        return defaultPosition;
    }

    /*
     * Fonction : ResetCheckpoint
     * Description :
     * Réinitialise les informations du checkpoint.
     * Après l'appel de cette fonction, aucun checkpoint n'est considéré comme actif.
     */
    public static void ResetCheckpoint()
    {
        lastCheckpointPosition = Vector3.zero;
        hasCheckpoint = false;
    }
}