using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

/*
 * Nom du script : MainMenu
 * Auteur : Michael Proulx
 * Date : 03/03/2026
 * Projet : AlphaOmega Inversion
 * 
 * Description globale :
 * Ce script gère le menu principal du jeu.
 * 
 * Le système permet :
 * - de gérer les différentes fenêtres du menu
 * - de sauvegarder le nom du joueur
 * - d'ouvrir les introductions
 * - de lancer les modes 2D et 3D
 * - de sélectionner des niveaux ou zones
 * - d'afficher les scores
 * - d'ouvrir les paramètres
 * - de quitter le jeu
 * 
 * Informations pertinentes :
 * - Le script utilise TextMeshPro pour l'interface.
 * - Les données du joueur sont sauvegardées avec PlayerPrefs.
 * - Les fenêtres sont activées/désactivées dynamiquement.
 * - Le username est limité à 10 caractères.
 * - Le script fonctionne avec :
 *      - LeaderboardManager
 *      - SettingsMenu
 *      - AudioManager
 *      - Inventory
 */

public class MainMenu : MonoBehaviour
{
    /*
     * =========================
     * SECTION : Fenêtres
     * =========================
     */

    // Fenêtre principale Play.
    [Header("Windows")]
    public GameObject playWindow;

    // Fenêtre de sélection des niveaux.
    public GameObject levelSelector;

    // Fenêtre des scores.
    public GameObject scoresWindow;

    // Fenêtre des paramètres.
    public GameObject settingsWindow;

    // Fenêtre des contrôles.
    public GameObject controlsWindow;

    // Fenêtre des crédits.
    public GameObject creditsWindow;

    // Fenêtre de confirmation pour quitter.
    public GameObject quitConfirmationWindow;

    // Fenêtre d'introduction du mode 2D.
    public GameObject introduction2DWindow;

    // Fenêtre d'introduction du mode 3D.
    public GameObject introduction3DWindow;

    /*
     * =========================
     * SECTION : Interface utilisateur
     * =========================
     */

    // Champ de texte pour le username.
    [Header("UI")]
    public TMP_InputField usernameInput;

    // Bouton Play Game.
    public Button playGameButton;

    // Bouton Levels.
    public Button levelsButton;

    // Bouton Enter.
    public Button enterButton;

    // Texte du message de connexion.
    public TextMeshProUGUI connectionMessage;

    // CanvasGroup utilisé pour les animations du message.
    public CanvasGroup connectionCanvasGroup;

    /*
     * =========================
     * SECTION : Managers
     * =========================
     */

    // Référence vers le menu des paramètres.
    [Header("Managers")]
    public SettingsMenu settingsMenu;

    // Référence vers le gestionnaire audio.
    public AudioManager audioManager;

    // Référence vers le leaderboard.
    public LeaderboardManager leaderboardManager;

    /*
     * =========================
     * SECTION : Scènes 2D
     * =========================
     */

    // Nom de la scène du niveau 1.
    [Header("Scènes 2D")]
    public string scene2DLevel1 = "Niveau_1";

    // Nom de la scène du niveau 2.
    public string scene2DLevel2 = "Niveau_2";

    // Nom de la scène du niveau 3.
    public string scene2DLevel3 = "Niveau_3";

    /*
     * =========================
     * SECTION : Scène 3D
     * =========================
     */

    // Nom de la scène principale du mode 3D.
    [Header("Scène 3D")]
    public string scene3D = "PROJET_AOI";

