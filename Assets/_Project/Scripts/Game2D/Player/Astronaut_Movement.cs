using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

/*
 * Auteur : Michael Proulx
 * Date : -/03/2026
 * Projet : Alpha Omega Inversion - Jeu 2D
 *
 * Description globale :
 * Ce script gère le déplacement horizontal, le saut, l'inversion de gravité
 * et les animations du joueur 2D.
 *
 * Informations pertinentes :
 * - Le joueur doit avoir un Rigidbody2D, un Animator et un SpriteRenderer.
 * - Le script utilise PlayerControls avec le nouveau Input System de Unity.
 * - Le point groundCheck sert à détecter si le joueur touche le sol.
 */

public class PlayerMovement : MonoBehaviour
{
    // ─────────────────────────────────────────
    //  PARAMÈTRES PUBLICS (modifiables dans l'Inspector)
    // ─────────────────────────────────────────

    [Header("Déplacement")]
    public float speed = 5f;   // Vitesse de déplacement horizontal
    public float jumpForce = 50f;  // Force appliquée lors du saut

    [Header("Détection du sol")]
    public Transform groundCheck; // Point vide placé sous les pieds du personnage
    public LayerMask groundLayer; // Layer assigné au sol

    // Instance statique permettant d'accéder au mouvement du joueur depuis d'autres scripts.
    public static PlayerMovement instance;
    
    public Animator animator; // Contrôleur d'animations

    // Collider du joueur utilisé pour calculer la position du groundCheck lors de l'inversion de gravité.
    public BoxCollider2D playerCollider;

    public Rigidbody2D rb;             // Physique du personnage
    public AudioClip jumpSound;        // Son joué lors du saut

    // ─────────────────────────────────────────
    //  COMPOSANTS PRIVÉS
    // ─────────────────────────────────────────

    public SpriteRenderer spriteRenderer; // Rendu du sprite (pour le flip)
    private PlayerControls controls;       // Input Actions (nouveau Input System)

    // ─────────────────────────────────────────
    //  VARIABLES PRIVÉES
    // ─────────────────────────────────────────

    private Vector2 moveInput;    // Direction de déplacement lue depuis les inputs
    private Vector3 originalScale; // Scale original du personnage (sauvegardé au Start)

    private bool isGrounded;      // Vrai si le personnage touche le sol
    private bool jumpRequested;   // Vrai si le joueur a appuyé sur la touche de saut

    private float jumpCooldown = 0.3f; // Délai minimum entre deux sauts
    private float lastJumpTime = -1f;  // Timestamp du dernier saut
    private float groundedCooldown = 0f;  // Délai avant que isGrounded passe à false
                                          // Évite les oscillations au moment du décollage

    // Indique si la gravité du joueur est inversée.
    public bool gravityInverted = false;

    // Position normale du groundCheck.
    private Vector3 groundCheckNormal;

    // Position inversée du groundCheck lorsque la gravité est inversée.
    private Vector3 groundCheckInverted;

    // ─────────────────────────────────────────
    //  INITIALISATION DES INPUTS
    // ─────────────────────────────────────────

    // Initialise l'instance du joueur et configure les actions du nouveau Input System.
    void Awake()
    {
        if(instance != null)
        {
            Debug.LogWarning("Il y a plus d'une instance de PlayerMovement dans la scène");
        }
            instance = this;


        controls = new PlayerControls();

        // Lecture du déplacement horizontal/vertical
        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => moveInput = Vector2.zero;

        // Enregistre la demande de saut (traitée dans Update)
        controls.Player.Jump.performed += ctx => jumpRequested = true;
    }

    // Active les inputs quand l'objet est actif
    void OnEnable() { controls.Enable(); }

    // Désactive les inputs quand l'objet est inactif (évite les fuites mémoire)
    void OnDisable() { controls.Disable(); }

    // ─────────────────────────────────────────
    //  DÉMARRAGE
    // ─────────────────────────────────────────

    // Récupère les composants du joueur et prépare les positions du groundCheck.
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalScale = transform.localScale; // Sauvegarde la taille originale

        groundCheckNormal = groundCheck.localPosition;

