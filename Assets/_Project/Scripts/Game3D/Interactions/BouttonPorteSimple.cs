using UnityEngine;

/*
 * Auteur : David Champagne
 * Date : 28/04/2026
 * Projet : Alpha Omega Inversion - Jeu 3D
 *
 * Description globale :
 * Ce script gère un bouton qui active une porte coulissante.
 * Lorsque le joueur est assez proche, un message d'interaction est affiché.
 * Si le joueur appuie sur F, la porte assignée est activée.
 *
 * Informations pertinentes :
 * - Le joueur, la porte et le message UI doivent être assignés dans l'inspecteur.
 * - La porte doit utiliser le script PorteCoulissante.
 * - Un son peut être joué lors de l'interaction.
 */

public class BoutonPorteSimple : MonoBehaviour
{
    // Porte coulissante activée par ce bouton.
    public PorteCoulissante porte;

    // Référence vers le joueur.
    public Transform player;

    // Distance maximale à laquelle le joueur peut interagir avec le bouton.
    public float distanceInteraction = 3f;

    // Message affiché lorsque le joueur peut interagir.
    public GameObject messageUI;

    [Header("Audio")]
    // Source audio utilisée pour jouer le son d'interaction.
    public AudioSource audioSource;

    // Son joué lorsque le joueur interagit avec le bouton.
    public AudioClip sonInteraction;

    // Volume du son d'interaction.
    public float volumeSon = 1f;

    // Récupère l'AudioSource si elle n'est pas assignée.
    void Start()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    // Vérifie la distance du joueur, affiche le message et active la porte lorsque le joueur appuie sur F.
    void Update()
    {
        if (player == null || porte == null || messageUI == null)
        {
            return;
        }

        float distance = Vector3.Distance(player.position, transform.position);

        if (distance <= distanceInteraction)
        {
            messageUI.SetActive(true);

            if (Input.GetKeyDown(KeyCode.F))
            {
                JouerSonInteraction();
                porte.Activer();
            }
        }
        else
        {
            messageUI.SetActive(false);
        }
    }

    // Joue le son d'interaction si les références audio sont assignées.
    void JouerSonInteraction()
    {
        if (audioSource != null && sonInteraction != null)
        {
            audioSource.PlayOneShot(sonInteraction, volumeSon);
        }
    }
}