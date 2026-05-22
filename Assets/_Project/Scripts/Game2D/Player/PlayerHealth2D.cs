using UnityEngine;
using System.Collections;

/*
 * Auteur : Michael Proulx, David Champagne
 * Date : 07/03/2026 - Dernière Modification : 20/05/2026
 * Projet : Alpha Omega Inversion - Jeu 2D
 *
 * Description globale :
 * Ce script gère la vie du joueur dans la partie 2D.
 * Il permet au joueur de recevoir des dégâts, de récupérer de la vie,
 * de mourir, de réapparaître au dernier checkpoint et d'avoir une courte
 * période d'invincibilité après avoir subi des dégats.
 *
 * Informations pertinentes :
 * - Le script utilise HealthBar pour afficher la vie du joueur.
 * - Le script utilise CheckpointManager pour replacer le joueur au bon endroit.
 * - Le script désactive temporairement le mouvement, le Rigidbody2D et le Collider2D lors de la mort.
 * - Le script utilise GameOverManager2D pour afficher l'écran de fin de partie.
 */

public class PlayerHealth2D : MonoBehaviour
{
    // Vie maximale du joueur.
    public int maxHealth = 100;

    // Vie actuelle du joueur.
    public int currentHealth;

    // Durée pendant laquelle le joueur est invincible après avoir reçu des dégâts.
    public float invicibilityTimeAfterHit = 2.5f;

    // Temps entre chaque clignotement visuel pendant l'invincibilité.
    public float invicibilityFlashDelay = 0.15f;

    // Indique si le joueur est actuellement invincible.
    public bool isInvincible = false;

    // Indique si le joueur est mort.
    public bool isDead = false;

    [Header("Références")]
    // SpriteRenderer utilisé pour faire clignoter le joueur lorsqu'il est invincible.
    public SpriteRenderer graphics;

    // Barre de vie affichée dans l'interface.
    public HealthBar healthBar;

    // Son joué lorsque le joueur reçoit des dégâts.
    public AudioClip hitSound;

    [Header("Composants joueur")]
    // Script de mouvement du joueur, désactivé lors de la mort.
    public MonoBehaviour playerMovementScript;

    // Animator du joueur, utilisé pour déclencher l'animation de mort.
    public Animator animator;

    // Rigidbody2D du joueur, utilisé pour contrôler sa physique.
    public Rigidbody2D rb;

    // Collider2D du joueur, désactivé lors de la mort et du respawn.
    public Collider2D playerCollider;

    // Instance statique permettant d'accéder facilement à PlayerHealth2D.
    public static PlayerHealth2D instance;

    // Position de départ du joueur dans la scène.
    private Vector3 startingPosition;

    // Initialise l'instance statique du script.
    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Il y a plus d'une instance de PlayerHealth2D dans la scène.");
        }

        instance = this;
    }

    // Initialise la vie du joueur, récupère les composants manquants et configure la barre de vie.
    void Start()
    {
        startingPosition = transform.position;

        currentHealth = maxHealth;

        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }

        if (playerCollider == null)
        {
            playerCollider = GetComponent<Collider2D>();
        }

        if (graphics == null)
        {
            graphics = GetComponent<SpriteRenderer>();
        }

        if (playerMovementScript == null)
        {
            playerMovementScript = GetComponent<PlayerMovement>();

            if (playerMovementScript == null)
            {
                playerMovementScript = GetComponent<PlayerMovement_NoGravitySwitch>();
            }
        }

        if (healthBar != null)
        {
            healthBar.SetMaxHealth(maxHealth);
        }
        else
        {
            Debug.LogWarning("HealthBar non assignée dans PlayerHealth2D.");
        }
    }

    // Permet de tester les dégâts avec la touche H.
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            TakeDamage(50);
        }
    }

    // Redonne de la vie au joueur sans dépasser la vie maximale.
    public void HealPlayer(int amount)
    {
        if (isDead)
        {
            return;
        }

        currentHealth += amount;

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        if (healthBar != null)
        {
            healthBar.SetHealth(currentHealth);
        }
    }

    // Applique des dégâts au joueur, met à jour l'interface et déclenche l'invincibilité temporaire.
    public void TakeDamage(int damage)
    {
        if (isInvincible || isDead)
        {
            return;
        }

        if (AudioManager.instance != null && hitSound != null)
        {
            AudioManager.instance.PlayClipAt(hitSound, transform.position);
        }

        currentHealth -= damage;

        if (healthBar != null)
        {
            healthBar.SetHealth(currentHealth);
        }

        if (currentHealth <= 0)
        {
            Die();
            return;
        }

        isInvincible = true;
        StartCoroutine(InvicibilityFlash());
        StartCoroutine(HandleInvincibilityDelay());
    }

    // Gère la mort du joueur, sauvegarde le score, désactive ses contrôles et affiche l'écran de fin.
    public void Die()
    {
        if (isDead)
        {
            return;
        }

        isDead = true;

        ScoreSaver.AddScore2D(Inventory.eggCount);

        if (playerMovementScript != null)
        {
            playerMovementScript.enabled = false;
        }

        if (animator != null)
        {
            animator.SetTrigger("Death");
        }

        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        if (playerCollider != null)
        {
            playerCollider.enabled = false;
        }

        if (GameOverManager2D.instance != null)
        {
            GameOverManager2D.instance.OnPlayerDeath();
        }
        else
        {
            Debug.LogWarning("GameOverManager2D introuvable.");
        }

        Inventory.ResetEnergys();
    }

    // Replace le joueur au dernier checkpoint ou à sa position de départ, puis réactive ses composants.
    public void Respawn()
    {
        isDead = false;

        Vector3 spawnPosition = CheckpointManager.GetSpawnPosition(startingPosition);

        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        if (playerCollider != null)
        {
            playerCollider.enabled = false;
        }

        transform.position = spawnPosition;
        Physics2D.SyncTransforms();

        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.simulated = true;
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        if (playerCollider != null)
        {
            playerCollider.enabled = true;
        }

        if (playerMovementScript != null)
        {
            playerMovementScript.enabled = true;
        }

        currentHealth = maxHealth;
        isInvincible = false;

        if (graphics != null)
        {
            graphics.color = Color.white;
        }

        if (healthBar != null)
        {
            healthBar.SetHealth(currentHealth);
        }

        if (animator != null)
        {
            animator.Rebind();
            animator.Update(0f);
        }

        Debug.Log("Joueur respawn à : " + spawnPosition);
    }

    // Fait clignoter le joueur pendant la période d'invincibilité.
    public IEnumerator InvicibilityFlash()
    {
        while (isInvincible)
        {
            if (graphics != null)
            {
                graphics.color = new Color(1f, 1f, 1f, 0f);
            }

            yield return new WaitForSeconds(invicibilityFlashDelay);

            if (graphics != null)
            {
                graphics.color = new Color(1f, 1f, 1f, 1f);
            }

            yield return new WaitForSeconds(invicibilityFlashDelay);
        }
    }

    // Attend la fin du délai d'invincibilité avant de rendre le joueur vulnérable de nouveau.
    public IEnumerator HandleInvincibilityDelay()
    {
        yield return new WaitForSeconds(invicibilityTimeAfterHit);
        isInvincible = false;
    }
}