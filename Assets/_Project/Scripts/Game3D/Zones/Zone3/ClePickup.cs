using UnityEngine;

// Auteur: Timothy Chatelier
// Dernière date de modification: 22/05/2026
// Gère le ramassage d'une clé par le joueur.
// Ajoute la clé à l'inventaire, joue un son et désactive le collider après récupération.
public class ClePickup : MonoBehaviour
{
    [Header("Test rapide")]
    public bool isActivated = false; // Si vrai au démarrage, la clé est récupérée automatiquement

    [Header("Clé")]
    public string nomCle = "Cle1"; // Nom unique de la clé ajoutée à l'inventaire

    [Header("Message")]
    public string messageInteraction = "Appuyez sur F pour récupérer la clé"; // Message affiché quand le joueur est proche

    [Header("Son")]
    public AudioClip sonSucces; // Son joué lors de la récupération de la clé

    [Header("Après récupération")]
    public bool desactiverColliderApresPickup = true; // Si vrai, désactive le collider après ramassage pour éviter toute interaction future

    private AudioSource audioSource;       // Composant audio utilisé pour jouer le son de récupération
    private bool joueurAProximite = false; // Vrai si le joueur se trouve dans la zone de trigger
    private bool cleRecuperee = false;     // Vrai si la clé a déjà été ramassée

    // Initialise l'AudioSource et récupère automatiquement la clé si isActivated est vrai
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        if (isActivated)
        {
            RecupererCle();
        }
    }

    // Vérifie chaque frame si le joueur appuie sur E pour ramasser la clé
    void Update()
    {
        if (joueurAProximite && !cleRecuperee && Input.GetKeyDown(KeyCode.F))
        {
            RecupererCle();
        }
    }

    // Ajoute la clé à l'inventaire du joueur, joue le son, cache l'UI
    // et désactive le collider si l'option est activée
    void RecupererCle()
    {
        if (cleRecuperee) return;

        cleRecuperee = true;
        isActivated = true;

        // Cherche l'inventaire du joueur et y ajoute la clé
        InventaireJoueur inventaire = FindObjectOfType<InventaireJoueur>();
        if (inventaire != null)
        {
            inventaire.AjouterCle(nomCle);
        }
        else
        {
            Debug.LogWarning("InventaireJoueur introuvable.");
        }

        // Joue le son de récupération si disponible
        if (sonSucces != null && audioSource != null)
        {
            audioSource.PlayOneShot(sonSucces);
        }

        // Cache le message d'interaction
        if (UIManager.instance != null)
        {
            UIManager.instance.HideInteract();
        }

        joueurAProximite = false;

        // Désactive le collider pour éviter toute interaction future avec la clé
        if (desactiverColliderApresPickup)
        {
            Collider colliderCle = GetComponent<Collider>();
            if (colliderCle != null)
            {
                colliderCle.enabled = false;
            }
        }

        Debug.Log("Clé '" + nomCle + "' récupérée.");
    }

    // Détecte quand le joueur entre dans la zone et affiche le message d'interaction
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !cleRecuperee)
        {
            joueurAProximite = true;
            if (UIManager.instance != null)
            {
                UIManager.instance.ShowInteract(messageInteraction);
            }
        }
    }

    // Détecte quand le joueur quitte la zone et cache le message d'interaction
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            joueurAProximite = false;
            if (UIManager.instance != null)
            {
                UIManager.instance.HideInteract();
            }
        }
    }
}