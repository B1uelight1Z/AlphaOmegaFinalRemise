using UnityEngine;
using System.Collections;

/*
 * Auteur : David Champagne
 * Date : 21/05/2026
 * Projet : Alpha Omega Inversion - Jeu 2D
 *
 * Description globale :
 * Ce script gère la vie du joueur dans un niveau 2D sans inversion de gravité.
 * Il permet au joueur de recevoir des dégâts, de mourir, de réapparaître au dernier
 * checkpoint et d'avoir une période d'invincibilité temporaire après avoir été touché.
 *
 * Informations pertinentes :
 * - Le script utilise HealthBar pour afficher la vie du joueur.
 * - Le script utilise CheckpointManager pour replacer le joueur au bon endroit.
 * - Le script utilise GameOverManager2D lorsque le joueur meurt.
 */

public class PlayerHealth2D_NoGravitySwitch : MonoBehaviour
{
    // Vie maximale du joueur.
    public int maxHealth = 100;

    // Vie actuelle du joueur.
    public int currentHealth;

    // Durée de l'invincibilité après avoir reçu des dégâts.
    public float invicibilityTimeAfterHit = 2.5f;

    // Délai entre chaque clignotement pendant l'invincibilité.
    public float invicibilityFlashDelay = 0.15f;

    // Indique si le joueur est actuellement invincible.
    public bool isInvincible = false;

    [Header("Références")]
    // SpriteRenderer utilisé pour faire clignoter le joueur.
    public SpriteRenderer graphics;

    // Barre de vie affichée dans l'interface.
    public HealthBar healthBar;

    // Son joué lorsque le joueur reçoit des dégâts.
    public AudioClip hitSound;

    [Header("Composants joueur")]
    // Script de mouvement du joueur sans inversion de gravité.
    public PlayerMovement_NoGravitySwitch playerMovementScript;

    // Animator du joueur utilisé pour déclencher l'animation de mort.
    public Animator animator;

    // Instance statique permettant d'accéder facilement à ce script.
    public static PlayerHealth2D_NoGravitySwitch instance;

    // Rigidbody2D utilisé pour gérer la physique du joueur.
    public Rigidbody2D rb;

    // Collider2D du joueur.
    public Collider2D playerCollider;

    // Indique si le joueur est mort.
    public bool isDead = false;

    // Position de départ du joueur dans la scène.
    private Vector3 startingPosition;

    // Initialise l'instance statique du script.
    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Il y a plus d'une instance de PlayerHealth2D_NoGravitySwitch dans la scène");
        }

        instance = this;
    }

    // Initialise la vie, sauvegarde la position de départ et récupère les composants manquants.
    void Start()
    {
        currentHealth = maxHealth;

        startingPosition = transform.position;

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
            playerMovementScript = GetComponent<PlayerMovement_NoGravitySwitch>();
        }

        if (healthBar != null)
        {
            healthBar.SetMaxHealth(maxHealth);
        }
    }

    // Applique des dégâts au joueur, met à jour la barre de vie et déclenche l'invincibilité.
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

    // Gère la mort du joueur, arrête son mouvement et affiche l'écran de fin.
    public void Die()
    {
        if (isDead)
            return;

        isDead = true;

        rb.linearVelocity = Vector2.zero;

        if (animator != null)
        {
            animator.SetTrigger("Death");
        }

        if (GameOverManager2D.instance != null)
        {
            GameOverManager2D.instance.OnPlayerDeath();
        }

        Inventory.ResetEggs();
        Inventory.ResetEnergys();
    }

    // Replace le joueur au dernier checkpoint ou à sa position de départ, puis réinitialise son état.
    public void Respawn()
    {
        isDead = false;

        Vector3 spawnPosition =
            CheckpointManager.GetSpawnPosition(startingPosition);

        transform.position = spawnPosition;

        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.simulated = true;

        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;

        playerCollider.enabled = true;

        if (playerMovementScript != null)
        {
            playerMovementScript.enabled = true;
        }

        currentHealth = maxHealth;

        isInvincible = false;

        graphics.color = Color.white;

        if (healthBar != null)
        {
            healthBar.SetHealth(currentHealth);
        }

        if (animator != null)
        {
            animator.Rebind();
            animator.Update(0f);
        }
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

    // Attend la fin du délai d'invincibilité avant de rendre le joueur vulnérable.
    public IEnumerator HandleInvincibilityDelay()
    {
        yield return new WaitForSeconds(invicibilityTimeAfterHit);

        isInvincible = false;
    }
}