    /*
     * Fonction : Start
     * Description :
     * Initialise le menu principal.
     * 
     * Cette fonction :
     * - récupère les références nécessaires
     * - configure les boutons
     * - cache les fenêtres
     * - charge le dernier username
     * - configure les validations
     */
    void Start()
    {
        // Recherche automatiquement le SettingsMenu.
        settingsMenu = FindFirstObjectByType<SettingsMenu>();

        // Limite le username à 10 caractères.
        usernameInput.characterLimit = 10;

        // Ferme toutes les fenêtres au démarrage.
        CloseAllWindows();

        // Désactive le bouton Play Game.
        if(playGameButton != null)
            playGameButton.interactable = false;

        // Désactive le bouton Levels.
        if(levelsButton != null)
            levelsButton.interactable = false;

        // Désactive le bouton Enter.
        if(enterButton != null)
            enterButton.interactable = false;

        // Cache le message de connexion.
        if(connectionMessage != null)
            connectionMessage.gameObject.SetActive(false);

        /*
         * Charge automatiquement l'ancien username
         * sauvegardé dans PlayerPrefs.
         */
        if(PlayerPrefs.HasKey("PlayerName"))
        {
            usernameInput.text = PlayerPrefs.GetString("PlayerName");
        }

        /*
         * Vérifie automatiquement le username
         * lorsque le joueur écrit.
         */
        usernameInput.onValueChanged.AddListener(delegate { CheckUsername(); });

        // Vérifie immédiatement le username actuel.
        CheckUsername();
    }

    /*
     * =========================
     * SECTION : Validation Username
     * =========================
     */

    /*
     * Fonction : CheckUsername
     * Description :
     * Vérifie si le username entré est valide.
     * 
     * Active le bouton Enter uniquement
     * si le champ n'est pas vide.
     */
    public void CheckUsername()
    {
        // Vérifie que les références existent.
        if(usernameInput == null || enterButton == null)
            return;

        // Vérifie si le texte est valide.
        bool validName = !string.IsNullOrWhiteSpace(usernameInput.text);

        // Active ou désactive le bouton Enter.
        enterButton.interactable = validName;
    }

    /*
     * Fonction : EnterUsername
     * Description :
     * Sauvegarde le username du joueur.
     * 
     * Cette fonction :
     * - valide le nom
     * - limite la longueur
     * - sauvegarde les données
     * - active les boutons du menu
     * - affiche un message animé
     */
    public void EnterUsername()
    {
        // Récupère le username sans espaces inutiles.
        string playerName = usernameInput.text.Trim();

        // Vérifie si le nom est vide.
        if(string.IsNullOrWhiteSpace(playerName))
            return;

        // Vérifie la longueur maximale.
        if(playerName.Length > 10)
        {
            StartCoroutine(ShowAnimatedMessage(
                "Maximum de 10 caractères !"
            ));

            return;
        }

        // Sauvegarde le username.
        PlayerPrefs.SetString("PlayerName", playerName);

        // Sauvegarde PlayerPrefs.
        PlayerPrefs.Save();

        // Active le bouton Play.
        if(playGameButton != null)
            playGameButton.interactable = true;

        // Active le bouton Levels.
        if(levelsButton != null)
            levelsButton.interactable = true;

        // Affiche un message de connexion.
        StartCoroutine(ShowAnimatedMessage(
            "Utilisateur " + playerName + " connecté"
        ));
    }

    /*
     * Fonction : ShowAnimatedMessage
     * Description :
     * Affiche un message animé avec un effet
     * de fade in et fade out.
     * 
     * Paramètre :
     * - message : texte à afficher
     */
    private System.Collections.IEnumerator ShowAnimatedMessage(string message)
    {
        if(connectionMessage != null && connectionCanvasGroup != null)
        {
            // Affiche le texte.
            connectionMessage.gameObject.SetActive(true);

            // Définit le message.
            connectionMessage.text = message;

            // Rend le texte invisible au départ.
            connectionCanvasGroup.alpha = 0f;

            /*
             * =====================
             * FADE IN
             * =====================
             */

            float timer = 0f;

            while(timer < 1f)
            {
                timer += Time.deltaTime * 2f;

                connectionCanvasGroup.alpha =
                    Mathf.Lerp(0f, 1f, timer);

                yield return null;
            }

            // Temps d'affichage.
            yield return new WaitForSeconds(1.5f);

            /*
             * =====================
             * FADE OUT
             * =====================
             */

            timer = 0f;

            while(timer < 1f)
            {
                timer += Time.deltaTime * 2f;

                connectionCanvasGroup.alpha =
                    Mathf.Lerp(1f, 0f, timer);

                yield return null;
            }

            // Cache le texte.
            connectionMessage.gameObject.SetActive(false);
        }
    }

