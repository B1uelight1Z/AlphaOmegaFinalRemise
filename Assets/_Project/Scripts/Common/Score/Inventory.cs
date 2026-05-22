using UnityEngine;
using TMPro;

/*
 * Nom du script : Inventory
 * Auteur : Michael Proulx
 * Date : 03/03/2026
 * Projet : AlphaOmega Inversion - Jeu 2D
 * 
 * Description globale :
 * Ce script permet de gérer l'inventaire du joueur.
 * 
 * Le système conserve :
 * - le nombre d'œufs collectés
 * - le nombre d'énergies collectées
 * 
 * Le script met également à jour automatiquement
 * l'interface utilisateur lorsque les valeurs changent.
 * 
 * Informations pertinentes :
 * - Le script utilise un système Singleton avec "instance".
 * - Les compteurs sont statiques afin d'être accessibles
 *   depuis n'importe quel autre script.
 * - L'interface utilise TextMeshPro.
 * - Les valeurs ne peuvent jamais devenir négatives.
 */

public class Inventory : MonoBehaviour
{
    // Nombre total d'œufs possédés par le joueur.
    public static int eggCount;

    // Nombre total d'énergies possédées par le joueur.
    public static int energyCount;

    /*
     * =========================
     * SECTION : Interface utilisateur
     * =========================
     */

    // Texte affichant le nombre d'œufs.
    [Header("UI")]
    public TextMeshProUGUI eggCountText;

    // Texte affichant le nombre d'énergies.
    public TextMeshProUGUI energyCountText;

    // Instance globale de l'inventaire.
    public static Inventory instance;

    /*
     * Fonction : Awake
     * Description :
     * Initialise l'instance Singleton du système
     * d'inventaire.
     * 
     * Cette fonction est appelée avant Start.
     */
    private void Awake()
    {
        // Vérifie si une autre instance existe déjà.
        if (instance != null)
        {
            Debug.LogWarning(
                "Il y a plus d'une instance de Inventory dans la scène"
            );
        }

        // Définit cette instance comme inventaire principal.
        instance = this;
    }

    /*
     * Fonction : Start
     * Description :
     * Met à jour l'interface utilisateur
     * au démarrage de la scène.
     */
    private void Start()
    {
        // Met à jour les textes de l'inventaire.
        UpdateUI();
    }

    /*
     * Fonction : AddEgg
     * Description :
     * Ajoute ou retire des œufs
     * dans l'inventaire du joueur.
     * 
     * Paramètre :
     * - count : quantité à ajouter ou retirer
     */
    public void AddEgg(int count)
    {
        // Modifie le nombre d'œufs.
        eggCount += count;

        // Empêche les valeurs négatives.
        if (eggCount < 0)
        {
            eggCount = 0;
        }

        // Met à jour l'interface utilisateur.
        UpdateUI();
    }

    /*
     * Fonction : AddEnergy
     * Description :
     * Ajoute ou retire de l'énergie
     * dans l'inventaire du joueur.
     * 
     * Paramètre :
     * - count : quantité à ajouter ou retirer
     */
    public void AddEnergy(int count)
    {
        // Modifie le nombre d'énergies.
        energyCount += count;

        // Empêche les valeurs négatives.
        if (energyCount < 0)
        {
            energyCount = 0;
        }

        // Met à jour l'interface utilisateur.
        UpdateUI();
    }

    /*
     * Fonction : UpdateUI
     * Description :
     * Met à jour les textes affichés
     * dans l'interface utilisateur.
     * 
     * Cette fonction affiche :
     * - le nombre d'œufs
     * - le nombre d'énergies
     */
    public void UpdateUI()
    {
        // Vérifie que le texte des œufs existe.
        if (eggCountText != null)
        {
            // Met à jour l'affichage des œufs.
            eggCountText.text = eggCount.ToString();
        }

        // Vérifie que le texte des énergies existe.
        if (energyCountText != null)
        {
            // Met à jour l'affichage des énergies.
            energyCountText.text = energyCount.ToString();
        }
    }

    /*
     * Fonction : ResetEggs
     * Description :
     * Réinitialise complètement le nombre d'œufs
     * du joueur.
     * 
     * Cette fonction met également à jour l'interface.
     */
    public static void ResetEggs()
    {
        // Réinitialise le compteur d'œufs.
        eggCount = 0;

        // Met à jour l'interface si une instance existe.
        if (instance != null)
        {
            instance.UpdateUI();
        }
    }

    /*
     * Fonction : ResetEnergys
     * Description :
     * Réinitialise complètement le nombre d'énergies
     * du joueur.
     * 
     * Cette fonction met également à jour l'interface.
     */
    public static void ResetEnergys()
    {
        // Réinitialise le compteur d'énergies.
        energyCount = 0;

        // Met à jour l'interface si une instance existe.
        if (instance != null)
        {
            instance.UpdateUI();
        }
    }
}