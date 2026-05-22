using UnityEngine;

/*
* Auteur : David Champagne
 * Date : 26/04/2026 - Dernière Mofication : 18/05/2026
  * Projet : Alpha Omega Inversion - Jeu 3D
 *
 * Description globale :
 * Ce script gère un bouton qui active deux portes coulissantes.
 * Lorsque le joueur est assez proche, un message d'interaction est affiché.
 * Si le joueur appuie sur F, les deux portes assignées sont activées.
 *
 * Informations pertinentes :
 * - Le joueur doit être assigné dans l'inspecteur.
 * - messageUI est affiché seulement lorsque le joueur est assez proche.
 * - Les portes gauche et droite doivent avoir le script PorteCoulissante.
 * - Un son peut être joué lors de l'interaction.
 */

public class BoutonDoublePorte : MonoBehaviour
{
    // Porte coulissante située à gauche.
    public PorteCoulissante porteGauche;

    // Porte coulissante située à droite.
    public PorteCoulissante porteDroite;

    // Référence vers le joueur.
    public Transform player;

    // Distance maximale à laquelle le joueur peut interagir avec le bouton.
    public float distanceInteraction = 1.5f;

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

    // Vérifie la distance du joueur, affiche le message et active les portes si le joueur appuie sur F.
    void Update()
    {
        if (player == null)
        {
            return;
        }

        if (messageUI == null)
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

                if (porteGauche != null)
                {
                    porteGauche.Activer();
                }

                if (porteDroite != null)
                {
                    porteDroite.Activer();
                }
            }
        }
        else
        {
            messageUI.SetActive(false);
        }
    }

    // Joue le son d'interaction du bouton si les références audio sont assignées.
    void JouerSonInteraction()
    {
        if (audioSource != null && sonInteraction != null)
        {
            audioSource.PlayOneShot(sonInteraction, volumeSon);
        }
    }
}