    /*
     * =========================
     * SECTION : Play Game
     * =========================
     */

    /*
     * Fonction : OpenPlayWindow
     * Description :
     * Ouvre la fenêtre Play après validation
     * du username.
     */
    public void OpenPlayWindow()
    {
        string playerName = usernameInput.text;

        // Vérifie si le username est vide.
        if(string.IsNullOrWhiteSpace(playerName))
        {
            Debug.Log("Le joueur doit entrer un username !");
            return;
        }

        // Sauvegarde le username.
        PlayerPrefs.SetString("PlayerName", playerName);
        PlayerPrefs.Save();

        // Ferme toutes les fenêtres.
        CloseAllWindows();

        // Ouvre la fenêtre Play.
        if(playWindow != null)
        {
            playWindow.SetActive(true);
        }
    }

    /*
     * Fonction : ClosePlayWindow
     * Description :
     * Ferme la fenêtre Play.
     */
    public void ClosePlayWindow()
    {
        if(playWindow != null)
        {
            playWindow.SetActive(false);
        }
    }

    /*
     * =========================
     * SECTION : Introductions
     * =========================
     */

    public void OpenIntroduction2DWindow()
    {
        CloseAllWindows();

        if(introduction2DWindow != null)
        {
            introduction2DWindow.SetActive(true);
        }
    }

    public void CloseIntroduction2DWindow()
    {
        if(introduction2DWindow != null)
        {
            introduction2DWindow.SetActive(false);
        }
    }

    public void OpenIntroduction3DWindow()
    {
        CloseAllWindows();

        if(introduction3DWindow != null)
        {
            introduction3DWindow.SetActive(true);
        }
    }

    public void CloseIntroduction3DWindow()
    {
        if(introduction3DWindow != null)
        {
            introduction3DWindow.SetActive(false);
        }
    }

    /*
     * =========================
     * SECTION : Level Selector
     * =========================
     */

    public void OpenLevelSelector()
    {
        CloseAllWindows();

        // Réinitialise les œufs.
        Inventory.ResetEggs();

        if(levelSelector != null)
        {
            levelSelector.SetActive(true);
        }
    }

    public void CloseLevelSelector()
    {
        if(levelSelector != null)
        {
            levelSelector.SetActive(false);
        }
    }

    /*
     * =========================
     * SECTION : Scores
     * =========================
     */

    public void OpenScores()
    {
        CloseAllWindows();

        if(scoresWindow != null)
        {
            scoresWindow.SetActive(true);
        }

        // Met à jour les leaderboards.
        if(leaderboardManager != null)
        {
            leaderboardManager.DisplayScores();
        }
    }

    public void CloseScores()
    {
        if(scoresWindow != null)
        {
            scoresWindow.SetActive(false);
        }
    }

    /*
     * =========================
     * SECTION : Settings
     * =========================
     */

    public void OpenSettings()
    {
        CloseAllWindows();

        if(settingsWindow != null)
        {
            settingsWindow.SetActive(true);
        }
    }

    public void CloseSettings()
    {
        if(settingsWindow != null)
        {
            settingsWindow.SetActive(false);
        }
    }

    /*
     * =========================
     * SECTION : Controls
     * =========================
     */

    public void OpenControls()
    {
        CloseAllWindows();

        if(controlsWindow != null)
        {
            controlsWindow.SetActive(true);
        }
    }

    public void CloseControls()
    {
        if(controlsWindow != null)
        {
            controlsWindow.SetActive(false);
        }
    }

