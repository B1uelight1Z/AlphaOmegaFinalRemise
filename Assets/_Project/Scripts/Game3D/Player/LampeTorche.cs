using UnityEngine;

// Auteur: David Champagne
// Derniere date de modification: 22/05/2026
// Gere l'activation et la bascule automatique des differentes lampes du joueur.
// Adapte l'eclairage actif selon l'etat du joueur (debout, accroupi, vue FPS ou vue TPS).
public class LampeTorche : MonoBehaviour
{
    [Header("Lampes")]
    public Light lampeFusil; // Lampe utilisee lorsque le joueur est debout
    public Light lampeCrawlFPS; // Lampe utilisee en mode accroupi (crawl) en vue a la premiere personne
    public Light lampeCrawlTPS; // Lampe utilisee en mode accroupi (crawl) en vue a la troisieme personne

    [Header("Reference joueur")]
    public AstronautController astronautController; // Reference vers le script de controle du joueur pour connaitre sa posture

    [Header("Controle")]
    public KeyCode toucheLampe = KeyCode.L; // Touche du clavier utilisee pour allumer ou eteindre la lampe

    private bool lampeAllumee = false; // Vrai si la lampe globale est activee par le joueur

    // Initialise l'etat des lampes au lancement du jeu
    void Start()
    {
        MettreAJourLampes();
    }

    // Verifie chaque frame si le joueur appuie sur la touche pour basculer l'etat de la lampe
    void Update()
    {
        if (Input.GetKeyDown(toucheLampe))
        {
            lampeAllumee = !lampeAllumee;
        }

        MettreAJourLampes();
    }

    // Actualise l'activation de chaque composant Light selon la posture et la vue actuelle du joueur
    void MettreAJourLampes()
    {
        bool estCrawl = false;
        bool estTPS = false;

        // Recupere l'etat actuel des mouvements et de la camera du joueur
        if (astronautController != null)
        {
            estCrawl = astronautController.EstAccroupi();
            estTPS = astronautController.EstModeTPS();
        }

        // Active la lampe du fusil uniquement si la lampe est allumee et que le joueur n'est pas accroupi
        if (lampeFusil != null)
        {
            lampeFusil.enabled = lampeAllumee && !estCrawl;
        }

        // Active la lampe de crawl FPS si le joueur est allume, accroupi et en vue premiere personne
        if (lampeCrawlFPS != null)
        {
            lampeCrawlFPS.enabled = lampeAllumee && estCrawl && !estTPS;
        }

        // Active la lampe de crawl TPS si le joueur est allume, accroupi et en vue troisieme personne
        if (lampeCrawlTPS != null)
        {
            lampeCrawlTPS.enabled = lampeAllumee && estCrawl && estTPS;
        }
    }
}