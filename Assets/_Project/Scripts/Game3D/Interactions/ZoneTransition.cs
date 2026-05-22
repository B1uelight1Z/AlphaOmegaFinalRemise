using System.Collections;
using UnityEngine;
using TMPro;

/*
 * Auteur : David Champagne
 * Date : 17/05/2026
 * Projet : Alpha Omega Inversion - Jeu 3D
 *
 * Description globale :
 * Ce script gère l'affichage d'un texte de transition lorsque le joueur entre dans une nouvelle zone.
 * Il peut aussi jouer un son de transition.
 * Le texte apparaît progressivement, reste affiché pendant quelques secondes, puis disparaît progressivement.
 *
 * Informations pertinentes :
 * - L'objet doit avoir un Collider avec "Is Trigger" activé.
 * - Le joueur doit avoir le tag "Player".
 * - Le script utilise TextMeshProUGUI pour afficher le nom ou la description de la zone.
 */

public class ZoneTransition : MonoBehaviour
{
    [Header("Audio")]
    // Source audio utilisée pour jouer le son de transition.
    public AudioSource audioSource;

    // Son joué lorsque le joueur entre dans la zone de transition.
    public AudioClip sonTransition;

    [Header("Texte")]
    // Texte affiché lors de la transition entre les zones.
    public TextMeshProUGUI texteZone;

    // Message affiché au joueur lorsqu'il entre dans la zone.
    public string messageZone = "Zone 2 - Salle d'ingénierie";

    // Durée pendant laquelle le texte reste visible avant de disparaître.
    public float dureeAffichage = 2f;

    // Durée de l'effet de fondu à l'apparition et à la disparition.
    public float dureeFade = 1f;

    // Empêche la transition de se déclencher plusieurs fois.
    private bool _dejaActive = false;

    // Déclenche la transition lorsque le joueur entre dans la zone.
    private void OnTriggerEnter(Collider other)
    {
        if (_dejaActive)
        {
            return;
        }

        if (!other.CompareTag("Player"))
        {
            return;
        }

        _dejaActive = true;
        StartCoroutine(JouerTransition());
    }

    // Joue le son, affiche le texte avec un fondu, attend, puis cache le texte avec un fondu.
    private IEnumerator JouerTransition()
    {
        if (audioSource != null && sonTransition != null)
        {
            audioSource.PlayOneShot(sonTransition);
        }

        if (texteZone == null)
        {
            yield break;
        }

        texteZone.text = messageZone;

        Color couleur = texteZone.color;
        couleur.a = 0f;
        texteZone.color = couleur;

        texteZone.gameObject.SetActive(true);

        float temps = 0f;

        while (temps < dureeFade)
        {
            temps += Time.deltaTime;
            couleur.a = Mathf.Lerp(0f, 1f, temps / dureeFade);
            texteZone.color = couleur;
            yield return null;
        }

        yield return new WaitForSeconds(dureeAffichage);

        temps = 0f;

        while (temps < dureeFade)
        {
            temps += Time.deltaTime;
            couleur.a = Mathf.Lerp(1f, 0f, temps / dureeFade);
            texteZone.color = couleur;
            yield return null;
        }

        texteZone.gameObject.SetActive(false);
    }
}