    /*
     * =========================
     * SECTION : Credits
     * =========================
     */

    public void OpenCredits()
    {
        CloseAllWindows();

        if(creditsWindow != null)
        {
            creditsWindow.SetActive(true);
        }
    }

    public void CloseCredits()
    {
        if(creditsWindow != null)
        {
            creditsWindow.SetActive(false);
        }
    }

    /*
     * =========================
     * SECTION : Close All Windows
     * =========================
     */

    /*
     * Fonction : CloseAllWindows
     * Description :
     * Ferme toutes les fenêtres du menu principal.
     */
    public void CloseAllWindows()
    {
        if(playWindow != null)
            playWindow.SetActive(false);

        if(levelSelector != null)
            levelSelector.SetActive(false);

        if(settingsWindow != null)
            settingsWindow.SetActive(false);

        if(controlsWindow != null)
            controlsWindow.SetActive(false);

        if(creditsWindow != null)
            creditsWindow.SetActive(false);

        if(quitConfirmationWindow != null)
            quitConfirmationWindow.SetActive(false);

        if(introduction2DWindow != null)
            introduction2DWindow.SetActive(false);

        if(introduction3DWindow != null)
            introduction3DWindow.SetActive(false);

        if(scoresWindow != null)
            scoresWindow.SetActive(false);
    }

    /*
     * =========================
     * SECTION : Play 2D
     * =========================
     */

    public void Play2D()
    {
        // Réinitialise l'inventaire.
        Inventory.ResetEggs();
        Inventory.ResetEnergys();

        // Charge le premier niveau 2D.
        SceneManager.LoadScene(scene2DLevel1);
    }

    /*
     * =========================
     * SECTION : Play 3D
     * =========================
     */

    public void Play3D()
    {
        // Réinitialise l'inventaire.
        Inventory.ResetEggs();
        Inventory.ResetEnergys();

        // Lance la zone 1 du mode 3D.
        Load3DZone(1);
    }

    /*
     * =========================
     * SECTION : Levels 2D
     * =========================
     */

    public void Play2DLevel1()
    {
        SceneManager.LoadScene(scene2DLevel1);
    }

    public void Play2DLevel2()
    {
        SceneManager.LoadScene(scene2DLevel2);
    }

    public void Play2DLevel3()
    {
        SceneManager.LoadScene(scene2DLevel3);
    }

    /*
     * =========================
     * SECTION : Zones 3D
     * =========================
     */

    public void Play3DZone1()
    {
        Load3DZone(1);
    }

    public void Play3DZone2()
    {
        Load3DZone(2);
    }

    public void Play3DZone3()
    {
        Load3DZone(3);
    }

    /*
     * Fonction : Load3DZone
     * Description :
     * Configure la zone de départ du mode 3D
     * puis charge la scène 3D.
     * 
     * Paramètre :
     * - zoneId : zone de départ
     */
    private void Load3DZone(int zoneId)
    {
        // Sauvegarde la zone de départ.
        PlayerPrefs.SetInt("ZoneDepart3D", zoneId);

        // Sauvegarde PlayerPrefs.
        PlayerPrefs.Save();

        // Charge la scène 3D.
        SceneManager.LoadScene(scene3D);
    }

    /*
     * =========================
     * SECTION : Quitter le jeu
     * =========================
     */

    public void OpenQuitConfirmation()
    {
        CloseAllWindows();

        if(quitConfirmationWindow != null)
        {
            quitConfirmationWindow.SetActive(true);
        }
    }

    public void CancelQuit()
    {
        if(quitConfirmationWindow != null)
        {
            quitConfirmationWindow.SetActive(false);
        }
    }

    /*
     * Fonction : QuitGame
     * Description :
     * Ferme complètement le jeu.
     */
    public void QuitGame()
    {
        Debug.Log("Quit Game");

        Application.Quit();
    }
}