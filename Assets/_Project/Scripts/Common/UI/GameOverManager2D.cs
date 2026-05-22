using UnityEngine;
using UnityEngine.SceneManagement;

/*
 * Nom du script : GameOverManager2D
 * Auteur : Michael Proulx
 * Date : 03/03/2026
 * Projet : AlphaOmega Inversion - Jeu 2D
 * 
 * Description globale :
 * Ce script gère le système de Game Over
 * du mode 2D.
 * 
 * Le gestionnaire :
 * - affiche l'écran de Game Over
 * - met le jeu en pause
 * - permet de recommencer au checkpoint
 * - réinitialise les énergies
 * - ouvre le menu des paramètres
 * - permet le retour au menu principal
 * 
 * Informations pertinentes :
 * - Le script fonctionne avec PlayerHealth2D.
 * - Le jeu est mis en pause avec Time.timeScale.
 * - Le système utilise un Singleton avec "instance".
 * - EnergyManager est utilisé pour réinitialiser
 *   les objets d'énergie.
 */

public class GameOverManager2D : MonoBehaviour
{
    /*
     * =========================
     * SECTION : Interface utilisateur
     * =========================
     */

    // Interface affichée lors du Game Over.
    public GameObject gameOverUI;

    // Fenêtre des paramètres.
    public GameObject settingsWindow;

    /*
     * =========================
     * SECTION : Joueur
     * =========================
     */

    // Référence vers le système de vie du joueur.
    public PlayerHealth2D playerHealth;

    // Instance globale du gestionnaire.
    public static GameOverManager2D instance;

    /*
     * Fonction : Awake
     * Description :
     * Initialise l'instance Singleton
     * du gestionnaire de Game Over.
     */
    private void Awake()
    {
        // Définit cette instance comme gestionnaire principal.
        instance = this;
    }

    /*
     * Fonction : Start
     * Description :
     * Initialise les interfaces du système.
     * 
     * Cette fonction cache :
     * - l'écran de Game Over
     * - la fenêtre des paramètres
     */
    private void Start()
    {
        // Cache l'interface de Game Over.
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(false);
        }

        // Cache la fenêtre des paramètres.
        if (settingsWindow != null)
        {
            settingsWindow.SetActive(false);
        }
    }

    /*
     * Fonction : OnPlayerDeath
     * Description :
     * Est appelée lorsque le joueur meurt.
     * 
     * Cette fonction :
     * - affiche le Game Over
     * - affiche le curseur
     * - met le jeu en pause
     */
    public void OnPlayerDeath()
    {
        // Affiche l'écran de Game Over.
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true);
        }

        // Affiche le curseur.
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Met le jeu en pause.
        Time.timeScale = 0f;
    }

    /*
     * Fonction : RetryButton
     * Description :
     * Relance la partie après un Game Over.
     * 
     * Cette fonction :
     * - réactive le temps
     * - cache les interfaces
     * - replace le joueur
     * - réinitialise les énergies
     * - recache le curseur
     */
    public void RetryButton()
    {
        // Réactive le temps du jeu.
        Time.timeScale = 1f;

        // Cache l'écran de Game Over.
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(false);
        }

        // Cache la fenêtre des paramètres.
        if (settingsWindow != null)
        {
            settingsWindow.SetActive(false);
        }

        // Replace le joueur au checkpoint.
        if (playerHealth != null)
        {
            playerHealth.Respawn();
        }

        // Réinitialise les objets d'énergie.
        if (EnergyManager.instance != null)
        {
            EnergyManager.instance.ResetEnergies();
        }

        // Cache le curseur.
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    /*
     * Fonction : OpenSettingsWindow
     * Description :
     * Ouvre la fenêtre des paramètres
     * depuis l'écran de Game Over.
     */
    public void OpenSettingsWindow()
    {
        // Cache le Game Over.
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(false);
        }

        // Affiche les paramètres.
        if (settingsWindow != null)
        {
            settingsWindow.SetActive(true);
        }
    }

    /*
     * Fonction : CloseSettingsWindow
     * Description :
     * Ferme la fenêtre des paramètres
     * et retourne à l'écran de Game Over.
     */
    public void CloseSettingsWindow()
    {
        // Cache les paramètres.
        if (settingsWindow != null)
        {
            settingsWindow.SetActive(false);
        }

        // Réaffiche le Game Over.
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true);
        }
    }

    /*
     * Fonction : LoadMainMenu
     * Description :
     * Retourne au menu principal du jeu.
     * 
     * Cette fonction :
     * - réactive le temps
     * - réinitialise les checkpoints
     * - affiche le curseur
     * - charge la scène MainMenu
     */
    public void LoadMainMenu()
    {
        // Réactive le temps du jeu.
        Time.timeScale = 1f;

        // Réinitialise les checkpoints.
        CheckpointManager.ResetCheckpoint();

        // Affiche le curseur.
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Charge le menu principal.
        SceneManager.LoadScene("MainMenu");
    }
}