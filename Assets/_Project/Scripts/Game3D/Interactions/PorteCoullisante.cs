using UnityEngine;

/*
 * Auteur : David Champagne
 * Date : 24/04/2026
 * Projet : AlphaOmega Inversion - Jeu 3D
 *
 * Description globale :
 * Ce script gère une porte coulissante qui peut s'ouvrir et se fermer.
 * Lorsqu'elle est activée, la porte change de destination et se déplace progressivement
 * entre sa position fermée et sa position ouverte.
 *
 * Informations pertinentes :
 * - positionFermee et positionOuverte doivent être assignées dans l'inspecteur.
 * - Le script utilise Vector3.MoveTowards pour déplacer la porte.
 * - Des sons peuvent être joués lors de l'ouverture et de la fermeture.
 */

//David Champagne

public class PorteCoulissante : MonoBehaviour
{
    // Position de la porte lorsqu'elle est fermée.
    public Transform positionFermee;

    // Position de la porte lorsqu'elle est ouverte.
    public Transform positionOuverte;

    // Vitesse de déplacement de la porte.
    public float vitesse = 3f;

    [Header("Audio")]
    // Source audio utilisée pour jouer les sons de la porte.
    public AudioSource audioSource;

    // Son joué lorsque la porte s'ouvre.
    public AudioClip sonOuverture;

    // Son joué lorsque la porte se ferme.
    public AudioClip sonFermeture;

    // Volume des sons de la porte.
    public float volumeSon = 1f;

    // Indique si la porte est actuellement ouverte.
    private bool estOuverte = false;

    // Position vers laquelle la porte doit se déplacer.
    private Vector3 destination;

    // Initialise la porte à sa position fermée et récupère l'AudioSource si nécessaire.
    void Start()
    {
        destination = positionFermee.position;
        transform.position = destination;

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    // Déplace continuellement la porte vers sa destination actuelle.
    void Update()
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            destination,
            vitesse * Time.deltaTime
        );
    }

    // Alterne entre l'ouverture et la fermeture de la porte, puis joue le son correspondant.
    public void Activer()
    {
        if (estOuverte)
        {
            destination = positionFermee.position;
            estOuverte = false;

            if (audioSource != null && sonFermeture != null)
            {
                audioSource.PlayOneShot(sonFermeture, volumeSon);
            }
        }
        else
        {
            destination = positionOuverte.position;
            estOuverte = true;

            if (audioSource != null && sonOuverture != null)
            {
                audioSource.PlayOneShot(sonOuverture, volumeSon);
            }
        }
    }
}