using UnityEngine;
using TMPro;

/*
 * Auteur : David Champagne
 * Date : 26/04/2026 - Dernière Mofication : 18/05/2026
 * Projet : Alpha Omega Inversion - Jeu 3D
 *
 * Description globale :
 * Ce script gère le bouton d'activation d'un ascenseur.
 * Lorsque le joueur est assez proche, un message d'interaction est affiché.
 * Si tous les boutons nécessaires de la zone sont activés, le joueur peut activer
 * l'ascenseur avec la touche F.
 *
 * Informations pertinentes :
 * - Le joueur doit avoir le tag "Player".
 * - Le script utilise ZoneButtonManager pour vérifier si tous les boutons sont activés.
 * - Le script utilise TextMeshProUGUI pour afficher les messages d'interaction.
 * - Des sons peuvent être joués lorsque l'ascenseur est activé ou refusé.
 */


public class AscenseurBtnScript : MonoBehaviour
{
    // Référence vers l'ascenseur contrôlé par ce bouton.
    public Ascenseur ascenseur;

    // Gestionnaire qui vérifie l'état des boutons de la zone.
    public ZoneButtonManager zoneButtonManager;

    // Distance maximale à laquelle le joueur peut interagir avec le bouton.
    public float distanceInteraction = 3f;

    [Header("Audio")]
    // Source audio utilisée pour jouer les sons du bouton.
    public AudioSource audioSource;

    // Son joué lorsque l'ascenseur est activé.
    public AudioClip sonBouton;

    // Son joué lorsque l'ascenseur est encore verrouillé.
    public AudioClip sonRefus;

    // Volume des sons joués par le bouton.
    public float volumeSon = 1f;

    [Header("UI Interaction")]
    // Texte affiché pour informer le joueur de l'action possible.
    public TextMeshProUGUI texteInteraction;

    // Message affiché lorsque l'ascenseur peut être activé.
    public string messagePret = "Appuyez sur F pour activer l’ascenseur";

    // Message affiché lorsque l'ascenseur est verrouillé.
    public string messageVerrouille = "Ascenseur verrouillé. Boutons restants : ";

    // Référence vers la position du joueur.
    private Transform player;

    // Cherche le joueur, récupère l'AudioSource si nécessaire et cache le texte d'interaction au départ.
    void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogError("Le tag Player non trouvé.");
        }

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        if (texteInteraction != null)
        {
            texteInteraction.gameObject.SetActive(false);
        }
    }

    // Vérifie la distance entre le joueur et le bouton, puis permet l'interaction avec la touche F.
    void Update()
    {
        if (player == null || ascenseur == null || zoneButtonManager == null)
        {
            CacherMessageInteraction();
            return;
        }

        float distance = Vector3.Distance(player.position, transform.position);

        if (distance <= distanceInteraction)
        {
            AfficherMessageInteraction();

            if (Input.GetKeyDown(KeyCode.F))
            {
                EssayerActiverAscenseur();
            }
        }
        else
        {
            CacherMessageInteraction();
        }
    }

    // Tente d'activer l'ascenseur si tous les boutons nécessaires sont activés.
    void EssayerActiverAscenseur()
    {
        if (zoneButtonManager.TousLesBoutonsSontActives())
        {
            Debug.Log("Ascenseur activé.");

            if (audioSource != null && sonBouton != null)
            {
                audioSource.PlayOneShot(sonBouton, volumeSon);
            }

            CacherMessageInteraction();
            ascenseur.Monter();
        }
        else
        {
            Debug.Log("Ascenseur verrouillé. Boutons restants : " + zoneButtonManager.GetBoutonsRestants());

            if (audioSource != null && sonRefus != null)
            {
                audioSource.PlayOneShot(sonRefus, volumeSon);
            }

            AfficherMessageInteraction();
        }
    }

    // Affiche le message d'interaction approprié selon l'état des boutons de la zone.
    void AfficherMessageInteraction()
    {
        if (texteInteraction == null)
        {
            return;
        }

        if (zoneButtonManager.TousLesBoutonsSontActives())
        {
            texteInteraction.text = messagePret;
        }
        else
        {
            texteInteraction.text = messageVerrouille + zoneButtonManager.GetBoutonsRestants();
        }

        texteInteraction.gameObject.SetActive(true);
    }

    // Cache le message d'interaction.
    void CacherMessageInteraction()
    {
        if (texteInteraction != null)
        {
            texteInteraction.gameObject.SetActive(false);
        }
    }
}