using System.Collections;
using UnityEngine;

// Auteur: David Champagne, Michael Proulx
// Dernière date de modification: 22/05/2026
// Gère les points de vie, les dégâts et la mort d'un ennemi.
// Déclenche des retours visuels (changement de couleur) et sonores lors des impacts ou du décès, et désactive l'entité avant sa destruction.
public class EnnemyHealth : MonoBehaviour
{
    [Header("Vie")]
    public float vieMax = 100f; // Quantité maximale de points de vie de l'ennemi
    private float _vie; // Quantité actuelle de points de vie de l'ennemi
    private bool _estMort = false; // Indique si l'ennemi est déjà mort pour éviter les calculs redondants

    [Header("Audio")]
    public AudioClip sonTouche; // Effet sonore joué lorsque l'ennemi reçoit un coup
    public AudioClip sonMort; // Effet sonore joué au moment où l'ennemi meurt
    public float volumeAudio = 1f; // Volume des effets sonores joués (entre 0 et 1)

    [Header("Feedback couleur")]
    public Color couleurTouche = Color.yellow; // Couleur de flash temporaire appliquée lorsque l'ennemi prend des dégâts
    public Color couleurMort = Color.red; // Couleur finale appliquée au cadavre de l'ennemi
    public float dureeCouleurTouche = 0.12f; // Temps en secondes pendant lequel la couleur d'impact reste visible
    public float dureeAvantDestruction = 0.25f; // Délai en secondes avant que le GameObject ne soit retiré de la scène après sa mort

    private Renderer[] _renderers; // Liste de tous les composants Renderer sur l'ennemi et ses enfants pour changer sa couleur
    private Color[] _couleursOriginales; // Tableau stockant les couleurs initiales des matériaux pour pouvoir les restaurer après un flash

    // Initialise les points de vie au maximum et sauvegarde les couleurs d'origine de tous les matériaux trouvés dans les enfants
    void Start()
    {
        _vie = vieMax;

        _renderers = GetComponentsInChildren<Renderer>();
        _couleursOriginales = new Color[_renderers.Length];

        for (int i = 0; i < _renderers.Length; i++)
        {
            Material mat = _renderers[i].material;

            if (mat.HasProperty("_Color"))
            {
                _couleursOriginales[i] = mat.color;
            }
            else if (mat.HasProperty("_BaseColor"))
            {
                _couleursOriginales[i] = mat.GetColor("_BaseColor");
            }
            else
            {
                _couleursOriginales[i] = Color.white;
            }
        }
    }

    // Applique les dégâts à l'ennemi, vérifie s'il doit mourir, et déclenche le feedback sonore et visuel d'impact
    public void PrendreDegats(float degats)
    {
        if (_estMort)
        {
            return;
        }

        _vie -= degats;
        Debug.Log($"{gameObject.name} : {_vie}/{vieMax} PV");

        if (_vie <= 0f)
        {
            Mourir();
        }
        else
        {
            if (sonTouche != null)
            {
                AudioSource.PlayClipAtPoint(sonTouche, transform.position, volumeAudio);
            }

            StartCoroutine(FlashCouleurTouche());
        }
    }

    // Coroutine qui applique la couleur de dégâts, attend le délai défini, puis restaure les couleurs d'origine de l'entité
    IEnumerator FlashCouleurTouche()
    {
        AppliquerCouleur(couleurTouche);

        yield return new WaitForSeconds(dureeCouleurTouche);

        RemettreCouleurOriginale();
    }

    // Désactive les scripts, les colliders, met à jour le score global, joue le son de mort et planifie la destruction de l'objet
    void Mourir()
    {
        if (_estMort)
        {
            return;
        }

        _estMort = true;

        Debug.Log($"{gameObject.name} est mort.");

        if (GameManager.instance != null)
        {
            GameManager.instance.AddScore(1);
        }

        if (sonMort != null)
        {
            AudioSource.PlayClipAtPoint(sonMort, transform.position, volumeAudio);
        }

        AppliquerCouleur(couleurMort);

        Collider[] colliders = GetComponentsInChildren<Collider>();

        foreach (Collider collider in colliders)
        {
            collider.enabled = false;
        }

        MonoBehaviour[] scripts = GetComponentsInChildren<MonoBehaviour>();

        foreach (MonoBehaviour script in scripts)
        {
            if (script != this)
            {
                script.enabled = false;
            }
        }

        Destroy(gameObject, dureeAvantDestruction);
    }

    // Parcourt tous les Renderers de l'ennemi pour modifier temporairement ou définitivement leur couleur principale selon les propriétés disponibles
    void AppliquerCouleur(Color couleur)
    {
        foreach (Renderer renderer in _renderers)
        {
            if (renderer == null)
            {
                continue;
            }

            Material mat = renderer.material;

            if (mat.HasProperty("_Color"))
            {
                mat.color = couleur;
            }
            else if (mat.HasProperty("_BaseColor"))
            {
                mat.SetColor("_BaseColor", couleur);
            }
        }
    }

    // Parcourt tous les Renderers pour restituer à chacun la couleur exacte sauvegardée lors de l'initialisation du script
    void RemettreCouleurOriginale()
    {
        for (int i = 0; i < _renderers.Length; i++)
        {
            if (_renderers[i] == null)
            {
                continue;
            }

            Material mat = _renderers[i].material;

            if (mat.HasProperty("_Color"))
            {
                mat.color = _couleursOriginales[i];
            }
            else if (mat.HasProperty("_BaseColor"))
            {
                mat.SetColor("_BaseColor", _couleursOriginales[i]);
            }
        }
    }
}