using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;

/*
 * Nom de la classe : ScoreEntry
 * Auteur : Michael Proulx
 * Date : 03/03/2026
 * Projet : AlphaOmega Inversion
 * 
 * Description globale :
 * Cette classe représente une entrée individuelle
 * dans le leaderboard.
 * 
 * Chaque entrée contient :
 * - le nom du joueur
 * - son score
 * - la date du score
 * 
 * Informations pertinentes :
 * - La classe est sérialisable afin d'être sauvegardée
 *   en JSON avec JsonUtility.
 */

[System.Serializable]
public class ScoreEntry
{
    // Nom du joueur.
    public string playerName;

    // Score obtenu par le joueur.
    public int score;

    // Date à laquelle le score a été enregistré.
    public string date;
}

/*
 * Nom de la classe : ScoreList
 * Auteur : Michael Proulx
 * Date : 03/03/2026
 * Projet : AlphaOmega Inversion
 * 
 * Description globale :
 * Cette classe contient une liste de scores
 * utilisée pour sauvegarder les leaderboards.
 * 
 * Informations pertinentes :
 * - La classe est sérialisable pour permettre
 *   la sauvegarde en JSON.
 */

[System.Serializable]
public class ScoreList
{
    // Liste des scores du leaderboard.
    public List<ScoreEntry> scores = new List<ScoreEntry>();
}

/*
 * Nom du script : LeaderboardManager
 * Auteur : Michael Proulx
 * Date : 03/03/2026
 * Projet : AlphaOmega Inversion
 * 
 * Description globale :
 * Ce script permet de gérer les leaderboards
 * du jeu.
 * 
 * Le système :
 * - sauvegarde les scores des joueurs
 * - affiche les meilleurs scores
 * - gère les scores 2D et 3D séparément
 * - sauvegarde les données avec PlayerPrefs
 * 
 * Informations pertinentes :
 * - Le script utilise TextMeshPro pour l'affichage.
 * - Les scores sont sauvegardés en format JSON.
 * - Les leaderboards conservent uniquement
 *   les 5 meilleurs scores.
 * - Le script utilise un Singleton avec "instance".
 */

public class LeaderboardManager : MonoBehaviour
{
    // Instance globale du gestionnaire de leaderboard.
    public static LeaderboardManager instance;

    /*
     * =========================
     * SECTION : Interface utilisateur
     * =========================
     */

    // Champ de texte utilisé pour entrer le nom du joueur.
    public TMP_InputField playerNameInput;

    // Texte affichant le leaderboard du mode 2D.
    public TextMeshProUGUI leaderboard2DText;

    // Texte affichant le leaderboard du mode 3D.
    public TextMeshProUGUI leaderboard3DText;

    /*
     * =========================
     * SECTION : Données
     * =========================
     */

    // Liste des scores du mode 2D.
    private ScoreList scores2D = new ScoreList();

    // Liste des scores du mode 3D.
    private ScoreList scores3D = new ScoreList();

