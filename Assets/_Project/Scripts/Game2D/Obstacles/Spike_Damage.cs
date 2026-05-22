using UnityEngine;

/*
 * Nom du script : Obstacles_Damage
 * Auteur : Michael Proulx
 * Date : 03/03/2026
 * Projet : AlphaOmega Inversion - Jeu 2D
 * 
 * Description globale :
 * Ce script permet de gérer les dégâts causés
 * par différents obstacles dangereux dans le jeu.
 * 
 * Lorsqu'un joueur entre en collision avec l'obstacle,
 * le script applique des dégâts au joueur selon
 * le système de vie utilisé.
 * 
 * Informations pertinentes :
 * - Le joueur doit posséder le tag "Player".
 * - Les dégâts peuvent être modifiés dans l'inspecteur Unity.
 * - Le script vérifie automatiquement quel système
 *   de vie est présent sur le joueur.
 */

public class Obstacles_Damage : MonoBehaviour
{
    // Quantité de dégâts infligés lors d'une collision.
    public int damageOnCollision = 100;

    /*
     * Fonction : OnCollisionEnter2D
     * Description :
     * Détecte les collisions entre l'obstacle
     * et d'autres objets de la scène.
     * 
     * Si l'objet touché possède le tag "Player",
     * le script tente d'appliquer des dégâts
     * au système de vie du "Player".
     */
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Vérifie si l'objet touché est le joueur.
        if (collision.transform.CompareTag("Player"))
        {
            /*
             * Tente de récupérer le script PlayerHealth2D
             * présent sur le joueur.
             */
            PlayerHealth2D playerHealth2D =
                collision.transform.GetComponent<PlayerHealth2D>();

            // Vérifie que le script existe avant d'appliquer des dégâts.
            if (playerHealth2D != null)
            {
                // Inflige des dégâts au joueur.
                playerHealth2D.TakeDamage(damageOnCollision);
            }

            /*
             * Tente de récupérer le script
             * PlayerHealth2D_NoGravitySwitch.
             */
            PlayerHealth2D_NoGravitySwitch playerHealthNoGravity =
                collision.transform.GetComponent<PlayerHealth2D_NoGravitySwitch>();

            // Vérifie que le script existe avant d'appliquer des dégâts.
            if (playerHealthNoGravity != null)
            {
                // Inflige des dégâts au joueur.
                playerHealthNoGravity.TakeDamage(damageOnCollision);
            }
        }
    }
}