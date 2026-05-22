using UnityEngine;
using UnityEngine.SceneManagement;

/*
 * Nom du script : GameOverManager
 * Auteur : Michael Proulx
 * Date : 03/03/2026
 * Projet : AlphaOmega Inversion - Jeu 3D
 * 
 * Description globale :
 * Ce script gère tout le système de Game Over
 * du mode 3D.
 * 
 * Le gestionnaire :
 * - détecte la mort du joueur
 * - affiche l'interface de Game Over
 * - bloque les contrôles du joueur
 * - gère les checkpoints
 * - replace le joueur lors d'un retry
 * - permet le retour au menu principal
 * 
 * Informations pertinentes :
 * - Le script utilise un système Singleton.
 * - Le joueur est replacé au dernier checkpoint enregistré.
 * - Le jeu est mis en pause pendant le Game Over.
 * - Le script fonctionne avec :
 *      - PlayerHealth
 *      - AstronautController
 *      - CameraController
 */

public class GameOverManager : MonoBehaviour
{
    // Instance globale du gestionnaire de Game Over.
    public static GameOverManager instance;

    /*
     * =========================
     * SECTION : Interface utilisateur
     * =========================
     */

    // Interface affichée lors du Game Over.
    [Header("UI")]
    public GameObject gameOverUI;

    // Fenêtre des paramètres.
    public GameObject settingsWindow;

    /*
     * =========================
     * SECTION : Joueur
     * =========================
     */

    // Référence vers le transform du joueur.
    [Header("Joueur")]
    public Transform player;

    // Référence vers le système de vie du joueur.
    public PlayerHealth playerHealth;

    // Rigidbody du joueur.
    public Rigidbody playerRigidbody;

    // Contrôleur principal du joueur.
    public AstronautController astronautController;

    /*
     * =========================
     * SECTION : Caméras
     * =========================
     */

    // Gestionnaire de caméra du jeu.
    [Header("Caméras")]
    public CameraController cameraController;

    /*
     * =========================
     * SECTION : Checkpoint
     * =========================
     */

    // Position du dernier checkpoint.
    private Vector3 spawnPoint;

    // Rotation du dernier checkpoint.
    private Quaternion spawnRotation;

    // Vérifie si le joueur est actuellement mort.
    private bool joueurMort = false;

    /*
     * Fonction : Awake
     * Description :
     * Initialise l'instance Singleton
     * du gestionnaire.
     */
    private void Awake()
    {
        // Définit cette instance comme gestionnaire principal.
        instance = this;
    }

    /*
     * Fonction : Start
     * Description :
     * Initialise les références et configure
     * l'état de départ du système.
     * 
     * Cette fonction :
     * - cache les interfaces
     * - récupère les composants manquants
     * - sauvegarde le point de départ initial
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

        /*
         * Récupère automatiquement le Rigidbody
         * du joueur si nécessaire.
         */
        if (playerRigidbody == null && player != null)
        {
            playerRigidbody = player.GetComponent<Rigidbody>();
        }

        /*
         * Récupère automatiquement le contrôleur
         * du joueur si nécessaire.
         */
        if (astronautController == null && player != null)
        {
            astronautController = player.GetComponent<AstronautController>();
        }

        // Sauvegarde le point de départ initial.
        if (player != null)
        {
            spawnPoint = player.position;
            spawnRotation = player.rotation;
        }
    }

    /*
     * Fonction : SetCheckpoint
     * Description :
     * Sauvegarde un nouveau checkpoint.
     * 
     * Paramètres :
     * - position : position du checkpoint
     * - rotation : rotation du checkpoint
     */
    public void SetCheckpoint(Vector3 position, Quaternion rotation)
    {
        // Sauvegarde la position.
        spawnPoint = position;

        // Sauvegarde la rotation.
        spawnRotation = rotation;

        // Affiche des informations dans la console Unity.
        Debug.Log("Nouveau checkpoint enregistré : " + spawnPoint);
    }

    /*
     * Fonction : OnPlayerDeath
     * Description :
     * Est appelée lorsque le joueur meurt.
     * 
     * Cette fonction :
     * - affiche le Game Over
     * - bloque les contrôles
     * - arrête le joueur
     * - bloque la caméra
     * - met le jeu en pause
     */
    public void OnPlayerDeath()
    {
        // Empêche plusieurs activations du Game Over.
        if (joueurMort)
        {
            return;
        }

        // Active l'état de mort.
        joueurMort = true;

        // Affiche l'interface de Game Over.
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true);
        }

        // Bloque les contrôles du joueur.
        if (astronautController != null)
        {
            astronautController.BloquerControle(true);
        }

        // Arrête tous les mouvements du joueur.
        if (playerRigidbody != null)
        {
            playerRigidbody.linearVelocity = Vector3.zero;
            playerRigidbody.angularVelocity = Vector3.zero;
        }

        /*
         * Force la caméra en mode TPS
         * et empêche les changements.
         */
        if (cameraController != null)
        {
            cameraController.AutoriserChangementCamera(false);
            cameraController.ForcerTPS();
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
     * - réactive le jeu
     * - replace le joueur au checkpoint
     * - réinitialise la vie
     * - réactive les contrôles
     * - remet la caméra en FPS
     */
    public void RetryButton()
    {
        // Réactive le temps du jeu.
        Time.timeScale = 1f;

        // Désactive l'état de mort.
        joueurMort = false;

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

        // Replace le joueur au checkpoint.
        ReplacerJoueurAuCheckpoint();

        // Réinitialise la vie du joueur.
        if (playerHealth != null)
        {
            playerHealth.ResetHealth();
        }

        // Réactive les contrôles du joueur.
        if (astronautController != null)
        {
            astronautController.BloquerControle(false);
        }

        /*
         * Réactive le changement de caméra
         * et remet la vue FPS.
         */
        if (cameraController != null)
        {
            cameraController.AutoriserChangementCamera(true);
            cameraController.ForcerFPS();
        }

        // Cache le curseur.
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    /*
     * Fonction : ReplacerJoueurAuCheckpoint
     * Description :
     * Replace le joueur au dernier checkpoint sauvegardé.
     * 
     * Cette fonction :
     * - arrête la physique
     * - replace le joueur
     * - réactive correctement le Rigidbody
     */
    void ReplacerJoueurAuCheckpoint()
    {
        // Vérifie que le joueur existe.
        if (player == null)
        {
            return;
        }

        // Arrête complètement le Rigidbody.
        if (playerRigidbody != null)
        {
            playerRigidbody.linearVelocity = Vector3.zero;
            playerRigidbody.angularVelocity = Vector3.zero;
            playerRigidbody.Sleep();
        }

        // Replace le joueur.
        player.position = spawnPoint;

        // Replace la rotation du joueur.
        player.rotation = spawnRotation;

        // Synchronise la physique Unity.
        Physics.SyncTransforms();

        // Réactive le Rigidbody.
        if (playerRigidbody != null)
        {
            playerRigidbody.WakeUp();
        }

        // Réactive l'objet joueur.
        player.gameObject.SetActive(true);

        // Affiche des informations dans la console Unity.
        Debug.Log("Retry : joueur replacé au checkpoint : " + spawnPoint);
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
     * - affiche le curseur
     * - charge la scène MainMenu
     */
    public void LoadMainMenu()
    {
        // Réactive le temps du jeu.
        Time.timeScale = 1f;

        // Affiche le curseur.
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Charge le menu principal.
        SceneManager.LoadScene("MainMenu");
    }
}