        groundCheckInverted = new Vector3(
            groundCheckNormal.x,
            -groundCheckNormal.y,
            groundCheckNormal.z
        );
    }

    // ─────────────────────────────────────────
    //  MISE À JOUR (chaque frame)
    // ─────────────────────────────────────────

    // Met à jour la détection du sol, le saut, le sens du sprite, les animations et l'inversion de gravité.
    void Update()
    {
        DetectGround();
        HandleJump();
        FlipSprite();
        UpdateAnimations();

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (SceneManager.GetActiveScene().name != "Niveau_3")
            {
                InvertGravity();
            }
        }
    }

    // ─────────────────────────────────────────
    //  PHYSIQUE (intervalle fixe)
    // ─────────────────────────────────────────

    void FixedUpdate()
    {
        // Applique le déplacement horizontal sans affecter la vélocité verticale (gravité)
        rb.linearVelocity = new Vector2(moveInput.x * speed, rb.linearVelocity.y);
    }

    // ─────────────────────────────────────────
    //  MÉTHODES PRIVÉES
    // ─────────────────────────────────────────

    /// Détecte si le personnage touche le sol via un cercle de collision.
    /// Un cooldown évite que isGrounded oscille rapidement au moment du décollage.
    void DetectGround()
    {
        bool groundedCheck = Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer);

        if (groundedCheck)
        {
            isGrounded = true;
            groundedCooldown = 0.2f; // Maintient isGrounded à true pendant 0.2s après avoir quitté le sol
        }
        else
        {
            groundedCooldown -= Time.deltaTime;
            if (groundedCooldown <= 0f)
            {
                isGrounded = false;
            }
        }
    }

    /// Applique la force de saut si le personnage est au sol
    /// et que le cooldown entre deux sauts est écoulé.
    void HandleJump()
    {
        bool cooldownPassed = Time.time > lastJumpTime + jumpCooldown;

        if (jumpRequested && isGrounded && cooldownPassed)
        {
            AudioManager.instance.PlayClipAt(jumpSound, transform.position);
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, gravityInverted ? -jumpForce : jumpForce);
            lastJumpTime = Time.time;
            isGrounded = false; // Force isGrounded à false immédiatement pour éviter un double saut
            groundedCooldown = 0f; // Reset le cooldown de détection du sol
        }

        jumpRequested = false; // Réinitialise la demande de saut dans tous les cas
    }

    /// Retourne le sprite horizontalement selon la direction du déplacement.
    /// Utilise flipX pour ne pas modifier le localScale (ce qui déplacerait le GroundCheck).
    void FlipSprite()
    {
        if (moveInput.x > 0)
            spriteRenderer.flipX = false; // Regarde à droite
        else if (moveInput.x < 0)
            spriteRenderer.flipX = true;  // Regarde à gauche
    }

    /// Met à jour les paramètres de l'Animator selon l'état du personnage.
    /// - Speed        : vitesse horizontale (déclenche Run)
    /// - VerticalSpeed : vélocité verticale (montée/descente)
    /// - IsJumping    : vrai si le personnage est en l'air (déclenche Jump)
    void UpdateAnimations()
    {
        animator.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));
        animator.SetBool("IsJumping", !isGrounded);
    }

    /// Affiche le cercle de détection du sol dans l'éditeur Unity (vue Scene).
    /// Visible uniquement quand l'objet est sélectionné.
    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, 0.1f);
        }
    }

    // Inverse la gravité du joueur, retourne le sprite verticalement et déplace le groundCheck du bon côté.
    void InvertGravity()
{
    if (isGrounded)
    {
        gravityInverted = !gravityInverted;

        rb.gravityScale *= -1;

        spriteRenderer.flipY = !spriteRenderer.flipY;

        float playerHeight = playerCollider.bounds.size.y;

        Vector3 pos = groundCheck.localPosition;

        if (gravityInverted)
            pos.y = playerHeight / 2;
        else
            pos.y = -playerHeight / 2;

        groundCheck.localPosition = gravityInverted ? groundCheckInverted : groundCheckNormal;
    }
}
}