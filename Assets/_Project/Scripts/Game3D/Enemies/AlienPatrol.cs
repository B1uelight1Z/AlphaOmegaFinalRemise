using System.Collections;
using UnityEngine;
using UnityEngine.AI;

// Auteur: David Champagne, Michael Proulx
// Dernière date de modification: 22/05/2026
// Gère le comportement d'un alien combinant une patrouille par points et une chasse du joueur.
// Intègre des mécaniques de détection visuelle, d'alerte par icône, d'agro temporaire suite à un tir et d'étourdissement court.
public class AlienPatrol : MonoBehaviour
{
    public Transform[] points; // Tableau des points de passage pour la patrouille
    public Transform Player; // Référence vers le transform du joueur

    [Header("Distance joueur")]
    public float distanceArretJoueur = 1.4f; // Distance minimale pour s'arrêter devant le joueur et attaquer

    [Header("Réaction quand touché")]
    public float dureeArretQuandTouche = 0.15f; // Durée de l'immobilisation lorsque l'alien subit un dégât

    private Coroutine coroutineArret; // Stocke la coroutine de l'arrêt court pour pouvoir l'interrompre si nécessaire
    private bool estArreteCourtement = false; // Vrai si l'alien est actuellement immobilisé par un tir

    [Header("Détection")]
    public float detectDistance = 5f; // Rayon de détection à partir duquel l'alien repère le joueur

    [Header("Vitesses")]
    public float vitessePatrouille = 2f; // Vitesse de déplacement en mode patrouille
    public float vitesseChasse = 3.5f; // Vitesse de déplacement en mode poursuite standard
    public float vitesseAgro = 6f; // Vitesse de déplacement accrue suite à une alerte par tir

    [Header("Agro par tir")]
    public float dureeAgro = 6f; // Temps pendant lequel l'alien poursuit agressivement le joueur après avoir été touché
    private NavMeshAgent agent; // Référence vers le composant de navigation
    private int destinationIndex = 0; // Index du point de patrouille actuel vers lequel l'alien se dirige

    private bool chasingPlayer = false; // Vrai si le joueur est repéré par la détection de distance
    private bool agroParTir = false; // Vrai si l'alien est en état d'agro forcé suite à un tir reçu
    private Coroutine coroutineAgro; // Stocke la coroutine d'agro pour la réinitialiser si l'alien est touché à nouveau

    [Header("Feedback alerte")]
    public GameObject alertIcon; // Visuel de l'icône d'alerte à afficher au-dessus de l'alien
    public float dureeIconeAlerte = 1f; // Temps d'affichage de l'icône d'alerte à l'écran

    private Coroutine coroutineIconeAlerte; // Stocke la coroutine de l'icône pour gérer son affichage proprement

    // Initialise les composants, cherche le joueur par tag s'il est manquant et configure l'icône
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if (Player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                Player = playerObject.transform;
            }
        }

        if (alertIcon != null)
        {
            alertIcon.SetActive(false);
        }
    }

    // Boucle principale qui orchestre les états de l'alien (étourdissement, détection, chasse ou patrouille)
    void Update()
    {
        if (estArreteCourtement) return; // Bloque toute action si l'alien est actuellement immobilisé par un coup

        SearchPlayer();

        if (chasingPlayer)
        {
            ChasserJoueur();
        }
        else
        {
            Patrol();
        }
    }

    // Déplace l'alien cycliquement d'un point de patrouille à un autre à vitesse normale
    void Patrol()
    {
        if (points.Length == 0) return;

        agent.isStopped = false;
        agent.speed = vitessePatrouille;
        agent.destination = points[destinationIndex].position;

        if (Vector3.Distance(transform.position, points[destinationIndex].position) < 1f)
        {
            destinationIndex = (destinationIndex + 1) % points.Length;
        }
    }

    // Dirige l'alien vers la position du joueur et gère la distance de sécurité pour l'attaque
    void ChasserJoueur()
    {
        float distance = Vector3.Distance(transform.position, Player.position);

        if (distance <= distanceArretJoueur)
        {
            agent.isStopped = true; // Arrête le déplacement si l'alien est au corps à corps
        }
        else
        {
            agent.isStopped = false;
            agent.speed = agroParTir ? vitesseAgro : vitesseChasse; // Adapte la vitesse selon la source de la détection
            agent.destination = Player.position;
        }
    }

    // Calcule la distance avec le joueur pour mettre à jour l'état de poursuite visuelle ou par tir
    void SearchPlayer()
    {
        if (agroParTir)
        {
            chasingPlayer = true; // Force l'état de chasse si l'agro par tir est actif
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, Player.position);

        if (distanceToPlayer <= detectDistance)
        {
            chasingPlayer = true;
        }
        else
        {
            chasingPlayer = false;
        }
    }

    // Point d'entrée public pour déclencher la réaction d'agro suite à un tir reçu avec un délai d'action
    public void DeclencherAgroAvecDelai(Transform joueur, float delai)
    {
        if (coroutineAgro != null)
        {
            StopCoroutine(coroutineAgro);
        }

        coroutineAgro = StartCoroutine(AgroApresDelai(joueur, delai));
    }

    // Applique l'état d'alerte et de traque agressive après l'écoulement du délai spécifié
    private IEnumerator AgroApresDelai(Transform joueur, float delai)
    {
        yield return new WaitForSeconds(delai);

        Player = joueur;
        agroParTir = true;
        AfficherIconeAlerte();

        yield return new WaitForSeconds(dureeAgro);

        agroParTir = false;
    }

    // Point d'entrée public pour infliger un court étourdissement physique à l'alien (réaction à l'impact)
    public void ArreterCourtement()
    {
        if (coroutineArret != null)
        {
            StopCoroutine(coroutineArret);
        }

        coroutineArret = StartCoroutine(ArretCourt());
    }

    // Immobilise temporairement le NavMeshAgent et réactive ses mouvements après le délai d'impact
    private IEnumerator ArretCourt()
    {
        estArreteCourtement = true;
        agent.isStopped = true;

        yield return new WaitForSeconds(dureeArretQuandTouche);

        agent.isStopped = false;
        estArreteCourtement = false;
    }

    // Déclenche l'apparition temporaire du retour visuel d'alerte au-dessus de l'alien
    void AfficherIconeAlerte()
    {
        if (alertIcon == null) return;

        if (coroutineIconeAlerte != null)
        {
            StopCoroutine(coroutineIconeAlerte);
        }

        coroutineIconeAlerte = StartCoroutine(IconeAlerteCoroutine());
    }

    // Active l'objet de l'icône d'alerte et le désactive une fois la durée spécifiée écoulée
    private IEnumerator IconeAlerteCoroutine()
    {
        alertIcon.SetActive(true);
        yield return new WaitForSeconds(dureeIconeAlerte);
        alertIcon.SetActive(false);
    }
}