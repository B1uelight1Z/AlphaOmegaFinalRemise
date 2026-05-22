using UnityEngine;

/*
 * Nom du script : EnergyManager
 * Auteur : Michael Proulx
 * Date : 03/03/2026
 * Projet : AlphaOmega Inversion - Jeu 2D
 * 
 * Description globale :
 * Ce script permet de gérer tous les objets d'énergie
 * présents dans la scène.
 * 
 * Le gestionnaire conserve une liste de toutes les énergies
 * récupérables puis peut les réinitialiser lorsque nécessaire.
 * 
 * Ce système est utile pour :
 * - les checkpoints
 * - les redémarrages de niveau
 * - les respawns du joueur
 * 
 * Informations pertinentes :
 * - Le script utilise un Singleton avec "instance".
 * - Tous les objets PickUpEnergy de la scène sont détectés
 *   automatiquement au démarrage.
 * - Chaque PickUpEnergy doit posséder une fonction ResetEnergy().
 */

public class EnergyManager : MonoBehaviour
{
    // Instance globale du gestionnaire d'énergie.
    public static EnergyManager instance;

    // Liste de toutes les énergies présentes dans la scène.
    private PickUpEnergy[] energies;

    /*
     * Fonction : Awake
     * Description :
     * Initialise l'instance globale du gestionnaire.
     * 
     * Cette fonction est appelée avant Start.
     */
    private void Awake()
    {
        // Définit cette instance comme gestionnaire principal.
        instance = this;
    }

    /*
     * Fonction : Start
     * Description :
     * Recherche automatiquement tous les objets
     * PickUpEnergy présents dans la scène.
     * 
     * Les objets trouvés sont sauvegardés
     * dans un tableau pour une utilisation future.
     */
    private void Start()
    {
        // Recherche tous les objets PickUpEnergy dans la scène.
        energies = FindObjectsByType<PickUpEnergy>(FindObjectsSortMode.None);
    }

    /*
     * Fonction : ResetEnergies
     * Description :
     * Réinitialise toutes les énergies présentes
     * dans la scène.
     * 
     * Cette fonction appelle ResetEnergy()
     * sur chaque objet PickUpEnergy valide.
     */
    public void ResetEnergies()
    {
        // Vérifie si la liste des énergies existe.
        if (energies == null)
        {
            return;
        }

        // Parcourt toutes les énergies de la scène.
        foreach (PickUpEnergy energy in energies)
        {
            // Vérifie que l'objet existe encore.
            if (energy != null)
            {
                // Réinitialise l'énergie.
                energy.ResetEnergy();
            }
        }
    }
}