    /*
     * Fonction : Awake
     * Description :
     * Initialise le Singleton du leaderboard.
     * 
     * Empêche plusieurs instances du gestionnaire
     * dans la scène.
     */
    private void Awake()
    {
        // Vérifie si aucune instance n'existe.
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            // Détruit les instances supplémentaires.
            Destroy(gameObject);
        }
    }

    /*
     * Fonction : Start
     * Description :
     * Charge les scores sauvegardés
     * puis met à jour l'affichage.
     */
    private void Start()
    {
        // Charge les leaderboards sauvegardés.
        LoadScores();

        // Met à jour l'affichage des scores.
        DisplayScores();
    }

    /*
     * Fonction : SavePlayerName
     * Description :
     * Sauvegarde le nom du joueur
     * dans PlayerPrefs.
     * 
     * Si aucun nom n'est entré,
     * le nom "Player" est utilisé.
     */
    public void SavePlayerName()
    {
        // Vérifie que le champ input existe.
        if (playerNameInput == null)
        {
            Debug.LogWarning("PlayerNameInput non assigné.");
            return;
        }

        // Récupère le texte entré par le joueur.
        string playerName = playerNameInput.text;

        // Vérifie si le nom est vide.
        if (string.IsNullOrWhiteSpace(playerName))
        {
            playerName = "Player";
        }

        // Sauvegarde le nom du joueur.
        PlayerPrefs.SetString("PlayerName", playerName);

        // Sauvegarde les données.
        PlayerPrefs.Save();
    }

    /*
     * Fonction : AddScore2D
     * Description :
     * Ajoute un score au leaderboard 2D.
     * 
     * Paramètre :
     * - score : score obtenu par le joueur
     */
    public void AddScore2D(int score)
    {
        AddScore("Leaderboard2D", score, scores2D);
    }

    /*
     * Fonction : AddScore3D
     * Description :
     * Ajoute un score au leaderboard 3D.
     * 
     * Paramètre :
     * - score : score obtenu par le joueur
     */
    public void AddScore3D(int score)
    {
        AddScore("Leaderboard3D", score, scores3D);
    }

    /*
     * Fonction : AddScore
     * Description :
     * Ajoute un score dans un leaderboard spécifique.
     * 
     * Cette fonction :
     * - crée une nouvelle entrée
     * - trie les scores
     * - conserve les 5 meilleurs
     * - sauvegarde les données
     * - met à jour l'affichage
     * 
     * Paramètres :
     * - saveKey : clé PlayerPrefs utilisée
     * - score : score obtenu
     * - list : leaderboard concerné
     */
    private void AddScore(string saveKey, int score, ScoreList list)
    {
        // Crée une nouvelle entrée de score.
        ScoreEntry entry = new ScoreEntry();

        // Définit le nom du joueur.
        entry.playerName = PlayerPrefs.GetString("PlayerName", "Player");

        // Définit le score.
        entry.score = score;

        // Définit la date actuelle.
        entry.date = System.DateTime.Now.ToString("dd/MM/yyyy");

        // Ajoute l'entrée dans la liste.
        list.scores.Add(entry);

        /*
         * Trie les scores du plus grand au plus petit
         * puis conserve uniquement les 5 meilleurs.
         */
        list.scores = list.scores
            .OrderByDescending(x => x.score)
            .Take(5)
            .ToList();

        // Convertit la liste en JSON.
        string json = JsonUtility.ToJson(list);

        // Sauvegarde les données.
        PlayerPrefs.SetString(saveKey, json);

        // Sauvegarde PlayerPrefs.
        PlayerPrefs.Save();

        // Met à jour l'affichage.
        DisplayScores();
    }

    /*
     * Fonction : LoadScores
     * Description :
     * Charge les scores sauvegardés
     * depuis PlayerPrefs.
     * 
     * Cette fonction initialise également
     * les listes si elles sont nulles.
     */
    void LoadScores()
    {
        // Charge les scores 2D si une sauvegarde existe.
        if (PlayerPrefs.HasKey("Leaderboard2D"))
        {
            scores2D = JsonUtility.FromJson<ScoreList>(
                PlayerPrefs.GetString("Leaderboard2D")
            );
        }

        // Charge les scores 3D si une sauvegarde existe.
        if (PlayerPrefs.HasKey("Leaderboard3D"))
        {
            scores3D = JsonUtility.FromJson<ScoreList>(
                PlayerPrefs.GetString("Leaderboard3D")
            );
        }

        // Vérifie que la liste 2D existe.
        if (scores2D == null)
        {
            scores2D = new ScoreList();
        }

        // Vérifie que la liste 3D existe.
        if (scores3D == null)
        {
            scores3D = new ScoreList();
        }

        // Vérifie que la liste interne des scores 2D existe.
        if (scores2D.scores == null)
        {
            scores2D.scores = new List<ScoreEntry>();
        }

        // Vérifie que la liste interne des scores 3D existe.
        if (scores3D.scores == null)
        {
            scores3D.scores = new List<ScoreEntry>();
        }
    }

    /*
     * Fonction : DisplayScores
     * Description :
     * Met à jour l'affichage des leaderboards
     * dans l'interface utilisateur.
     * 
     * Les scores affichés incluent :
     * - le rang
     * - le nom du joueur
     * - le score
     * - la date
     */
    public void DisplayScores()
    {
        /*
         * =========================
         * Affichage leaderboard 2D
         * =========================
         */

        if (leaderboard2DText != null)
        {
            // Réinitialise le texte.
            leaderboard2DText.text = "";

            // Parcourt tous les scores 2D.
            for (int i = 0; i < scores2D.scores.Count; i++)
            {
                leaderboard2DText.text +=
                    (i + 1) + ". " +
                    scores2D.scores[i].playerName +
                    " - " +
                    scores2D.scores[i].score +
                    " - " +
                    scores2D.scores[i].date +
                    "\n";
            }
        }

        /*
         * =========================
         * Affichage leaderboard 3D
         * =========================
         */

        if (leaderboard3DText != null)
        {
            // Réinitialise le texte.
            leaderboard3DText.text = "";

            // Parcourt tous les scores 3D.
            for (int i = 0; i < scores3D.scores.Count; i++)
            {
                leaderboard3DText.text +=
                    (i + 1) + ". " +
                    scores3D.scores[i].playerName +
                    " - " +
                    scores3D.scores[i].score +
                    " - " +
                    scores3D.scores[i].date +
                    "\n";
            }
        }
    }
}