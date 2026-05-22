using UnityEngine;

/*
 * Auteur : Timothy Chatelier
 * Date : 10/03/2026
 * Projet : Alpha Omega Inversion - Jeu 2D
 *
 * Description globale :
 * Ce script gère le ramassage du lance-flammes par le joueur.
 * Lorsque le joueur entre en contact avec l'objet, le lance-flammes est équipé,
 * un son peut être joué et l'objet ramassable est détruit.
 *
 * Informations pertinentes :
 * - L'objet doit avoir un Collider2D avec "Is Trigger" activé.
 * - Le joueur doit avoir le tag "Player".
 */

public class PickUpFlamethrower : MonoBehaviour
{
    // Son joué lorsque le joueur ramasse le lance-flammes.
    public AudioClip pickupSound;

    // Vérifie si le joueur touche le lance-flammes, l'équipe, joue un son et détruit l'objet.
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Équipe le lance-flammes.
            Flamethrower ft = other.GetComponentInChildren<Flamethrower>();
            if (ft != null)
                ft.Equip();

            if (AudioManager.instance != null && pickupSound != null)
                AudioManager.instance.PlayClipAt(pickupSound, transform.position);

            Destroy(gameObject);
        }
    }
}