using UnityEngine;
using UnityEngine.SceneManagement;

/*
 * Nom du script : LoadScene
 * Auteur : Michael Proulx
 * Date : 03/03/2026
 * Projet : AlphaOmega Inversion
 * 
 * Description globale :
 * Ce script permet de gérer le chargement
 * des différentes scènes du jeu.
 * 
 * Le système permet :
 * - de charger une scène normalement
 * - de démarrer le mode 3D à partir
 *   d'une zone spécifique
 * - de réinitialiser les checkpoints
 * - de sauvegarder certaines informations
 *   dans PlayerPrefs
 * 
 * Informations pertinentes :
 * - Le script utilise SceneManager pour charger les scènes.
 * - Les informations de départ 3D sont sauvegardées
 *   dans PlayerPrefs.
 * - CheckpointManager est réinitialisé avant chaque chargement.
 * - Les fonctions Load3DZone1/2/3 sont prévues
 *   pour être utilisées avec des boutons UI.
 */

public class LoadScene : MonoBehaviour
{
    /*
     * =========================
     * SECTION : Scène 3D
     * =========================
     */

    // Nom de la scène principale du mode 3D.
    [Header("Scène 3D unique")]
    public string scene3DName = "PROJET_AOI";

    /*
     * Fonction : LoadScenePassed
     * Description :
     * Charge une scène spécifiée en paramètre.
     * 
     * Cette fonction :
     * - réinitialise les checkpoints
     * - désactive le démarrage spécial 3D
     * - charge la scène demandée
     * 
     * Paramètre :
     * - sceneName : nom de la scène à charger
     */
    public void LoadScenePassed(string sceneName)
    {
        // Réinitialise les checkpoints.
        CheckpointManager.ResetCheckpoint();

        /*
         * Désactive le démarrage spécial
         * du mode 3D.
         */
        PlayerPrefs.SetInt("DemarreDepuisMenu3D", 0);

        // Sauvegarde les données PlayerPrefs.
        PlayerPrefs.Save();

        // Charge la scène demandée.
        SceneManager.LoadScene(sceneName);
    }

    /*
     * Fonction : Load3DZone1
     * Description :
     * Lance le jeu 3D en commençant
     * à la zone 1.
     */
    public void Load3DZone1()
    {
        Load3DZone(1);
    }

    /*
     * Fonction : Load3DZone2
     * Description :
     * Lance le jeu 3D en commençant
     * à la zone 2.
     */
    public void Load3DZone2()
    {
        Load3DZone(2);
    }

    /*
     * Fonction : Load3DZone3
     * Description :
     * Lance le jeu 3D en commençant
     * à la zone 3.
     */
    public void Load3DZone3()
    {
        Load3DZone(3);
    }

    /*
     * Fonction : Load3DZone
     * Description :
     * Configure les données nécessaires
     * pour démarrer le mode 3D à partir
     * d'une zone spécifique.
     * 
     * Cette fonction :
     * - sauvegarde la zone de départ
     * - active le mode démarrage 3D
     * - réinitialise les checkpoints
     * - charge la scène 3D principale
     * 
     * Paramètre :
     * - zoneId : identifiant de la zone de départ
     */
    private void Load3DZone(int zoneId)
    {
        // Affiche des informations dans la console Unity.
        Debug.Log("Chargement scène 3D à partir de la zone : " + zoneId);

        // Sauvegarde la zone de départ du joueur.
        PlayerPrefs.SetInt("ZoneDepart3D", zoneId);

        // Active le mode démarrage depuis le menu 3D.
        PlayerPrefs.SetInt("DemarreDepuisMenu3D", 1);

        // Sauvegarde les données PlayerPrefs.
        PlayerPrefs.Save();

        // Réinitialise les checkpoints.
        CheckpointManager.ResetCheckpoint();

        // Charge la scène principale du mode 3D.
        SceneManager.LoadScene(scene3DName);
    }
}