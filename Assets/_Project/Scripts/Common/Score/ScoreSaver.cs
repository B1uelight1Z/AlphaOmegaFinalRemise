using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/*
 * Nom du script : ScoreSaver
 * Auteur : Michael Proulx
 * Date : 03/03/2026
 * Projet : AlphaOmega Inversion
 * 
 * Description globale :
 * Ce script statique permet de sauvegarder
 * les scores des joueurs dans les leaderboards.
 * 
 * Le système :
 * - ajoute un nouveau score
 * - trie automatiquement les scores
 * - conserve uniquement les 5 meilleurs
 * - sauvegarde les données avec PlayerPrefs
 * 
 * Ce script fonctionne pour :
 * - le leaderboard 2D
 * - le leaderboard 3D
 * 
 * Informations pertinentes :
 * - Le script utilise les classes ScoreEntry
 *   et ScoreList.
 * - Les données sont sauvegardées en format JSON.
 * - Le nom du joueur est récupéré depuis PlayerPrefs.
 * - Le script est statique : aucune instance n'est nécessaire.
 */

public static class ScoreSaver
{
    /*
     * Fonction : AddScore2D
     * Description :
     * Ajoute un score au leaderboard 2D.
     * 
     * Paramètre :
     * - score : score obtenu par le joueur
     */
    public static void AddScore2D(int score)
    {
        AddScore("Leaderboard2D", score);
    }

    /*
     * Fonction : AddScore3D
     * Description :
     * Ajoute un score au leaderboard 3D.
     * 
     * Paramètre :
     * - score : score obtenu par le joueur
     */
    public static void AddScore3D(int score)
    {
        AddScore("Leaderboard3D", score);
    }

    /*
     * Fonction : AddScore
     * Description :
     * Ajoute un score dans un leaderboard spécifique.
     * 
     * Cette fonction :
     * - charge les données existantes
     * - crée une nouvelle entrée
     * - trie les scores
     * - conserve les 5 meilleurs
     * - sauvegarde les données
     * 
     * Paramètres :
     * - saveKey : clé PlayerPrefs utilisée
     * - score : score obtenu par le joueur
     */
    private static void AddScore(string saveKey, int score)
    {
        // Crée une nouvelle liste de scores.
        ScoreList list = new ScoreList();

        /*
         * Vérifie si une sauvegarde existe déjà
         * pour ce leaderboard.
         */
        if (PlayerPrefs.HasKey(saveKey))
        {
            // Charge les données sauvegardées.
            list = JsonUtility.FromJson<ScoreList>(
                PlayerPrefs.GetString(saveKey)
            );
        }

        // Vérifie que la liste existe.
        if (list == null)
        {
            list = new ScoreList();
        }

        // Vérifie que la liste interne des scores existe.
        if (list.scores == null)
        {
            list.scores = new List<ScoreEntry>();
        }

        // Crée une nouvelle entrée de score.
        ScoreEntry entry = new ScoreEntry();

        // Définit le nom du joueur.
        entry.playerName = PlayerPrefs.GetString("PlayerName", "Player");

        // Définit le score obtenu.
        entry.score = score;

        // Définit la date actuelle.
        entry.date = System.DateTime.Now.ToString("dd/MM/yyyy");

        // Ajoute le score à la liste.
        list.scores.Add(entry);

        /*
         * Trie les scores du plus grand au plus petit
         * puis conserve uniquement les 5 meilleurs.
         */
        list.scores = list.scores
            .OrderByDescending(x => x.score)
            .Take(5)
            .ToList();

        // Convertit les données en JSON.
        string json = JsonUtility.ToJson(list);

        // Sauvegarde les données dans PlayerPrefs.
        PlayerPrefs.SetString(saveKey, json);

        // Sauvegarde les modifications.
        PlayerPrefs.Save();

        // Affiche un message dans la console Unity.
        Debug.Log("Score sauvegardé dans " + saveKey + " : " + score);
    }
}