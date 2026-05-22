using UnityEngine;

/*
 * Nom du script : Blades_Deplacement
 * Auteur : Michael Proulx
 * Date : 03/03/2026
 * Projet : AlphaOmega Inversion - Jeu 2D
 * 
 * Description globale :
 * Ce script contrôle le déplacement d'une scie dangereuse dans le jeu.
 * La scie se déplace automatiquement entre plusieurs waypoints afin
 * de créer un obstacle mobile pour le joueur.
 * 
 * Lorsqu'un joueur entre en collision avec la scie, des dégâts
 * lui sont infligés.
 * 
 * Informations pertinentes :
 * - Les waypoints doivent être assignés dans l'inspecteur Unity.
 * - Le déplacement fonctionne en boucle entre tous les waypoints.
 * - Le joueur doit posséder le tag "Player".
 * - Le script PlayerHealth2D est utilisé pour appliquer les dégâts.
 */

public class Blades_Deplacement : MonoBehaviour
{
    // Vitesse de déplacement de la scie.
    public float speed;

    // Liste des points que la scie doit suivre.
    public Transform[] waypoints;

    // Quantité de dégâts infligés au joueur lors d'une collision.
    public int damageOnCollision = 40;

    // Référence au Rigidbody2D de la scie.
    private Rigidbody2D rb;

    // Waypoint actuellement ciblé par la scie.
    private Transform target;

    // Index du waypoint actuel dans la liste.
    private int destPoint = 0;

    /*
     * Fonction : Start
     * Description :
     * Initialise la destination de départ de la scie.
     * Au lancement de la scène, la scie se dirige vers
     * le premier waypoint de la liste.
     */
    void Start()
    {
        // Par défaut, se dirige vers le premier waypoint.
        target = waypoints[0];
    }

    /*
     * Fonction : Update
     * Description :
     * Déplace la scie en direction du waypoint actuel.
     * Lorsque la scie atteint presque sa destination,
     * elle passe automatiquement au waypoint suivant.
     * 
     * Le déplacement se fait en boucle infinie.
     */
    void Update()
    {
        // Calcul de la direction entre la scie et la cible.
        Vector3 dir = target.position - transform.position;

        // Déplacement de la scie vers le waypoint ciblé.
        transform.Translate(dir.normalized * speed * Time.deltaTime, Space.World);

        // Vérifie si la scie est presque arrivée au waypoint.
        if(Vector3.Distance(transform.position, target.position) < 0.3f)
        {
            /*
             * Passe au waypoint suivant.
             * 
             * Le modulo (%) permet de revenir automatiquement
             * au premier waypoint lorsque le dernier est atteint.
             */
            destPoint = (destPoint + 1) % waypoints.Length;

            // Nouvelle cible à atteindre.
            target = waypoints[destPoint];
        }
    }

    /*
     * Fonction : OnCollisionEnter2D
     * Description :
     * Détecte les collisions entre la scie et d'autres objets.
     * 
     * Si l'objet touché possède le tag "Player",
     * le script applique des dégâts au joueur.
     */
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Vérifie si l'objet touché est le joueur.
        if (collision.transform.CompareTag("Player"))
        {
            // Récupère le script de vie du joueur.
            PlayerHealth2D playerHealth2D = collision.transform.GetComponent<PlayerHealth2D>();

            // Inflige des dégâts au joueur.
            playerHealth2D.TakeDamage(damageOnCollision);
        }
    }
}