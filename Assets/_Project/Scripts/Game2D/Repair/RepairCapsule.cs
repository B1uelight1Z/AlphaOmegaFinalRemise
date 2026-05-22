using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

/*
 * Nom du script : RepairCapsule
 * Auteur : Timothy Chatelier
 * Date : 03/03/2026
 * Projet : AlphaOmega Inversion - Jeu 2D
 * 
 * Description globale :
 * Ce script permet de gérer la réparation d'une capsule
 * dans le jeu.
 * 
 * Le joueur peut interagir avec la capsule en utilisant
 * de l'énergie afin de la réparer.
 * 
 * Lorsque la capsule est réparée :
 * - son apparence change
 * - un effet lumineux est activé
 * - un son est joué
 * - le RepairCapsuleManager est notifié
 * 
 * Informations pertinentes :
 * - Le joueur doit posséder le tag "Player".
 * - Le système utilise le nouveau Input System de Unity.
 * - L'énergie du joueur est gérée par Inventory.
 * - Le script fonctionne avec RepairCapsuleManager.
 * - Une capsule réparée ne peut plus être réparée à nouveau.
 */

public class RepairCapsule : MonoBehaviour
{
    // Permet de réparer automatiquement la capsule au démarrage.
    [Header("Test rapide")]
    public bool reparerAuDemarrage = false;

    /*
     * =========================
     * SECTION : Interaction
     * =========================
     */

    // Interface affichée lorsque le joueur peut interagir.
    [Header("Interaction")]
    public GameObject interactPrompt;

    // Message affiché lorsque le joueur manque d'énergie.
    public GameObject noEnergyPrompt;

    // Quantité d'énergie nécessaire pour réparer la capsule.
    public int energyCost = 1;

    /*
     * =========================
     * SECTION : Visuels
     * =========================
     */

    // Sprite de la capsule brisée.
    [Header("Visuels")]
    public Sprite brokenSprite;

    // Sprite de la capsule réparée.
    public Sprite repairedSprite;

    // Référence vers le SpriteRenderer de la capsule.
    private SpriteRenderer spriteRenderer;

    /*
     * =========================
     * SECTION : Effets
     * =========================
     */

    // Son joué lors de la réparation.
    [Header("Effets")]
    public AudioClip repairSound;

    // Effet lumineux activé lorsque la capsule est réparée.
    public GameObject repairedLightFX;

    /*
     * =========================
     * SECTION : État
     * =========================
     */

    // Vérifie si la capsule est réparée.
    [Header("État")]
    public bool isRepaired = false;

    /*
     * =========================
     * SECTION : Position réparée
     * =========================
     */

    // Décalage appliqué à la capsule lorsqu'elle est réparée.
    [Header("Position réparée")]
    public Vector3 repairedPositionOffset = new Vector3(0.39f, 1.08f, 0);

    // Vérifie si le joueur est proche de la capsule.
    private bool playerInRange = false;

    // Vérifie si le manager a déjà été averti.
    private bool managerDejaNotifie = false;

    /*
     * Fonction : Start
     * Description :
     * Initialise les composantes et l'état visuel
     * de la capsule.
     * 
     * Cette fonction :
     * - désactive les interfaces inutiles
     * - configure les effets visuels
     * - applique l'état réparé ou brisé
     */
    void Start()
    {
        // Récupère le SpriteRenderer attaché à l'objet.
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Cache le message d'interaction.
        if (interactPrompt != null)
        {
            interactPrompt.SetActive(false);
        }

        // Cache le message d'énergie insuffisante.
        if (noEnergyPrompt != null)
        {
            noEnergyPrompt.SetActive(false);
        }

        // Désactive l'effet lumineux au départ.
        if (repairedLightFX != null)
        {
            repairedLightFX.SetActive(false);
        }

        /*
         * Vérifie si la capsule doit commencer
         * déjà réparée.
         */
        if (isRepaired || reparerAuDemarrage)
        {
            AppliquerEtatRepare(false);
        }
        else
        {
            AppliquerEtatBrise();
        }
    }

    /*
     * Fonction : Update
     * Description :
     * Vérifie si le joueur tente de réparer
     * la capsule.
     * 
     * Si le joueur possède assez d'énergie :
     * - l'énergie est consommée
     * - la capsule est réparée
     * 
     * Sinon :
     * - un message d'erreur est affiché
     */
    void Update()
    {
        // Vérifie si le joueur peut interagir avec la capsule.
        if (playerInRange && !isRepaired)
        {
            // Vérifie si la touche E est pressée.
            if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
            {
                // Vérifie si le joueur possède assez d'énergie.
                if (Inventory.energyCount >= energyCost)
                {
                    // Retire l'énergie nécessaire.
                    Inventory.instance.AddEnergy(-energyCost);

                    // Lance la réparation.
                    RepairCapsuleAction();
                }
                else
                {
                    // Affiche un message si le joueur manque d'énergie.
                    if (noEnergyPrompt != null)
                    {
                        StartCoroutine(ShowNoEnergyMessage());
                    }
                }
            }
        }
    }

