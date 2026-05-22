using UnityEngine;

// Auteur: David Champagne, Michael Proulx
// Derniere date de modification: 22/05/2026
// Gere les points de vie du joueur, l'encaissement des degats, les effets sonores associes et la mort.
// Assure la liaison avec l'interface graphique (HealthBar) et declenche la sauvegarde du score lors de la defaite.
public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100; // Sante maximale que le joueur peut posseder
    public int currentHealth; // Points de vie actuels du joueur
    public HealthBar healthBar; // Reference vers la barre de vie UI pour actualiser l'affichage graphique

    public static PlayerHealth instance; // Singleton permettant un acces rapide a ce script depuis d'autres classes

    private Rigidbody rb; // Reference au composant Rigidbody du joueur
    private bool isDead = false; // Devient vrai quand les points de vie tombent a zero pour empecher les actions repetitives

    [Header("Audio - Degats")]
    public AudioSource audioDegat; // Source audio utilisee pour emettre les sons de blessure
    public AudioClip sonDegat; // Clip audio joue lorsque le joueur encaisse des degats
    public float volumeDegat = 0.7f; // Volume de lecture de l'effet sonore de degat
    public float delaiEntreSonsDegat = 0.5f; // Temps d'attente minimum entre deux alertes sonores de blessure

    private float _tempsProchainSonDegat = 0f; // Chronometre memorisant le moment ou le prochain son peut etre joue

    // Initialise le Singleton et recupere les composants requis avant le lancement du jeu
    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Plus d'une instance de PlayerHealth !");
        }

        instance = this;
        rb = GetComponent<Rigidbody>();

        if (audioDegat == null)
        {
            audioDegat = GetComponent<AudioSource>();
        }
    }

    // Configure la sante de depart au maximum et synchronise l'interface graphique
    void Start()
    {
        currentHealth = maxHealth;

        if (healthBar != null)
        {
            healthBar.SetMaxHealth(maxHealth);
        }
    }

    // Retire des points de vie au joueur, declenche le son de blessure et verifie si le seuil critique est atteint
    public void TakeDamage(int damage)
    {
        if (isDead)
        {
            return;
        }

        currentHealth -= damage;
        JouerSonDegat();

        if (healthBar != null)
        {
            healthBar.SetHealth(currentHealth);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // Force la mort immediate du joueur independamment de ses points de vie actuels
    public void KillPlayer()
    {
        if (isDead)
        {
            return;
        }

        currentHealth = 0;

        if (healthBar != null)
        {
            healthBar.SetHealth(currentHealth);
        }

        Die();
    }

    // Joue l'effet sonore de degat si le delai d'attente minimum requis s'est ecoule
    void JouerSonDegat()
    {
        if (Time.time < _tempsProchainSonDegat)
        {
            return;
        }

        _tempsProchainSonDegat = Time.time + delaiEntreSonsDegat;

        if (audioDegat != null && sonDegat != null)
        {
            audioDegat.PlayOneShot(sonDegat, volumeDegat);
        }
    }

    // Execute la sequence de fin de partie, arrete les alarmes, sauvegarde le score global et vide l'inventaire
    void Die()
    {
        if (isDead)
        {
            return;
        }

        isDead = true;
        Debug.Log("Le joueur est mort !");

        if (GestionnaireAlarme.instance != null)
        {
            GestionnaireAlarme.instance.ReinitialiserAlarme();
        }

        // Sauvegarder score 3D dans le systeme de classement de fin de partie
        if (GameManager.instance != null)
        {
            Debug.Log("Score 3D sauvegarde a la mort : " + GameManager.instance.score);
            ScoreSaver.AddScore3D(GameManager.instance.score);
        }
        else
        {
            Debug.LogWarning("GameManager instance est NULL. Score 3D non sauvegarde.");
            ScoreSaver.AddScore3D(0);
        }

        if (GameOverManager.instance != null)
        {
            GameOverManager.instance.OnPlayerDeath();
        }
        else
        {
            Debug.LogWarning("GameOverManager instance est NULL.");
        }

        // Reset inventaire pour vider les elements ramasses pendant la tentative
        Inventory.ResetEggs();
        Inventory.ResetEnergys();
    }

    // Remet a neuf la sante du joueur et reinitialise son etat de vie (utile lors d'un respawn)
    public void ResetHealth()
    {
        isDead = false;
        currentHealth = maxHealth;

        if (healthBar != null)
        {
            healthBar.SetHealth(currentHealth);
        }
    }
}