using UnityEngine;

/*
 * Auteur : Timothy Chatelier
 * Date : 18/05/2026
 * Projet : Alpha Omega Inversion - Jeu 3D
 *
 * Description globale :
 * Ce script gère une interaction avec un intercom.
 * Lorsque le joueur est proche, un message d'interaction est affiché.
 * Si le joueur appuie sur E, un message d'intercom apparaît temporairement à l'écran.
 *
 * Informations pertinentes :
 * - L'objet doit avoir un Collider avec "Is Trigger" activé.
 * - Le joueur doit avoir le tag "Player".
 * - Le script utilise UIManager pour afficher et cacher les messages.
 */

public class Intercom : MonoBehaviour
{
    [Header("Messages")]
    // Message affiché lorsque le joueur peut interagir avec l'intercom.
    public string messageInteraction = "Appuyez sur F pour interagir";

    // Message principal affiché après l'interaction avec l'intercom.
    [TextArea(3, 6)]
    public string messageIntercom = "Attention, mission en cours. Restez vigilant.";

    // Durée d'affichage du message d'intercom.
    public float dureeMesage = 4f;

    // Indique si le joueur est dans la zone d'interaction.
    private bool joueurAProximite = false;

    // Empêche de réactiver l'intercom pendant que son message est déjà affiché.
    private bool dejaActive = false;

    // Vérifie si le joueur appuie sur E lorsqu'il est proche de l'intercom.
    void Update()
    {
        if (joueurAProximite && Input.GetKeyDown(KeyCode.F) && !dejaActive)
            AfficherMessage();
    }

    // Affiche le message de l'intercom et programme sa disparition.
    void AfficherMessage()
    {
        dejaActive = true;
        UIManager.instance.HideInteract();
        UIManager.instance.ShowIntercom(messageIntercom); // ← affiche seulement
        Invoke(nameof(CacherMessage), dureeMesage);
    }

    // Cache le message de l'intercom et permet une nouvelle activation.
    void CacherMessage()
    {
        UIManager.instance.HideIntercom(); // ← corrigé
        dejaActive = false;
    }

    // Affiche le message d'interaction lorsque le joueur entre dans la zone.
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            joueurAProximite = true;
            UIManager.instance.ShowInteract(messageInteraction);
        }
    }

    // Cache le message d'interaction lorsque le joueur quitte la zone.
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            joueurAProximite = false;
            UIManager.instance.HideInteract();
        }
    }
}