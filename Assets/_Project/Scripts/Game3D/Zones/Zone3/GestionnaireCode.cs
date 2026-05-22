using UnityEngine;
using System.Collections;

// Auteur: Timothy Chatelier
// Dernière date de modification: 22/05/2026
// Gère la logique du puzzle de code : vérifie la séquence d'appareils activés,
// ouvre la porte si la combinaison est correcte et réinitialise en cas d'erreur.
public class GestionnaireCode : MonoBehaviour
{
    public static GestionnaireCode instance; // Référence statique unique accessible depuis tous les scripts

    [Header("Combinaison correcte (ex: 3,1,4,2)")]
    public int[] combinaisonCorrecte; // Séquence d'identifiants d'appareils à activer dans le bon ordre

    [Header("Porte à ouvrir")]
    public PorteCoulissante porte; // Référence à la porte déverrouillée quand la combinaison est réussie

    [Header("Sons")]
    public AudioClip sonBonneConsole;   // Son joué quand le bon appareil est activé
    public AudioClip sonMauvaiseConsole; // Son joué quand le mauvais appareil est activé

    private int[] tentative;        // Tableau stockant la séquence entrée par le joueur
    private int etapeActuelle = 0;  // Index de l'étape actuelle dans la combinaison
    public bool resolu = false;     // Vrai si la combinaison a été résolue avec succès
    private AudioSource _audio;     // Composant audio utilisé pour jouer les sons de feedback

    // Initialise le singleton et prépare le tableau de tentative selon la longueur de la combinaison
    void Awake()
    {
        instance = this;
        tentative = new int[combinaisonCorrecte.Length];
    }

    // Initialise l'AudioSource au démarrage
    void Start()
    {
        _audio = gameObject.AddComponent<AudioSource>();
    }

    // Traite l'appui sur un appareil : vérifie si l'id correspond à la bonne étape,
    // avance la progression ou réinitialise en cas d'erreur
    public void AppuyerAppareil(int id)
    {
        if (resolu) return;

        if (id == combinaisonCorrecte[etapeActuelle])
        {
            tentative[etapeActuelle] = id;
            etapeActuelle++;
            JouerSon(sonBonneConsole);
            AfficherIndicateur();

            // Vérifie si toute la combinaison a été complétée
            if (etapeActuelle >= combinaisonCorrecte.Length)
            {
                resolu = true;
                Debug.Log("Combinaison correcte !");
                porte.Activer();
                UIManager.instance.HideCode();
            }
        }
        else
        {
            // Mauvaise console : joue le son d'erreur et réinitialise la tentative
            JouerSon(sonMauvaiseConsole);
            StartCoroutine(Reinitialiser());
        }
    }

    // Met à jour l'affichage de l'indicateur de progression dans l'UI
    void AfficherIndicateur()
    {
        UIManager.instance.ShowCode(GetIndicateur());
    }

    // Affiche brièvement un message d'erreur, puis remet la tentative à zéro après un court délai
    IEnumerator Reinitialiser()
    {
        UIManager.instance.ShowCode("Code : X X X X");
        yield return new WaitForSeconds(0.8f);

        etapeActuelle = 0;
        tentative = new int[combinaisonCorrecte.Length];
        AfficherIndicateur();
        Debug.Log("Combinaison réinitialisée.");
    }

    // Joue un clip audio en one-shot si le clip est valide
    void JouerSon(AudioClip clip)
    {
        if (clip != null)
            _audio.PlayOneShot(clip);
    }

    // Retourne une chaîne affichant "O" pour chaque étape réussie et "_" pour les étapes restantes
    public string GetIndicateur()
    {
        string affichage = "Code : ";
        for (int i = 0; i < combinaisonCorrecte.Length; i++)
        {
            if (i < etapeActuelle)
                affichage += "O";
            else
                affichage += "_";

            if (i < combinaisonCorrecte.Length - 1)
                affichage += " ";
        }
        return affichage;
    }
}