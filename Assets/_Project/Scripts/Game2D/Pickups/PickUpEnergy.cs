using UnityEngine;

/*
 * Auteur : Timothy Chatelier, David Champagne
 * Date : 09/03/2026 - Modification 20/05/2026
 * Projet : Alpha Omega Inversion - Jeu 2D
 *
 * Description globale :
 * Ce script gère un objet d'énergie que le joueur peut ramasser.
 * Lorsque le joueur entre en contact avec l'objet, un son peut être joué,
 * l'énergie est ajoutée à l'inventaire et l'objet est désactivé.
 *
 * Informations pertinentes :
 * - Le script utilise Inventory pour ajouter l'énergie.
 * - L'objet est désactivé au lieu d'être détruit pour pouvoir être réactivé avec ResetEnergy.
 */

public class PickUpEnergy : MonoBehaviour
{
    // Son joué lorsque le joueur ramasse l'énergie.
    public AudioClip sound;

    // Empêche l'objet d'être ramassé plusieurs fois.
    private bool pickedUp = false;

    // Vérifie si le joueur touche l'objet, ajoute l'énergie à l'inventaire et désactive l'objet.
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (pickedUp)
        {
            return;
        }

        if (collision.CompareTag("Player"))
        {
            pickedUp = true;

            if (AudioManager.instance != null && sound != null)
            {
                AudioManager.instance.PlayClipAt(sound, transform.position);
            }

            if (Inventory.instance != null)
            {
                Inventory.instance.AddEnergy(1);
            }

            gameObject.SetActive(false);
        }
    }

    // Réinitialise l'objet afin qu'il puisse être ramassé de nouveau.
    public void ResetEnergy()
    {
        pickedUp = false;
        gameObject.SetActive(true);
    }
}