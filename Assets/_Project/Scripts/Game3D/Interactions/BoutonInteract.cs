using UnityEngine;
using TMPro;

/*
 * Auteur : Michael Proulx, David Champagne
 * Date : 24/04/2026 - Dernière modification: 18/05/2026
 * Projet : Alpha Omega Inversion - Jeu 3D
 *
 * Description globale :
 * Ce script gère un bouton interactif dans la scène 3D.
 * Lorsque le joueur entre dans la zone du bouton, un message d'interaction est affiché.
 * Si le joueur appuie sur F, le bouton s'active, change de couleur et joue un son.
 *
 * Informations pertinentes :
 * - L'objet doit avoir un Collider avec "Is Trigger" activé.
 * - Le joueur doit avoir le tag "Player".
 * - Le script utilise TextMeshProUGUI pour afficher le message d'interaction.
 */

public class BoutonInteract : MonoBehaviour
{
    [Header("État")]
    // Indique si le bouton a déjà été activé.
    public bool isActivated = false;

    // Indique si le joueur est actuellement dans la zone d'interaction.
    private bool playerInRange = false;

    [Header("Audio")]
    // Source audio utilisée pour jouer le son d'activation.
    public AudioSource audioSource;

    // Son joué lorsque le bouton est activé.
    public AudioClip sonActivation;

    // Volume du son d'activation.
    public float volumeSon = 1f;

    [Header("Visuel")]
    // Couleur appliquée au bouton lorsqu'il est activé.
    public Color couleurActive = Color.green;

    [Header("UI Interaction")]
    // Texte affiché lorsque le joueur peut interagir avec le bouton.
    public TextMeshProUGUI texteInteraction;

    // Message affiché au joueur lorsqu'il est proche du bouton.
    public string messageInteraction = "Appuyez sur F pour activer";

    // Récupère l'AudioSource si nécessaire, cache le message d'interaction et applique l'état visuel si le bouton est déjà activé.
    void Start()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        if (texteInteraction != null)
        {
            texteInteraction.gameObject.SetActive(false);
        }

        if (isActivated)
        {
            AppliquerEtatVisuel();
        }
    }

    // Vérifie si le joueur peut activer le bouton et maintient l'état visuel si le bouton est activé.
    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.F) && !isActivated)
        {
            ActiverBouton();
        }

        if (isActivated)
        {
            AppliquerEtatVisuel();
        }
    }

    // Active le bouton, applique son visuel, joue un son et cache le message d'interaction.
    void ActiverBouton()
    {
        isActivated = true;

        Debug.Log(gameObject.name + " : bouton activé.");

        AppliquerEtatVisuel();

        if (audioSource != null && sonActivation != null)
        {
            audioSource.PlayOneShot(sonActivation, volumeSon);
        }

        CacherMessageInteraction();
    }

    // Change la couleur du renderer du bouton pour montrer qu'il est activé.
    void AppliquerEtatVisuel()
    {
        Renderer renderer = GetComponent<Renderer>();

        if (renderer == null)
        {
            renderer = GetComponentInChildren<Renderer>();
        }

        if (renderer != null)
        {
            renderer.material.color = couleurActive;
        }
    }

    // Affiche le message d'interaction au joueur.
    void AfficherMessageInteraction()
    {
        if (texteInteraction == null)
        {
            return;
        }

        texteInteraction.text = messageInteraction;
        texteInteraction.gameObject.SetActive(true);
    }

    // Cache le message d'interaction.
    void CacherMessageInteraction()
    {
        if (texteInteraction == null)
        {
            return;
        }

        texteInteraction.gameObject.SetActive(false);
    }

    // Détecte l'entrée du joueur dans la zone d'interaction du bouton.
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isActivated)
        {
            playerInRange = true;
            AfficherMessageInteraction();
        }
    }

    // Détecte la sortie du joueur de la zone d'interaction du bouton.
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            CacherMessageInteraction();
        }
    }
}