using UnityEngine;
using UnityEngine.SceneManagement;

/*
 * Nom du script : DontDestroyOnLoadScene
 * Auteur : Michael Proulx
 * Date : 03/03/2026
 * Projet : AlphaOmega Inversion - Jeu 2D
 * 
 * Description globale :
 * Ce script permet de conserver certains objets
 * entre les changements de scène.
 * 
 * Les objets ajoutés dans le tableau "objects"
 * ne seront pas détruits lors du chargement
 * d'une nouvelle scène.
 * 
 * Le script permet également de retirer ces objets
 * du système DontDestroyOnLoad afin de les replacer
 * dans la scène active.
 * 
 * Informations pertinentes :
 * - Le script utilise un système Singleton.
 * - Les objets à conserver doivent être ajoutés
 *   dans l'inspecteur Unity.
 * - DontDestroyOnLoad est utile pour :
 *      - les gestionnaires globaux
 *      - la musique
 *      - les données persistantes
 *      - les systèmes de sauvegarde
 */

public class DontDestroyOnLoadScene : MonoBehaviour
{
    // Liste des objets qui doivent survivre aux changements de scène.
    public GameObject[] objects;

    // Instance globale du gestionnaire.
    public static DontDestroyOnLoadScene instance;

    /*
     * Fonction : Awake
     * Description :
     * Initialise l'instance Singleton
     * et applique DontDestroyOnLoad
     * aux objets sélectionnés.
     * 
     * Cette fonction est appelée avant Start.
     */
    void Awake()
    {
        // Vérifie si une autre instance existe déjà.
        if(instance != null)
        {
            Debug.LogWarning(
                "Il y a plus d'une instance de DontDestroyOnLoadScene dans la scène"
            );
        }

        // Définit cette instance comme gestionnaire principal.
        instance = this;

        /*
         * Parcourt tous les objets du tableau
         * afin de les rendre persistants
         * entre les scènes.
         */
        foreach(var element in objects)
        {
            // Empêche la destruction de l'objet lors d'un changement de scène.
            DontDestroyOnLoad(element);
        }
    }

    /*
     * Fonction : RemoveFromDontDestroyOnLoad
     * Description :
     * Retire les objets du système DontDestroyOnLoad
     * et les replace dans la scène actuellement active.
     * 
     * Cette fonction est utile lorsqu'un objet
     * ne doit plus être conservé globalement.
     */
    public void RemoveFromDontDestroyOnLoad()
    {
        // Parcourt tous les objets persistants.
        foreach(var element in objects)
        {
            /*
             * Déplace l'objet dans la scène active.
             * 
             * Cela retire l'objet de la scène spéciale
             * DontDestroyOnLoad.
             */
            SceneManager.MoveGameObjectToScene(
                element,
                SceneManager.GetActiveScene()
            );
        }
    }
}