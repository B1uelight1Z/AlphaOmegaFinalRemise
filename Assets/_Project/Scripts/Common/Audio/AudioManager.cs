using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

/*
 * Nom du script : AudioManager
 * Auteur : Michael Proulx
 * Date : 03/03/2026
 * Projet : AlphaOmega Inversion - Jeu 2D
 * 
 * Description globale :
 * Ce script permet de gérer la musique et les effets sonores
 * dans le jeu.
 * 
 * Le gestionnaire :
 * - joue la musique de la scène
 * - configure automatiquement l'AudioSource
 * - permet de jouer des effets sonores en 3D
 * - utilise un système Singleton pour être accessible
 *   depuis les autres scripts
 * 
 * Informations pertinentes :
 * - Le script utilise AudioMixerGroup pour les effets sonores.
 * - Les effets sonores temporaires sont créés dynamiquement.
 * - La musique est jouée en boucle automatiquement.
 * - Les autres scripts peuvent utiliser :
 *      AudioManager.instance.PlayClipAt(...)
 */

public class AudioManager : MonoBehaviour
{
    // Instance globale du gestionnaire audio.
    public static AudioManager instance;

    /*
     * =========================
     * SECTION : Musique
     * =========================
     */

    // Musique utilisée dans la scène actuelle.
    [Header("Musique de la scène")]
    public AudioClip musiqueScene;

    /*
     * =========================
     * SECTION : Audio
     * =========================
     */

    // AudioSource principale utilisée pour la musique.
    [Header("Audio")]
    public AudioSource audioSource;

    // Mixer utilisé pour les effets sonores.
    public AudioMixerGroup soundEffectMixer;

    /*
     * Fonction : Awake
     * Description :
     * Initialise le système audio et configure
     * l'instance Singleton.
     * 
     * Cette fonction :
     * - empêche plusieurs AudioManager
     * - récupère ou crée un AudioSource
     * - configure les paramètres audio
     */
    private void Awake()
    {
        /*
         * Vérifie si une autre instance existe déjà.
         * 
         * Si oui, détruit cet objet afin d'éviter
         * les doublons.
         */
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // Définit cette instance comme gestionnaire principal.
        instance = this;

        // Tente de récupérer un AudioSource existant.
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        /*
         * Si aucun AudioSource n'existe,
         * en crée un automatiquement.
         */
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Configure la musique pour jouer en boucle.
        audioSource.loop = true;

        // Empêche la lecture automatique au chargement.
        audioSource.playOnAwake = false;

        /*
         * SpatialBlend à 0 :
         * le son est joué en 2D.
         */
        audioSource.spatialBlend = 0f;
    }

    /*
     * Fonction : Start
     * Description :
     * Configure et démarre la musique de la scène.
     * 
     * Cette fonction :
     * - récupère l'AudioSource
     * - applique la musique
     * - démarre la lecture
     */
    private void Start()
    {
        // Récupère l'AudioSource attaché à l'objet.
        audioSource = GetComponent<AudioSource>();

        /*
         * Si aucun AudioSource n'existe,
         * en crée un automatiquement.
         */
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Définit la musique de la scène.
        audioSource.clip = musiqueScene;

        // Active la lecture en boucle.
        audioSource.loop = true;

        // Désactive la lecture automatique Unity.
        audioSource.playOnAwake = false;

        // Définit le son comme 2D.
        audioSource.spatialBlend = 0f;

        // Lance la musique.
        audioSource.Play();
    }

    /*
     * Fonction : PlayClipAt
     * Description :
     * Joue un effet sonore à une position précise
     * dans la scène.
     * 
     * Cette fonction :
     * - crée un objet audio temporaire
     * - joue le son en 3D
     * - détruit automatiquement l'objet après lecture
     * 
     * Paramètres :
     * - clip : son à jouer
     * - pos : position dans la scène
     * 
     * Retour :
     * - retourne l'AudioSource créée
     */
    public AudioSource PlayClipAt(AudioClip clip, Vector3 pos)
    {
        // Vérifie qu'un clip audio existe.
        if (clip == null)
        {
            return null;
        }

        // Crée un objet temporaire pour jouer le son.
        GameObject tempGO = new GameObject("TempAudio");

        // Positionne l'objet dans la scène.
        tempGO.transform.position = pos;

        // Ajoute un AudioSource à l'objet temporaire.
        AudioSource tempAudioSource = tempGO.AddComponent<AudioSource>();

        // Définit le clip audio.
        tempAudioSource.clip = clip;

        // Applique le mixer des effets sonores.
        tempAudioSource.outputAudioMixerGroup = soundEffectMixer;

        /*
         * SpatialBlend à 1 :
         * le son est joué en 3D.
         */
        tempAudioSource.spatialBlend = 1f;

        // Lance le son.
        tempAudioSource.Play();

        /*
         * Détruit automatiquement l'objet
         * après la durée du son.
         */
        Destroy(tempGO, clip.length);

        // Retourne l'AudioSource créée.
        return tempAudioSource;
    }
}