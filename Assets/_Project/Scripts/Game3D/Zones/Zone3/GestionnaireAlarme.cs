using UnityEngine;
using TMPro;
using System.Collections;

// Auteur: Timothy Chatelier
// Dernière date de modification: 22/05/2026
// Gère le système d'alarme du jeu : déclenchement, décompte, changement de lumières
// et réinitialisation. Provoque un Game Over si le décompte atteint zéro.
public class GestionnaireAlarme : MonoBehaviour
{
    public static GestionnaireAlarme instance; // Référence statique unique accessible depuis tous les scripts

    [Header("Son")]
    public AudioClip sonAlarme; // Clip audio joué en boucle pendant l'alarme
    private AudioSource _audio; // Composant audio utilisé pour jouer le son d'alarme

    [Header("Décompte")]
    public float tempsDecompte = 3f;            // Durée en secondes avant le Game Over après déclenchement
    public TextMeshProUGUI texteDecompte;        // Texte UI affichant le décompte à l'écran

    [Header("Lumières")]
    public Light[] lumieresAlerte;              // Tableau de lumières à changer lors de l'alarme
    public Color couleurNormale = Color.blue;   // Couleur des lumières en état normal
    public Color couleurAlarme = Color.red;     // Couleur des lumières pendant l'alarme

    private bool _alarmeActive = false;         // Vrai si l'alarme est actuellement en cours
    private Coroutine _coroutineDecompte;       // Référence à la coroutine de décompte pour pouvoir l'arrêter

    // Initialise le singleton
    void Awake()
    {
        instance = this;
    }

    // Initialise l'AudioSource, configure la boucle du son d'alarme,
    // cache le texte de décompte et met les lumières en couleur normale
    void Start()
    {
        _audio = GetComponent<AudioSource>();
        if (_audio == null)
        {
            _audio = gameObject.AddComponent<AudioSource>();
        }
        _audio.loop = true;
        _audio.clip = sonAlarme;

        if (texteDecompte != null)
        {
            texteDecompte.gameObject.SetActive(false);
        }

        SetCouleurLumieres(couleurNormale);
    }

    // Propriété publique en lecture seule pour vérifier si l'alarme est active
    public bool AlarmeActive => _alarmeActive;

    // Déclenche l'alarme : joue le son, change les lumières et démarre le décompte
    public void DeclencherAlarme()
    {
        if (_alarmeActive) return;

        _alarmeActive = true;

        if (sonAlarme != null && _audio != null)
        {
            _audio.Play();
        }

        SetCouleurLumieres(couleurAlarme);
        _coroutineDecompte = StartCoroutine(Decompte());
        Debug.Log("Alarme déclenchée !");
    }

    // Désactive l'alarme en appelant la réinitialisation complète
    public void DesactiverAlarme()
    {
        ReinitialiserAlarme();
        Debug.Log("Alarme désactivée !");
    }

    // Remet l'alarme à son état initial : arrête le son, les lumières,
    // le décompte et réinitialise les caméras de surveillance
    public void ReinitialiserAlarme()
    {
        _alarmeActive = false;

        if (_audio != null)
        {
            _audio.Stop();
        }

        SetCouleurLumieres(couleurNormale);

        if (_coroutineDecompte != null)
        {
            StopCoroutine(_coroutineDecompte);
            _coroutineDecompte = null;
        }

        if (texteDecompte != null)
        {
            texteDecompte.text = "";
            texteDecompte.gameObject.SetActive(false);
        }

        // Réinitialise la couleur de détection de toutes les caméras de surveillance
        SurveillanceCamera[] cameras = FindObjectsByType<SurveillanceCamera>(FindObjectsSortMode.None);
        foreach (SurveillanceCamera camera in cameras)
        {
            camera.ResetCouleurDetection();
        }
    }

    // Affiche un décompte à l'écran et déclenche le Game Over une fois le temps écoulé
    IEnumerator Decompte()
    {
        float tempsRestant = tempsDecompte;

        if (texteDecompte != null)
        {
            texteDecompte.gameObject.SetActive(true);
        }

        while (tempsRestant > 0)
        {
            if (texteDecompte != null)
            {
                texteDecompte.text = "ALARME ! " + Mathf.CeilToInt(tempsRestant);
            }
            tempsRestant -= Time.deltaTime;
            yield return null;
        }

        // Nettoie l'alarme avant de déclencher le Game Over
        _coroutineDecompte = null;
        ReinitialiserAlarme();

        // Tue le joueur ou appelle directement le Game Over si PlayerHealth est absent
        if (PlayerHealth.instance != null)
        {
            PlayerHealth.instance.KillPlayer();
        }
        else if (GameOverManager.instance != null)
        {
            Debug.LogWarning("PlayerHealth introuvable. Game Over appelé directement.");
            GameOverManager.instance.OnPlayerDeath();
        }
    }

    // Applique une couleur donnée à toutes les lumières d'alerte
    void SetCouleurLumieres(Color couleur)
    {
        foreach (Light lumiere in lumieresAlerte)
        {
            if (lumiere != null)
            {
                lumiere.color = couleur;
            }
        }
    }
}