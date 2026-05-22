using UnityEngine;
using UnityEngine.InputSystem;

/*
 * Auteur : David Champagne
 * Date : 21/05/2026
 * Projet : Alpha Omega Inversion - Jeu 2D
 *
 * Description globale :
 * Ce script gère le déplacement, le saut et les animations du joueur dans un niveau
 * où l'inversion de gravité n'est pas utilisée.
 *
 * Informations pertinentes :
 * - Le script utilise le nouveau Input System de Unity avec PlayerControls.
 * - Le joueur doit avoir un Rigidbody2D, un Animator et un SpriteRenderer.
 * - Le point groundCheck sert à vérifier si le joueur touche le sol pour sauter.
 */

public class PlayerMovement_NoGravitySwitch : MonoBehaviour
{
    [Header("Déplacement")]
    // Vitesse de déplacement horizontal du joueur.
    public float speed = 5f;

    // Force appliquée lorsque le joueur saute.
    public float jumpForce = 50f;

    [Header("Détection du sol")]
    // Point placé sous le joueur pour détecter le sol.
    public Transform groundCheck;

    // Layer utilisé pour identifier les objets considérés comme le sol.
    public LayerMask groundLayer;

    // Instance statique permettant d'accéder facilement à ce script.
    public static PlayerMovement_NoGravitySwitch instance;

    // Animator utilisé pour contrôler les animations du joueur.
    public Animator animator;

    // Collider principal du joueur.
    public BoxCollider2D playerCollider;

    // Rigidbody2D utilisé pour gérer la physique du joueur.
    public Rigidbody2D rb;

    // Son joué lorsque le joueur saute.
    public AudioClip jumpSound;

    // SpriteRenderer utilisé pour retourner le personnage selon la direction.
    public SpriteRenderer spriteRenderer;

    // Référence aux contrôles du nouveau Input System.
    private PlayerControls controls;

    // Direction de déplacement reçue depuis les inputs.
    private Vector2 moveInput;

    // Indique si le joueur touche le sol.
    private bool isGrounded;

    // Indique si un saut a été demandé par le joueur.
    private bool jumpRequested;

    // Délai minimum entre deux sauts.
    private float jumpCooldown = 0.3f;

    // Temps où le dernier saut a été effectué.
    private float lastJumpTime = -1f;

    // Petit délai utilisé pour stabiliser la détection du sol.
    private float groundedCooldown = 0f;

    // Initialise l'instance et configure les contrôles du joueur.
    void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Il y a plus d'une instance de PlayerMovement_NoGravitySwitch dans la scène");
        }

        instance = this;

        controls = new PlayerControls();

        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => moveInput = Vector2.zero;

        controls.Player.Jump.performed += ctx => jumpRequested = true;
    }

    // Active les contrôles lorsque l'objet devient actif.
    void OnEnable()
    {
        controls.Enable();
    }

    // Désactive les contrôles lorsque l'objet devient inactif.
    void OnDisable()
    {
        controls.Disable();
    }

    // Récupère les composants nécessaires au déplacement et aux animations.
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Met à jour la détection du sol, le saut, le sens du sprite et les animations.
    void Update()
    {
        DetectGround();
        HandleJump();
        FlipSprite();
        UpdateAnimations();
    }

    // Applique le déplacement horizontal du joueur dans la boucle physique.
    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(moveInput.x * speed, rb.linearVelocity.y);
    }

    // Vérifie si le joueur touche le sol avec un cercle de détection.
    void DetectGround()
    {
        bool groundedCheck = Physics2D.OverlapCircle(
            groundCheck.position,
            0.1f,
            groundLayer
        );

        if (groundedCheck)
        {
            isGrounded = true;
            groundedCooldown = 0.2f;
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

    // Gère le saut du joueur si celui-ci est au sol et si le délai entre les sauts est respecté.
    void HandleJump()
    {
        bool cooldownPassed = Time.time > lastJumpTime + jumpCooldown;

        if (jumpRequested && isGrounded && cooldownPassed)
        {
            if (AudioManager.instance != null)
            {
                AudioManager.instance.PlayClipAt(jumpSound, transform.position);
            }

            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x,
                jumpForce
            );

            lastJumpTime = Time.time;

            isGrounded = false;
            groundedCooldown = 0f;
        }

        jumpRequested = false;
    }

    // Retourne le sprite du joueur selon la direction du déplacement.
    void FlipSprite()
    {
        if (moveInput.x > 0)
        {
            spriteRenderer.flipX = false;
        }
        else if (moveInput.x < 0)
        {
            spriteRenderer.flipX = true;
        }
    }

    // Met à jour les paramètres de l'Animator selon la vitesse et l'état de saut du joueur.
    void UpdateAnimations()
    {
        animator.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));
        animator.SetBool("IsJumping", !isGrounded);
    }

    // Affiche le cercle de détection du sol dans la vue Scene de Unity lorsque l'objet est sélectionné.
    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, 0.1f);
        }
    }
}