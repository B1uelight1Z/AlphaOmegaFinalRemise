using UnityEngine;

/*

 * Auteur : David Champagne
 * Date : 15/05/2026
 * Projet : Alpha Omega Inversion - Jeu 3D
 *
 * Description globale :
 * Ce script gère l'ambiance sonore d'une zone.
 * Lorsque le joueur entre dans la zone, l'audio démarre.
 * Lorsque le joueur quitte la zone, l'audio s'arrête si l'option jouerUneSeuleFois
 * n'est pas activée.
 *
 * Informations pertinentes :
 * - L'objet doit avoir un Collider avec "Is Trigger" activé.
 * - Le joueur doit avoir le tag "Player".
 * - Le script utilise un AudioSource assigné dans l'inspecteur ou récupéré automatiquement.
 */

public class ZoneAmbianceAudio : MonoBehaviour
{
    [Header("Audio de la zone")]
    // Source audio jouée lorsque le joueur entre dans la zone.
    public AudioSource audioZone;

    [Header("Options")]
    // Indique si l'audio doit seulement être joué une seule fois.
    public bool jouerUneSeuleFois = false;

    // Indique si l'audio a déjà été joué.
    private bool _dejaJoue = false;

    // Prépare la source audio et l'arrête au démarrage.
    private void Start()
    {
        if (audioZone == null)
        {
            audioZone = GetComponent<AudioSource>();
        }

        if (audioZone != null)
        {
            audioZone.playOnAwake = false;
            audioZone.loop = true;
            audioZone.Stop();
        }
    }

    // Lance l'audio lorsque le joueur entre dans la zone.
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        if (jouerUneSeuleFois && _dejaJoue)
        {
            return;
        }

        if (audioZone != null && !audioZone.isPlaying)
        {
            audioZone.Play();
            _dejaJoue = true;
        }
    }

    // Arrête l'audio lorsque le joueur quitte la zone, sauf si l'audio doit être joué une seule fois.
    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        if (audioZone != null && audioZone.isPlaying && !jouerUneSeuleFois)
        {
            audioZone.Stop();
        }
    }
}