using UnityEngine;

/*
 * Auteur : Timothy Chatelier
 * Date : 09/03/2026
 * Projet : Alpha Omega Inversion - Jeu 2D
 *
 * Description globale :
 * Ce script gère les points de vie d'un alien.
 * Lorsqu'il reçoit des dégâts, sa vie diminue.
 * Si sa vie atteint zéro, l'alien déclenche son animation de mort et est détruit.
 *
 */

public class AlienHealth : MonoBehaviour
{
    // Nombre de points de vie de l'alien.
    public int health = 3;

    // Animator utilisé pour déclencher l'animation de mort de l'alien.
    public Animator animator;

    // Retire des points de vie à l'alien et vérifie si sa vie est à zéro.
    public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log("👾 Alien touché ! Vie restante : " + health);

        if (health <= 0)
            Die();
    }

    // Déclenche l'animation de mort de l'alien et détruit l'objet après un court délai.
    void Die()
    {
        Debug.Log("💀 Alien éliminé !");
        animator.SetTrigger("Death");
        Destroy(gameObject, 1f);
    }
}