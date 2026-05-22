using UnityEngine;

/*
 * Auteur : David Champagne
 * Date : 16/05/2026
 * Projet : Alpha Omega Inversion - Jeu 3D
 *
 * Description globale :
 * Ce script crée un effet de lumière brisé ou de court-circuit sur une lumière.
 * L'intensité de la lumière change à intervalles aléatoires pour donner un effet
 * instable ou inquiétant dans l'environnement.
 *
 * Informations pertinentes :
 * - Le script utilise un composant Light.
 * - Si aucune lumière n'est assignée dans l'inspecteur, le script tente de récupérer
 *   automatiquement le composant Light sur le même GameObject.
 * - L'option eteindreParMoment permet de créer des coupures rapides de lumière.
 */

public class LightFlicker : MonoBehaviour
{
    [Header("Référence")]
    // Référence vers la lumière qui doit scintiller.
    public Light lumiere;

    [Header("Intensité")]
    // Intensité minimale possible de la lumière.
    public float intensiteMin = 0.5f;

    // Intensité maximale possible de la lumière.
    public float intensiteMax = 4f;

    [Header("Timing")]
    // Temps minimum avant le prochain changement d'intensité.
    public float tempsMin = 0.05f;

    // Temps maximum avant le prochain changement d'intensité.
    public float tempsMax = 0.25f;

    [Header("Options")]
    // Permet à la lumière de s'éteindre complètement par moments.
    public bool eteindreParMoment = true;

    // Probabilité que la lumière s'éteigne lors d'un changement.
    public float chanceEteindre = 0.15f;

    // Moment où le prochain changement d'intensité doit avoir lieu.
    private float _tempsProchainChangement = 0f;

    // Récupère automatiquement le composant Light si aucune référence n'a été assignée.
    void Start()
    {
        if (lumiere == null)
        {
            lumiere = GetComponent<Light>();
        }
    }

    // Vérifie si le moment est venu de changer l'intensité de la lumière.
    void Update()
    {
        if (lumiere == null)
        {
            return;
        }

        if (Time.time < _tempsProchainChangement)
        {
            return;
        }

        ChangerLumiere();

        _tempsProchainChangement = Time.time + Random.Range(tempsMin, tempsMax);
    }

    // Change l'intensité de la lumière ou l'éteint temporairement selon les options.
    void ChangerLumiere()
    {
        if (eteindreParMoment && Random.value < chanceEteindre)
        {
            lumiere.intensity = 0f;
            return;
        }

        lumiere.intensity = Random.Range(intensiteMin, intensiteMax);
    }
}