using UnityEngine;

// Auteur: Timothy Chatelier
// Derni�re date de modification: 22/05/2026
// G�re l'appareil final du jeu qui n�cessite toutes les cl�s pour �tre activ�.
// V�rifie l'inventaire du joueur et d�clenche la fin du jeu si toutes les cl�s sont pr�sentes.
public class AppareilFinal : MonoBehaviour
{
    [Header("Cl�s requises")]
    public string[] clesRequises; // Liste des noms de cl�s n�cessaires pour activer l'appareil

    [Header("Sons")]
    public AudioClip sonSucces; // Son jou� quand l'activation r�ussit
    public AudioClip sonEchec;  // Son jou� quand il manque des cl�s

    [Header("Message")]
    public string messageInteraction = "Appuyez sur F pour ins�rer les cl�s"; // Message affich� quand le joueur est proche
    public string messageErreur = "Il vous manque des cl�s !";                 // Message affich� en cas d'�chec

    private bool joueurAProximite = false; // Vrai si le joueur se trouve dans la zone de trigger
    private bool active = false;           // Vrai si l'appareil a d�j� �t� activ� avec succ�s
    private AudioSource _audio;            // Composant audio utilis� pour jouer les sons

    // Initialise le composant AudioSource au d�marrage
    void Start()
    {
        _audio = gameObject.AddComponent<AudioSource>();
    }

    // V�rifie chaque frame si le joueur peut interagir avec l'appareil final
    void Update()
    {
        if (joueurAProximite && !active && Input.GetKeyDown(KeyCode.F))
            TenterActivation();
    }

    // V�rifie si le joueur poss�de toutes les cl�s requises et active l'appareil si c'est le cas
    void TenterActivation()
    {
        InventaireJoueur inventaire = FindObjectOfType<InventaireJoueur>();
        if (inventaire == null) return;

        foreach (string cle in clesRequises)
        {
            if (!inventaire.PossedeCle(cle))
            {
                JouerSon(sonEchec);
                UIManager.instance.ShowInteract(messageErreur);
                StartCoroutine(RafficherMessage());
                return;
            }
        }

        active = true;
        JouerSon(sonSucces);
        UIManager.instance.HideInteract();
        GameManager.instance.JeuComplete();
    }

    // Attend 2 secondes apr�s un �chec puis r�affiche le message d'interaction normal
    private System.Collections.IEnumerator RafficherMessage()
    {
        yield return new WaitForSeconds(2f);
        if (joueurAProximite && !active)
            UIManager.instance.ShowInteract(messageInteraction);
    }

    // Joue un clip audio en one-shot si le clip et la source audio sont valides
    void JouerSon(AudioClip clip)
    {
        if (clip != null && _audio != null)
            _audio.PlayOneShot(clip);
    }

    // D�tecte quand le joueur entre dans la zone et affiche le message d'interaction
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            joueurAProximite = true;
            UIManager.instance.ShowInteract(messageInteraction);
        }
    }

    // D�tecte quand le joueur quitte la zone et cache le message d'interaction
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            joueurAProximite = false;
            UIManager.instance.HideInteract();
        }
    }
}