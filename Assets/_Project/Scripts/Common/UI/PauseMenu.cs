using UnityEngine;
using UnityEngine.SceneManagement;

/*
 * Nom du script : PauseMenu
 * Auteur : Michael Proulx
 * Date : 03/03/2026
 * Projet : AlphaOmega Inversion
 * 
 * Description globale :
 * Ce script gère le système de pause du jeu.
 * 
 * Le système permet :
 * - de mettre le jeu en pause avec la touche ESC
 * - d'afficher le menu pause
 * - de désactiver les contrôles du joueur
 * - d'ouvrir les paramètres
 * - de reprendre la partie
 * - de retourner au menu principal
 * 
 * Informations pertinentes :
 * - Le script utilise un système Singleton avec "instance".
 * - Le jeu est mis en pause avec Time.timeScale.
 * - Le curseur est affiché uniquement lorsque le jeu est en pause.
 * - Les contrôleurs du joueur et de la caméra
 *   sont désactivés pendant la pause.
 */

public class PauseMenu : MonoBehaviour
{
    // Instance globale du menu pause.
    public static PauseMenu instance;

    /*
     * =========================
     * SECTION : Interface utilisateur
     * =========================
     */

    // Interface principale du menu pause.
    [Header("UI")]
    public GameObject pauseMenuUI;

    /*
     * =========================
     * SECTION : Joueur
     * =========================
     */

    // Script contrôlant le joueur.
    [Header("Player")]
    public MonoBehaviour playerController;

    // Script contrôlant la caméra.
    public MonoBehaviour cameraController;

    /*
     * =========================
     * SECTION : Fenêtres
     * =========================
     */

    // Fenêtre des paramètres.
    [Header("Windows")]
    public GameObject settingsWindow;

    // Vérifie si le jeu est actuellement en pause.
    private bool gameIsPaused = false;

    /*
     * Fonction : Awake
     * Description :
     * Initialise le Singleton du menu pause.
     * 
     * Cette fonction :
     * - vérifie qu'il n'existe qu'une seule instance
     * - désactive les doublons
     */
    void Awake()
    {
        /*
         * Vérifie si une autre instance existe déjà.
         */
        if (instance != null && instance != this)
        {
            Debug.LogWarning(
                "Deux PauseMenu détectés. Celui-ci est désactivé : "
                + gameObject.name
            );

            // Désactive ce script supplémentaire.
            enabled = false;

            return;
        }

        // Définit cette instance comme gestionnaire principal.
        instance = this;
    }

    /*
     * Fonction : Start
     * Description :
     * Initialise le système de pause au démarrage.
     * 
     * Cette fonction :
     * - désactive les interfaces
     * - remet le temps normal
     * - cache le curseur
     */
    void Start()
    {
        // Désactive l'état pause.
        gameIsPaused = false;

        // Réactive le temps normal du jeu.
        Time.timeScale = 1f;

        // Cache le menu pause.
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(false);
        }

        // Cache la fenêtre des paramètres.
        if (settingsWindow != null)
        {
            settingsWindow.SetActive(false);
        }

        // Cache le curseur.
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    /*
     * Fonction : Update
     * Description :
     * Vérifie si le joueur appuie sur ESC
     * afin d'ouvrir ou fermer le menu pause.
     */
    void Update()
    {
        // Vérifie si la touche ESC est pressée.
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("ESC détecté par : " + gameObject.name);

            /*
             * Vérifie si le jeu est déjà en pause
             * afin de reprendre ou mettre en pause.
             */
            if (gameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    /*
     * Fonction : Pause
     * Description :
     * Met le jeu en pause.
     * 
     * Cette fonction :
     * - arrête le temps
     * - affiche le menu pause
     * - désactive les contrôles
     * - affiche le curseur
     */
    public void Pause()
    {
        Debug.Log("Pause appelée par : " + gameObject.name);

        // Active l'état pause.
        gameIsPaused = true;

        // Met le jeu en pause.
        Time.timeScale = 0f;

        // Affiche le menu pause.
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(true);
        }

        // Désactive les contrôles du joueur.
        if (playerController != null)
        {
            playerController.enabled = false;
        }

        // Désactive les contrôles de la caméra.
        if (cameraController != null)
        {
            cameraController.enabled = false;
        }

        // Affiche le curseur.
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    /*
     * Fonction : Resume
     * Description :
     * Reprend la partie après une pause.
     * 
     * Cette fonction :
     * - réactive le temps
     * - cache les interfaces
     * - réactive les contrôles
     * - cache le curseur
     */
    public void Resume()
    {
        Debug.Log("Resume appelé par : " + gameObject.name);

        // Désactive l'état pause.
        gameIsPaused = false;

        // Réactive le temps du jeu.
        Time.timeScale = 1f;

        // Cache le menu pause.
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(false);
        }

        // Cache la fenêtre des paramètres.
        if (settingsWindow != null)
        {
            settingsWindow.SetActive(false);
        }

        // Réactive les contrôles du joueur.
        if (playerController != null)
        {
            playerController.enabled = true;
        }

        // Réactive les contrôles de la caméra.
        if (cameraController != null)
        {
            cameraController.enabled = true;
        }

        // Cache le curseur.
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    /*
     * Fonction : OpenSettingsWindow
     * Description :
     * Ouvre la fenêtre des paramètres
     * depuis le menu pause.
     */
    public void OpenSettingsWindow()
    {
        // Cache le menu pause.
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(false);
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
     * et retourne au menu pause.
     */
    public void CloseSettingsWindow()
    {
        // Cache les paramètres.
        if (settingsWindow != null)
        {
            settingsWindow.SetActive(false);
        }

        // Réaffiche le menu pause.
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(true);
        }
    }

    /*
     * Fonction : LoadMainMenu
     * Description :
     * Retourne au menu principal du jeu.
     * 
     * Cette fonction :
     * - réactive le temps
     * - désactive la pause
     * - affiche le curseur
     * - charge la scène MainMenu
     */
    public void LoadMainMenu()
    {
        // Réactive le temps du jeu.
        Time.timeScale = 1f;

        // Désactive l'état pause.
        gameIsPaused = false;

        // Affiche le curseur.
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Charge le menu principal.
        SceneManager.LoadScene("MainMenu");
    }
}