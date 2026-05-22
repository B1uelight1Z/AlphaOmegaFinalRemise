using UnityEngine;

/*
 * Auteur : David Champagne
* Date : 26/04/2026 - Dernière Mofication : 18/05/2026
* Projet : Alpha Omega Inversion - Jeu 3D
 *
 * Description globale :
 * Ce script gère le déplacement d'un ascenseur entre une position basse et une position haute.
 * Il permet de faire monter ou descendre l'ascenseur et de jouer des sons lors du départ,
 * du mouvement et de l'arrivée.
 *
 * Informations pertinentes :
 * - positionBas et positionHaut doivent être assignés dans l'inspecteur.
 * - L'ascenseur utilise Vector3.MoveTowards pour se déplacer progressivement.
 * - Une source audio peut être utilisée pour jouer les sons associés à l'ascenseur.
 */


public class Ascenseur : MonoBehaviour
{
    // Position basse de l'ascenseur.
    public Transform positionBas;

    // Position haute de l'ascenseur.
    public Transform positionHaut;

    // Vitesse de déplacement de l'ascenseur.
    public float vitesse = 2f;

    [Header("Audio")]
    // Source audio utilisée pour jouer les sons de l'ascenseur.
    public AudioSource audioSource;

    // Son joué au départ de l'ascenseur.
    public AudioClip sonDepart;

    // Son joué lorsque l'ascenseur atteint sa destination.
    public AudioClip sonArrivee;

    // Son joué pendant le déplacement de l'ascenseur.
    public AudioClip sonMouvement;

    // Volume des sons de l'ascenseur.
    public float volumeSon = 1f;

    // Position vers laquelle l'ascenseur se déplace.
    private Vector3 destination;

    // Indique si l'ascenseur est actuellement en mouvement.
    private bool estEnMouvement = false;

    // Place l'ascenseur à sa position basse et récupère l'AudioSource si nécessaire.
    void Start()
    {
        transform.position = positionBas.position;
        destination = positionBas.position;

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    // Déplace l'ascenseur vers sa destination lorsqu'il est en mouvement.
    void Update()
    {
        if (!estEnMouvement)
        {
            return;
        }

        transform.position = Vector3.MoveTowards(
            transform.position,
            destination,
            vitesse * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, destination) < 0.01f)
        {
            estEnMouvement = false;

            if (audioSource != null)
            {
                audioSource.Stop();

                if (sonArrivee != null)
                {
                    audioSource.PlayOneShot(sonArrivee, volumeSon);
                }
            }
        }
    }

    // Définit la destination vers la position haute et démarre le mouvement.
    public void Monter()
    {
        destination = positionHaut.position;
        DemarrerAscenseur();
    }

    // Définit la destination vers la position basse et démarre le mouvement.
    public void Descendre()
    {
        destination = positionBas.position;
        DemarrerAscenseur();
    }

    // Active le déplacement de l'ascenseur et joue les sons de départ et de mouvement.
    void DemarrerAscenseur()
    {
        estEnMouvement = true;

        if (audioSource == null)
        {
            return;
        }

        if (sonDepart != null)
        {
            audioSource.PlayOneShot(sonDepart, volumeSon);
        }

        if (sonMouvement != null)
        {
            audioSource.clip = sonMouvement;
            audioSource.loop = true;
            audioSource.volume = volumeSon;
            audioSource.Play();
        }
    }
}