    /*
     * Fonction : RepairCapsuleAction
     * Description :
     * Lance le processus de réparation
     * de la capsule.
     */
    void RepairCapsuleAction()
    {
        AppliquerEtatRepare(true);
    }

    /*
     * Fonction : AppliquerEtatRepare
     * Description :
     * Applique tous les changements visuels
     * et logiques liés à l'état réparé.
     * 
     * Cette fonction :
     * - change le sprite
     * - active les effets
     * - joue un son
     * - notifie le manager
     */
    void AppliquerEtatRepare(bool jouerSon)
    {
        // Vérifie si la capsule a déjà été traitée.
        if (isRepaired && managerDejaNotifie)
        {
            return;
        }

        // Vérifie si la capsule vient juste d'être réparée.
        bool vientDEtreReparee = !isRepaired;

        // Active l'état réparé.
        isRepaired = true;

        // Sauvegarde l'état réparé pour le démarrage.
        reparerAuDemarrage = true;

        // Déplace légèrement la capsule réparée.
        if (vientDEtreReparee)
        {
            transform.position += repairedPositionOffset;
        }

        // Change le sprite de la capsule.
        if (spriteRenderer != null && repairedSprite != null)
        {
            spriteRenderer.sprite = repairedSprite;
        }

        // Active l'effet lumineux.
        if (repairedLightFX != null)
        {
            repairedLightFX.SetActive(true);
        }

        // Joue le son de réparation.
        if (jouerSon && AudioManager.instance != null && repairSound != null)
        {
            AudioManager.instance.PlayClipAt(repairSound, transform.position);
        }

        // Cache les interfaces.
        if (interactPrompt != null)
        {
            interactPrompt.SetActive(false);
        }

        if (noEnergyPrompt != null)
        {
            noEnergyPrompt.SetActive(false);
        }

        // Désactive l'interaction avec le joueur.
        playerInRange = false;

        /*
         * Informe le manager qu'une capsule
         * a été réparée.
         */
        if (!managerDejaNotifie && RepairCapsuleManager.instance != null)
        {
            managerDejaNotifie = true;

            RepairCapsuleManager.instance.CapsuleReparee();
        }
    }

    /*
     * Fonction : AppliquerEtatBrise
     * Description :
     * Réinitialise la capsule dans son état brisé.
     */
    void AppliquerEtatBrise()
    {
        // Désactive l'état réparé.
        isRepaired = false;

        // Réinitialise la notification du manager.
        managerDejaNotifie = false;

        // Applique le sprite brisé.
        if (spriteRenderer != null && brokenSprite != null)
        {
            spriteRenderer.sprite = brokenSprite;
        }

        // Désactive l'effet lumineux.
        if (repairedLightFX != null)
        {
            repairedLightFX.SetActive(false);
        }
    }

    /*
     * Fonction : ShowNoEnergyMessage
     * Description :
     * Affiche temporairement un message indiquant
     * que le joueur ne possède pas assez d'énergie.
     */
    private System.Collections.IEnumerator ShowNoEnergyMessage()
    {
        // Affiche le message.
        noEnergyPrompt.SetActive(true);

        // Attend 2 secondes.
        yield return new WaitForSeconds(2f);

        // Cache le message.
        noEnergyPrompt.SetActive(false);
    }

    /*
     * Fonction : OnTriggerEnter2D
     * Description :
     * Détecte lorsque le joueur entre dans
     * la zone d'interaction de la capsule.
     */
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Vérifie si l'objet entrant est le joueur.
        if (other.CompareTag("Player"))
        {
            // Autorise l'interaction.
            playerInRange = true;

            // Affiche le message d'interaction.
            if (!isRepaired && interactPrompt != null)
            {
                interactPrompt.SetActive(true);
            }
        }
    }

    /*
     * Fonction : OnTriggerExit2D
     * Description :
     * Détecte lorsque le joueur quitte
     * la zone d'interaction de la capsule.
     */
    private void OnTriggerExit2D(Collider2D other)
    {
        // Vérifie si l'objet sortant est le joueur.
        if (other.CompareTag("Player"))
        {
            // Désactive l'interaction.
            playerInRange = false;

            // Cache le message d'interaction.
            if (interactPrompt != null)
            {
                interactPrompt.SetActive(false);
            }

            // Cache le message d'erreur.
            if (noEnergyPrompt != null)
            {
                noEnergyPrompt.SetActive(false);
            }
        }
    }
}