using UnityEngine;

/*
 * Nom du script : Spike_Damage
 * Auteur : Michael Proulx
 * Date : 03/03/2026
 * Projet : AlphaOmega Inversion - Jeu 2D
 * 
 * Description globale :
 * Ce script permet de gérer les dégâts causés
 * par des pics dangereux dans le jeu.
 * 
 * Lorsqu'un joueur entre en collision avec les pics,
 * des dégâts lui sont automatiquement infligés.
 * 
 * Informations pertinentes :
 * - Le joueur doit posséder le tag "Player".
 * - Le script PlayerHealth2D est utilisé pour gérer
 *   la vie du joueur.
 * - Les dégâts peuvent être modifiés dans l'inspecteur Unity.
 */

public class Spike_Damage : MonoBehaviour
{
    // Quantité de dégâts infligés lors d'une collision avec les pics.
    public int damageOnCollision = 100;

    /*
     * Fonction : OnCollisionEnter2D
     * Description :
     * Détecte les collisions entre les pics
     * et d'autres objets de la scène.
     * 
     * Si l'objet touché possède le tag "Player",
     * le joueur reçoit des dégâts.
     */
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Vérifie si l'objet touché est le joueur.
        if (collision.transform.CompareTag("Player"))
        {
            // Récupère le script de vie du joueur.
            PlayerHealth2D playerHealth2D = collision.transform.GetComponent<PlayerHealth2D>();

            // Inflige des dégâts au joueur.
            playerHealth2D.TakeDamage(damageOnCollision);
        }
    }
}