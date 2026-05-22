using UnityEngine;

// Auteur: Timothy Chatelier
// Derniere date de modification: 22/05/2026
// Infliche des degats continus au joueur tant qu'il reste a l'interieur de la zone de trigger.
// Gère un delai de recuperation (cooldown) entre chaque blessure pour eviter de tuer le joueur instantanement.
public class PlayerDamage : MonoBehaviour
{
    public int damage = 10; // Quantite de points de vie soustraits au joueur a chaque blessure
    public float damageCooldown = 1f; // Temps d'attente minimal requis en secondes entre deux attaques

    private float lastDamageTime; // Timestamp memoire de la derniere fois ou le joueur a encaisse des degats

    // Verifie en continu la presence du joueur dans la zone de danger pour lui appliquer des degats periodiques
    private void OnTriggerStay(Collider other)
    {
        // Ignore la collision si l'objet detecte dans la zone n'est pas le joueur
        if (!other.CompareTag("Player"))
        {
            return;
        }

        // Bloque l'attaque si le delai de recuperation n'est pas encore completement ecoule
        if (Time.time < lastDamageTime + damageCooldown)
        {
            return;
        }

        // Tente de recuperer le composant de gestion de vie situe sur le joueur ou ses parents
        PlayerHealth player = other.GetComponentInParent<PlayerHealth>();

        // Si le script est trouve, inflige les degats et reinitialise le chronometre de cooldown
        if (player != null)
        {
            player.TakeDamage(damage);
            lastDamageTime = Time.time;
        }
    }
}