using UnityEngine;

/*
 * Nom du script : TiltPlatform
 * Auteur : Timothy Chatelier
 * Date : 03/03/2026
 * Projet : AlphaOmega Inversion - Jeu 2D
 * 
 * Description globale :
 * Ce script permet de créer une plateforme basculante.
 * 
 * Lorsque le joueur monte sur la plateforme,
 * celle-ci s'incline selon la position du joueur.
 * Plus le joueur se déplace vers un côté,
 * plus la plateforme penche dans cette direction.
 * 
 * Lorsque le joueur quitte la plateforme,
 * celle-ci retourne progressivement à sa position initiale.
 * 
 * Informations pertinentes :
 * - Le joueur doit posséder le tag "Player".
 * - Le système utilise Rigidbody2D pour effectuer la rotation.
 * - La rotation est limitée par un angle maximal.
 * - La plateforme utilise FixedUpdate afin de garder
 *   un comportement physique stable.
 */

public class TiltPlatform : MonoBehaviour
{
    // Angle maximal que la plateforme peut atteindre.
    public float maxTiltAngle = 20f;

    // Vitesse à laquelle la plateforme tourne.
    public float tiltSpeed = 50f;

    // Largeur utilisée pour calculer l'inclinaison.
    public float platformHalfWidth = 1f;

    // Référence vers le Rigidbody2D de la plateforme.
    private Rigidbody2D rb;

    // Vérifie si le joueur est actuellement sur la plateforme.
    private bool playerIsOnPlatform = false;

    // Angle actuel de rotation de la plateforme.
    private float currentAngle = 0f;

    // Référence vers le joueur présent sur la plateforme.
    private Transform player;

    /*
     * Fonction : Start
     * Description :
     * Initialise les composantes nécessaires
     * et force la plateforme à démarrer
     * avec une rotation de 0 degré.
     * 
     * Cette fonction est appelée automatiquement
     * au début de la scène.
     */
    void Start()
    {
        // Récupère le Rigidbody2D attaché à la plateforme.
        rb = GetComponent<Rigidbody2D>();

        // Initialise l'angle actuel.
        currentAngle = 0f;

        // Force la rotation initiale à 0 degré.
        rb.MoveRotation(0f);
    }

    /*
     * Fonction : FixedUpdate
     * Description :
     * Gère la rotation physique de la plateforme.
     * 
     * Si le joueur est présent sur la plateforme,
     * l'angle cible est calculé selon la position
     * horizontale du joueur.
     * 
     * La plateforme tourne progressivement vers
     * cet angle afin d'obtenir un mouvement fluide.
     * 
     * Lorsque le joueur quitte la plateforme,
     * la plateforme retourne doucement à sa position normale.
     */
    void FixedUpdate()
    {
        // Angle cible de la plateforme.
        float targetAngle = 0f;

        // Vérifie si le joueur est présent sur la plateforme.
        if (playerIsOnPlatform && player != null)
        {
            /*
             * Calcule la position relative du joueur
             * par rapport au centre de la plateforme.
             */
            float relativeX = (player.position.x - transform.position.x) / platformHalfWidth;

            // Limite la valeur entre -1 et 1.
            relativeX = Mathf.Clamp(relativeX, -1f, 1f);

            /*
             * Calcule l'angle cible selon la position du joueur.
             * 
             * Plus le joueur est éloigné du centre,
             * plus la plateforme penche.
             */
            targetAngle = -relativeX * maxTiltAngle;
        }

        /*
         * Fait tourner progressivement la plateforme
         * vers l'angle cible.
         * 
         * MoveTowards permet d'obtenir une rotation fluide.
         */
        currentAngle = Mathf.MoveTowards(
            currentAngle,
            targetAngle,
            tiltSpeed * Time.fixedDeltaTime
        );

        // Applique la rotation au Rigidbody2D.
        rb.MoveRotation(currentAngle);
    }

    /*
     * Fonction : OnCollisionEnter2D
     * Description :
     * Détecte lorsque le joueur entre en collision
     * avec la plateforme.
     * 
     * Active le système d'inclinaison.
     */
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Affiche des informations dans la console Unity.
        Debug.Log("CollisionEnter : " + collision.transform.tag + " | " + collision.transform.name);

        // Vérifie si l'objet touché est le joueur.
        if (collision.transform.CompareTag("Player"))
        {
            // Sauvegarde la référence du joueur.
            player = collision.transform;

            // Active l'inclinaison de la plateforme.
            playerIsOnPlatform = true;
        }
    }

    /*
     * Fonction : OnCollisionExit2D
     * Description :
     * Détecte lorsque le joueur quitte la plateforme.
     * 
     * Désactive le système d'inclinaison et
     * permet à la plateforme de revenir à sa position normale.
     */
    private void OnCollisionExit2D(Collision2D collision)
    {
        // Affiche des informations dans la console Unity.
        Debug.Log("CollisionExit : " + collision.transform.tag + " | " + collision.transform.name);

        // Vérifie si l'objet quittant la plateforme est le joueur.
        if (collision.transform.CompareTag("Player"))
        {
            // Désactive l'inclinaison.
            playerIsOnPlatform = false;

            // Retire la référence du joueur.
            player = null;
        }
    }
}