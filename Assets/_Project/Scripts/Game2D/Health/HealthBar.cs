using UnityEngine;
using UnityEngine.UI;

/*
 * Auteur : Michael Proulx
 * Date : 07/03/2026
 * Projet : Alpha Omega Inversion - Jeu 2D
 *
 * Description globale :
 * Ce script gère l'affichage de la barre de vie du joueur.
 *
 * Informations pertinentes :
 * - Le slider représente la quantité de vie actuelle du joueur.
 * - Le Gradient permet de changer la couleur de la barre selon la vie restante.
 * - L'Image fill représente la partie visuelle remplie de la barre de vie.
 */

public class HealthBar : MonoBehaviour
{
    // Slider utilisé pour afficher la quantité de vie du joueur.
    public Slider slider;

    // Dégradé de couleur utilisé selon le pourcentage de vie restant.
    public Gradient gradient;

    // Image remplie de la barre de vie qui change de couleur.
    public Image fill;

    // Initialise la barre de vie avec la vie maximale du joueur.
    public void SetMaxHealth(int health)
    {
        slider.maxValue = health;
        slider.value = health;

        fill.color = gradient.Evaluate(1f);
    }

    // Met à jour la barre de vie selon la vie actuelle du joueur.
    public void SetHealth(int health)
    {
        slider.value = health;

        fill.color = gradient.Evaluate(slider.normalizedValue);
    }

}