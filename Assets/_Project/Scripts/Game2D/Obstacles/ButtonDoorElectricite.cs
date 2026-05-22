using UnityEngine;
using UnityEngine.InputSystem;

/*
 * Nom du script : ElectricityButton
 * Auteur : David Champagne
 * Date : 03/03/2026
 * Projet : AlphaOmega Inversion - Jeu 2D
 * 
 * Description globale :
 * Ce script permet de contrôler un bouton interactif dans le jeu.
 * 
 * Lorsque le joueur s'approche du bouton et appuie sur la touche F,
 * le système électrique associé est désactivé.
 * 
 * Le script change également :
 * - l'apparence du bouton
 * - l'indicateur de porte
 * - joue un effet sonore
 * 
 * Informations pertinentes :
 * - Le joueur doit posséder le tag "Player".
 * - Le système utilise le nouveau Input System de Unity.
 * - Le bouton ne peut être activé qu'une seule fois.
 * - AudioManager.instance est utilisé pour jouer le son.
 */

public class ElectricityButton : MonoBehaviour
{
    // Objet électrique qui sera désactivé après l'activation du bouton.
    public GameObject electricityToDisable;

    // Sprite du bouton lorsqu'il est désactivé.
    public Sprite redButton;

    // Sprite du bouton lorsqu'il est activé.
    public Sprite greenButton;

    // Sprite utilisé pour indiquer que la porte est activée.
    public Sprite IndicateurPortegreenButton;

    // Référence vers le SpriteRenderer de l'indicateur de porte.
    public SpriteRenderer IndicateurPorte;

    // Son joué lorsque le bouton est activé.
    public AudioClip sound;

    // Référence vers le SpriteRenderer du bouton.
    private SpriteRenderer spriteRenderer;

    // Vérifie si le joueur est assez proche pour interagir avec le bouton.
    private bool playerInRange = false;

    // Vérifie si le bouton a déjà été activé.
    private bool activated = false;

    /*
     * Fonction : Start
     * Description :
     * Initialise le SpriteRenderer du bouton
     * et applique le sprite rouge par défaut.
     * 
     * Cette fonction est appelée automatiquement
     * au début de la scène.
     */
    void Start()
    {
        // Récupère le SpriteRenderer attaché à l'objet.
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Définit le sprite initial du bouton.
        spriteRenderer.sprite = redButton;
    }

    /*
     * Fonction : Update
     * Description :
     * Vérifie à chaque frame si le joueur est proche
     * du bouton et appuie sur la touche F.
     * 
     * Si toutes les conditions sont respectées :
     * - le son est joué
     * - l'électricité est désactivée
     * - les sprites sont changés
     * - le bouton devient définitivement activé
     */
    void Update()
    {
        // Vérifie si le joueur peut activer le bouton.
        if (playerInRange && !activated && Keyboard.current.fKey.wasPressedThisFrame)
        {
            // Joue le son d'activation.
            AudioManager.instance.PlayClipAt(sound, transform.position);

            // Désactive l'objet électrique.
            electricityToDisable.SetActive(false);

            // Change le sprite du bouton.
            spriteRenderer.sprite = greenButton;

            // Change l'indicateur de porte.
            IndicateurPorte.sprite = IndicateurPortegreenButton;

            // Empêche une nouvelle activation.
            activated = true;
        }
    }

    /*
     * Fonction : OnTriggerEnter2D
     * Description :
     * Détecte lorsque le joueur entre dans la zone
     * d'interaction du bouton.
     * 
     * Permet au joueur d'activer le bouton.
     */
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Vérifie si l'objet entrant est le joueur.
        if (collision.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    /*
     * Fonction : OnTriggerExit2D
     * Description :
     * Détecte lorsque le joueur quitte la zone
     * d'interaction du bouton.
     * 
     * Le joueur ne peut alors plus activer le bouton.
     */
    private void OnTriggerExit2D(Collider2D collision)
    {
        // Vérifie si l'objet sortant est le joueur.
        if (collision.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}