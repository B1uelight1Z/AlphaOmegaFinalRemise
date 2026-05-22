using UnityEngine;

/*
 * Nom du script : CurrentSceneManager
 * Auteur : Michael Proulx
 * Date : 03/03/2026
 * Projet : AlphaOmega Inversion - Jeu 2D
 * 
 * Description globale :
 * Ce script agit comme gestionnaire de la scène actuelle.
 * 
 * Il permet principalement de conserver certaines
 * informations globales liées à la scène en cours,
 * comme la présence du joueur au démarrage.
 * 
 * Le script utilise un système Singleton afin que
 * les autres scripts puissent accéder facilement
 * au gestionnaire actif.
 * 
 * Informations pertinentes :
 * - Une seule instance de ce script devrait exister
 *   dans chaque scène.
 * - Le booléen isPlayerPresentByDefault peut être utilisé
 *   pour déterminer si le joueur doit être présent
 *   automatiquement dans la scène.
 */

public class CurrentSceneManager : MonoBehaviour
{
    /*
     * Détermine si le joueur doit être présent
     * automatiquement au chargement de la scène.
     */
    public bool isPlayerPresentByDefault = false;

    // Instance globale du gestionnaire de scène.
    public static CurrentSceneManager instance;

    /*
     * Fonction : Awake
     * Description :
     * Initialise l'instance Singleton du gestionnaire.
     * 
     * Cette fonction est appelée avant Start.
     * 
     * Si plusieurs instances sont détectées,
     * un avertissement est affiché dans la console Unity.
     */
    void Awake()
    {
        // Vérifie si une instance existe déjà.
        if(instance != null)
        {
            Debug.LogWarning(
                "Il y a plus d'une instance de CurrentSceneManager dans la scène"
            );
        }

        // Définit cette instance comme gestionnaire principal.
        instance = this;
    }
}