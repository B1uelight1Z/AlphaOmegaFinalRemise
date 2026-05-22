using UnityEngine;

/*
 * Auteur : Michael Proulx
 * Date : 05/03/2026
 * Projet : Alpha Omega Inversion - Jeu 2D
 *
 * Description globale :
 * Ce script gère le point faible d'un alien.
 * Lorsque le joueur touche ce point faible (collider), il rebondit vers le haut et l'alien meurt.
 *
 * Informations pertinentes :
 * - Ce script doit être placé sur un objet enfant de l'alien.
 * - L'objet doit avoir un Collider2D avec "Is Trigger" activé.
 */

public class WeakSpot : MonoBehaviour
{
    // Force verticale appliquée au joueur lorsqu'il touche le point faible.
    public float bounceForce = 8f;

    // Son joué lorsque le joueur touche le point faible.
    public AudioClip sound;

    // Référence vers le script de déplacement de l'alien parent.
    private Alien_Movement alien;

    // Empêche le point faible d'être activé plusieurs fois.
    private bool triggered = false;

    // Récupère le script Alien_Movement présent sur l'objet parent.
    void Start()
    {
        alien = GetComponentInParent<Alien_Movement>();
    }

    // Vérifie si le joueur touche le point faible, applique un rebond, joue un son et élimine l'alien.
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (triggered) return;

        if (collision.CompareTag("Player"))
        {
            triggered = true;

            Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();

            if (rb != null)
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, bounceForce);

            if (alien != null)
            {
                AudioManager.instance.PlayClipAt(sound, transform.position);
                alien.Die();
            }
                
        }
    }
}