using UnityEngine;

/*
 * Auteur : Michael Proulx, David Champagne
 * Date : 06/03/2026 - Modification: 20/05/2026
 * Projet : Alpha Omega Inversion - Jeu 2D
 *
 * Description globale :
 * Ce script gère le déplacement automatique d'un alien entre plusieurs waypoints.
 * Il permet aussi à l'alien d'infliger des dégâts au joueur lorsqu'il entre en collision avec lui.
 * Le script contient également la logique de mort de l'alien.
 *
 * Informations pertinentes :
 * - Les waypoints doivent être assignés dans l'inspecteur Unity.
 * - Le joueur doit avoir le tag "Player".
 */

public class Alien_Movement : MonoBehaviour
{
    // Vitesse de déplacement de l'alien.
    public float speed;

    // Tableau contenant les points que l'alien doit suivre.
    public Transform[] waypoints;

    // Dégâts prévus lors d'une collision avec le joueur.
    public int damageOnCollision = 20;

    // Indique si l'alien est mort.
    public bool isDead = false;

    // Collider de l'alien, utilisé pour le désactiver lors de sa mort.
    public Collider2D alienCollider;

    // Animator de l'alien, utilisé pour jouer l'animation de mort.
    public Animator animator;

    // SpriteRenderer de l'alien, utilisé pour retourner son image lorsqu'il change de direction.
    private SpriteRenderer graphics;

    // Waypoint actuellement ciblé par l'alien.
    private Transform target;

    // Position actuelle dans le tableau des waypoints.
    private int destPoint = 0;

    // Instance statique du script Alien_Movement.
    public static Alien_Movement instance;

    // Initialise l'instance du script et avertit s'il y en a plus d'une dans la scène.
    void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Il y a plus d'une instance de Alien_Movement dans la scène");
        }

        instance = this;
    }

    // Récupère le SpriteRenderer et vérifie que les waypoints sont valides avant de commencer le déplacement.
    void Start()
    {
        graphics = GetComponent<SpriteRenderer>();

        if (waypoints == null || waypoints.Length == 0)
        {
            Debug.LogWarning(gameObject.name + " : aucun waypoint assigné.");
            enabled = false;
            return;
        }

        if (waypoints[0] == null)
        {
            Debug.LogWarning(gameObject.name + " : le waypoint 0 est vide.");
            enabled = false;
            return;
        }

        target = waypoints[0];
    }

    // Déplace l'alien vers son waypoint actuel et change de waypoint lorsqu'il atteint sa cible.
    void Update()
    {
        if (isDead)
        {
            return;
        }

        if (target == null)
        {
            Debug.LogWarning(gameObject.name + " : target est null.");
            return;
        }

        Vector3 dir = target.position - transform.position;
        transform.Translate(dir.normalized * speed * Time.deltaTime, Space.World);

        if (Vector3.Distance(transform.position, target.position) < 0.3f)
        {
            destPoint = (destPoint + 1) % waypoints.Length;

            if (waypoints[destPoint] == null)
            {
                Debug.LogWarning(gameObject.name + " : waypoint " + destPoint + " est vide.");
                return;
            }

            target = waypoints[destPoint];

            if (graphics != null)
            {
                graphics.flipX = !graphics.flipX;
            }
        }
    }

    // Vérifie si l'alien touche le joueur et lui applique des dégâts selon le système de vie utilisé.
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (PlayerHealth2D.instance != null)
            {
                PlayerHealth2D.instance.TakeDamage(20);
            }

            if (PlayerHealth2D_NoGravitySwitch.instance != null)
            {
                PlayerHealth2D_NoGravitySwitch.instance.TakeDamage(20);
            }
        }
    }

    // Arrête l'alien, désactive son collider, déclenche son animation de mort et détruit l'objet après un délai.
    public void Die()
    {
        isDead = true;
        speed = 0;

        if (alienCollider != null)
        {
            alienCollider.enabled = false;
        }

        if (animator != null)
        {
            animator.SetTrigger("Death");
        }

        Destroy(gameObject, 1f);
    }
}