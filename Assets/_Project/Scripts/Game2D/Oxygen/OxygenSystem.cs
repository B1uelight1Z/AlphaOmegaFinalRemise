using UnityEngine;

/*
 * Auteur : Michael Proulx
 * Date : 07/03/2026
 * Projet : Alpha Omega Inversion - Jeu 2D
 *
 * Description globale :
 * Ce script gère le système d'oxygène du joueur.
 * L'oxygène diminue progressivement avec le temps.
 * Si l'oxygène atteint zéro, le joueur meurt.
 * Le script permet aussi d'ajouter de l'oxygène lorsque le joueur ramasse un objet.
 *
 * Informations pertinentes :
 * - Ce script utilise OxygenBar pour afficher l'oxygène dans l'interface.
 * - OxygenSystem utilise une instance statique pour être accessible par les power-ups.
 * - Le joueur doit avoir un système de vie PlayerHealth2D.
 */

public class OxygenSystem : MonoBehaviour
{
    // Quantité maximale d'oxygène du joueur.
    public float maxOxygen = 100f;

    // Quantité actuelle d'oxygène du joueur.
    public float currentOxygen;

    // Quantité d'oxygène perdue par seconde.
    public float oxygenLostPerSecond = 0.35f;

    // Référence vers la barre d'oxygène affichée dans l'interface.
    public OxygenBar oxygenBar;

    // Instance statique permettant d'accéder facilement au système d'oxygène.
    public static OxygenSystem instance;

    // Initialise l'instance et tente de trouver automatiquement la barre d'oxygène si elle n'est pas assignée.
    void Awake()
    {
        instance = this;

        if (oxygenBar == null)
        {
            oxygenBar = FindFirstObjectByType<OxygenBar>();

            if (oxygenBar == null)
            {
                Debug.LogError("OxygenBar introuvable !");
            }
        }
    }

    // Initialise l'oxygène du joueur à sa valeur maximale et met à jour la barre d'oxygène.
    void Start()
    {
        currentOxygen = maxOxygen;

        if (oxygenBar != null)
        {
            oxygenBar.SetMaxOxygen(maxOxygen);
        }
    }

    // Réduit l'oxygène du joueur à chaque frame.
    void Update()
    {
        BaisserOxygen();
    }

    // Diminue l'oxygène avec le temps, met à jour l'interface et tue le joueur si l'oxygène atteint zéro.
    void BaisserOxygen()
    {
        currentOxygen -= oxygenLostPerSecond * Time.deltaTime;

        if (currentOxygen < 0)
        {
            currentOxygen = 0;
        }

        if (oxygenBar != null)
        {
            oxygenBar.SetOxygen(currentOxygen);
        }

        if (currentOxygen <= 0)
        {
            PlayerHealth2D.instance.Die();
        }
    }

    // Ajoute de l'oxygène au joueur sans dépasser la quantité maximale.
    public void AddOxygen(float amount)
    {
        currentOxygen += amount;

        if (currentOxygen > maxOxygen)
        {
            currentOxygen = maxOxygen;
        }
            
        if (oxygenBar != null)
        {
            oxygenBar.SetOxygen(currentOxygen);
        }
    }
}