using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

/*
 * Nom du script : RepairCapsuleManager
 * Auteur : Timothy Chatelier
 * Date : 03/03/2026
 * Projet : AlphaOmega Inversion - Jeu 2D
 * 
 * Description globale :
 * Ce script gère le système d'objectif des capsules
 * à réparer dans le jeu.
 * 
 * Le gestionnaire :
 * - compte le nombre de capsules réparées
 * - met à jour l'interface utilisateur
 * - détecte lorsque l'objectif est complété
 * - affiche un message de fin de mission
 * - peut automatiquement retourner au menu principal
 * 
 * Informations pertinentes :
 * - Le script fonctionne avec RepairCapsule.
 * - L'interface utilise TextMeshPro.
 * - Le script utilise un Singleton avec "instance".
 * - Le retour au menu peut être activé ou désactivé.
 */

public class RepairCapsuleManager : MonoBehaviour
{
    // Instance globale du gestionnaire de capsules.
    public static RepairCapsuleManager instance;

    /*
     * =========================
     * SECTION : Objectif
     * =========================
     */

    // Nombre total de capsules à réparer.
    [Header("Objectif")]
    public int totalCapsules = 3;

    // Nombre actuel de capsules réparées.
    private int capsulesReparees = 0;

    /*
     * =========================
     * SECTION : UI
     * =========================
     */

    // Texte affichant la progression de l'objectif.
    [Header("UI")]
    public TextMeshProUGUI objectifText;

    // Texte affiché lorsque la mission est complétée.
    public TextMeshProUGUI finText;

    /*
     * =========================
     * SECTION : Fin de mission
     * =========================
     */

    // Détermine si le jeu retourne automatiquement au menu.
    [Header("Fin")]
    public bool retournerMenuApresFin = false;

    // Temps d'attente avant le retour au menu.
    public float delaiAvantMenu = 5f;

    // Nom de la scène du menu principal.
    public string nomSceneMenu = "MainMenu";

    // Vérifie si l'objectif est déjà complété.
    private bool objectifComplete = false;

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
     * Initialise l'interface utilisateur au début de la scène.
     * 
     * Cette fonction :
     * - cache le texte de fin
     * - affiche l'objectif actuel
     */
    private void Start()
    {
        // Cache le texte de fin de mission.
        if (finText != null)
        {
            finText.gameObject.SetActive(false);
        }

        // Met à jour l'affichage de l'objectif.
        MettreAJourObjectif();
    }

    /*
     * Fonction : CapsuleReparee
     * Description :
     * Est appelée lorsqu'une capsule est réparée.
     * 
     * Cette fonction :
     * - augmente le compteur
     * - met à jour l'interface
     * - vérifie si l'objectif est terminé
     */
    public void CapsuleReparee()
    {
        // Empêche les appels supplémentaires après la fin.
        if (objectifComplete)
        {
            return;
        }

        // Augmente le nombre de capsules réparées.
        capsulesReparees++;

        // Empêche de dépasser le total maximum.
        if (capsulesReparees > totalCapsules)
        {
            capsulesReparees = totalCapsules;
        }

        // Met à jour l'affichage.
        MettreAJourObjectif();

        // Vérifie si toutes les capsules sont réparées.
        if (capsulesReparees >= totalCapsules)
        {
            ObjectifComplete();
        }
    }

    /*
     * Fonction : MettreAJourObjectif
     * Description :
     * Met à jour le texte affichant
     * la progression des capsules réparées.
     */
    void MettreAJourObjectif()
    {
        // Vérifie que le texte existe.
        if (objectifText != null)
        {
            // Affiche le texte de progression.
            objectifText.gameObject.SetActive(true);

            // Met à jour le contenu du texte.
            objectifText.text =
                "Capsules réparées : "
                + capsulesReparees
                + " / "
                + totalCapsules;
        }
    }

    /*
     * Fonction : ObjectifComplete
     * Description :
     * Est appelée lorsque toutes les capsules
     * ont été réparées.
     * 
     * Cette fonction :
     * - affiche le message de victoire
     * - bloque la progression supplémentaire
     * - peut retourner automatiquement au menu
     */
    void ObjectifComplete()
    {
        // Marque l'objectif comme terminé.
        objectifComplete = true;

        // Met à jour le texte principal.
        if (objectifText != null)
        {
            objectifText.text =
                "Objectif complété : toutes les capsules sont réparées";
        }

        // Affiche le message de fin.
        if (finText != null)
        {
            finText.gameObject.SetActive(true);

            finText.text = "Mission complétée !";
        }

        // Affiche un message dans la console Unity.
        Debug.Log("Toutes les capsules sont réparées.");

        /*
         * Vérifie si le retour automatique
         * au menu est activé.
         */
        if (retournerMenuApresFin)
        {
            StartCoroutine(RetourMenu());
        }
    }

    /*
     * Fonction : RetourMenu
     * Description :
     * Coroutine qui retourne automatiquement
     * au menu principal après un délai.
     * 
     * Cette fonction :
     * - attend quelques secondes
     * - réactive le curseur
     * - réinitialise les checkpoints
     * - charge la scène du menu principal
     */
    IEnumerator RetourMenu()
    {
        // Attend avant le retour au menu.
        yield return new WaitForSeconds(delaiAvantMenu);

        // Réinitialise la vitesse du temps.
        Time.timeScale = 1f;

        // Réactive le curseur.
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Réinitialise les checkpoints.
        CheckpointManager.ResetCheckpoint();

        // Charge la scène du menu principal.
        SceneManager.LoadScene(nomSceneMenu);
    }
}