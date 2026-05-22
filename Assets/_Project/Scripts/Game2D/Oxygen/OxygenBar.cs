using UnityEngine;
using UnityEngine.UI;

/*
 * Auteur : Michael Proulx
 * Date : 07/03/2026
 * Projet : Alpha Omega Inversion - Jeu 2D
 *
 * Description globale :
 * Ce script gère l'affichage de la barre d'oxygène du joueur.
 * Il met à jour la valeur du Slider.
 *
 * Informations pertinentes :
 * - Le Slider représente la quantité d'oxygène actuelle du joueur.
 * - L'Image fill représente la partie remplie de la barre d'oxygène.
 */

public class OxygenBar : MonoBehaviour
{
    // Slider utilisé pour afficher la quantité d'oxygène.
    public Slider slider;

    // Dégradé de couleur utilisé selon le pourcentage d'oxygène restant.
    public Gradient gradient;

    // Image remplie de la barre d'oxygène.
    public Image fill;

    // Vérifie si le Slider est assigné. Sinon, tente de le récupérer automatiquement sur l'objet.
    void Awake()
    {
        if (slider == null)
        {
            slider = GetComponent<Slider>();
            if (slider == null)
                Debug.LogError("OxygenBar: Aucun Slider assigné !");
        }
    }

    // Initialise la barre d'oxygène avec la quantité maximale.
    public void SetMaxOxygen(float oxygen)
    {
        if (slider != null)
        {
            slider.maxValue = oxygen;
            slider.value = oxygen;

            if (fill != null && gradient != null)
                fill.color = gradient.Evaluate(1f);
        }
    }

    // Met à jour la barre d'oxygène selon la quantité actuelle.
    public void SetOxygen(float oxygen)
    {
        if (slider != null)
        {
            slider.value = oxygen;

            if (fill != null && gradient != null)
                fill.color = gradient.Evaluate(slider.normalizedValue);
        }
    }
}