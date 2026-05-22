using UnityEngine;

/*
 * Auteur : Timothy Chatelier
 * Date : -/03/2026
 * Projet : Alpha Omega Inversion - Jeu 2D
 *
 * Description globale :
 * Ce script gère le jet de feu créé par le lance-flammes.
 * Le jet disparaît automatiquement après une courte durée.
 * S'il touche un ennemi, il lui inflige des dégâts et disparaît.
 *
 * Informations pertinentes :
 * - Le jet doit avoir un Collider2D avec "Is Trigger" activé.
 * - Les ennemis doivent avoir le tag "Enemy".
 * - Les ennemis touchés doivent avoir le script AlienHealth pour recevoir des dégâts.
 */

public class FireJet : MonoBehaviour
{
    // Durée de vie du jet de feu avant sa destruction automatique.
    public float lifetime = 0.5f;  // Duree avant disparition

    // Détruit le jet après sa durée de vie.
    void Start()
    {
        // Detruit le jet apres sa duree de vie
        Destroy(gameObject, lifetime);
    }

    // Vérifie si le jet touche un ennemi, lui inflige des dégâts et détruit le jet.
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            // Trouve le script de vie de l'alien
            AlienHealth alienHealth = other.GetComponent<AlienHealth>();
            if (alienHealth != null)
                alienHealth.TakeDamage(1);

            Destroy(gameObject);
        }
    }
}