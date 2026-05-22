using UnityEngine;

/*
 * Auteur : Michael Proulx
 * Date : 08/03/2026
 * Projet : Alpha Omega Inversion - Jeu 2D
 *
 * Description globale :
 * Ce script gère le ramassage d'un œuf par le joueur.
 * Lorsque le joueur entre en contact avec l'objet, un son est joué,
 * le score d'œufs augmente et l'objet est détruit.
 *
 * Informations pertinentes :
 * - L'objet doit avoir un Collider2D avec "Is Trigger" activé.
 * - Le joueur doit avoir le tag "Player".
 * - Le script utilise Inventory pour ajouter un œuf au score.
 */

public class PickUpObject : MonoBehaviour
{
    // Son joué lorsque le joueur ramasse l'objet.
    public AudioClip sound;

    // Vérifie si le joueur touche l'objet, joue un son, ajoute un œuf au score et détruit l'objet.
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            AudioManager.instance.PlayClipAt(sound, transform.position);
            Inventory.instance.AddEgg(1);
            Destroy(gameObject);
        }
    }
}