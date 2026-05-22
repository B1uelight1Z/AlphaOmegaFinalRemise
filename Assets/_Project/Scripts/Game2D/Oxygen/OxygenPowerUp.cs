using UnityEngine;

/*
 * Auteur : Michael Proulx
 * Date : 07/03/2026
 * Projet : Alpha Omega Inversion - Jeu 2D
 *
 * Description globale :
 * Ce script gère un objet de récupération d'oxygène.
 * Lorsque le joueur entre en contact avec l'objet, un son est joué,
 * l'oxygène du joueur augmente et l'objet disparaît.
 *
 * Informations pertinentes :
 * - Le script utilise OxygenSystem pour ajouter de l'oxygène au joueur.
 */

public class OxygenPowerUp : MonoBehaviour
{
    // Son joué lorsque le joueur ramasse l'objet d'oxygène.
    public AudioClip sound;

    // Quantité d'oxygène redonnée au joueur.
    public float oxygenAmount = 30f;

    // Vérifie si le joueur touche l'objet, ajoute de l'oxygène, joue un son et détruit l'objet.
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            AudioManager.instance.PlayClipAt(sound, transform.position);
            OxygenSystem.instance.AddOxygen(oxygenAmount);
            Destroy(gameObject);
        }
    }
}