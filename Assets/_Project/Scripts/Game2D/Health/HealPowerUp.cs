using UnityEngine;

/*
 * Auteur : Michael Proulx
 * Date : 07/03/2026
 * Projet : Alpha Omega Inversion - Jeu 2D
 *
 * Description globale :
 * Ce script gère un objet de soin que le joueur peut ramasser.
 * Lorsque le joueur entre en contact avec l'objet, il récupère de la vie
 * si sa vie n'est pas déjà au maximum.
 *
 * Informations pertinentes :
 * - L'objet doit avoir un Collider2D avec "Is Trigger" activé.
 * - Le script utilise PlayerHealth2D pour soigner le joueur.
 * - Un son est joué lorsque le power-up est ramassé.
 */

public class HealPowerUp : MonoBehaviour
{
    // Nombre de points de vie redonnés au joueur.
    public int healthPoints = 50;

    // Son joué lorsque le joueur ramasse le power-up.
    public AudioClip pickPowerUpSound;
    
    // Vérifie si le joueur touche le power-up et lui redonne de la vie si nécessaire.
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            
            if(PlayerHealth2D.instance.currentHealth != PlayerHealth2D.instance.maxHealth)
            {
                AudioManager.instance.PlayClipAt(pickPowerUpSound, transform.position);
                PlayerHealth2D.instance.HealPlayer(healthPoints);
                Destroy(gameObject);
            }
        }
    } 
}