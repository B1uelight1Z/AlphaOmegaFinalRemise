using UnityEngine;

// Auteur: David Champagne, Michael Proulx
// Dernière date de modification: 22/05/2026
// Gère le comportement d'une caméra à la troisième personne (TPS).
// Ajuste dynamiquement la position, la hauteur, la visée, le décalage de côté selon l'état du joueur et gère les collisions avec le décor.
public class CameraTPS : MonoBehaviour
{
    public Transform joueur; // Référence vers le Transform du joueur que la caméra doit suivre

    [Header("Réglages caméra")]
    public float distanceDebout = 2f; // Distance de recul de la caméra lorsque le joueur est debout
    public float distanceCrawl = 2f; // Distance de recul de la caméra lorsque le joueur rampe ou est accroupi

    public float hauteurDebout = 1.5f; // Hauteur de la caméra par rapport au joueur debout
    public float hauteurCrawl = 0.4f; // Hauteur de la caméra par rapport au joueur qui rampe ou est accroupi

    public float decalageCoteDebout = 0.15f; // Décalage latéral de la caméra (effet "over the shoulder") en position debout
    public float decalageCoteCrawl = 0.1f; // Décalage latéral de la caméra en position rampante

    [Header("Réglages caméra - Visée")]
    public float decalageCoteVise = 0.4f; // Décalage latéral de la caméra pendant la visée pour mieux voir la cible

    [Tooltip("Décalage du point que la caméra regarde pendant la visée. Souvent proche de decalageCoteVise.")]
    public float decalageRegardVise = 0.4f; // Décalage latéral du point d'ancrage du regard pendant la visée

    public float sensibiliteSouris = 200f; // Sensibilité de la rotation verticale de la caméra avec la souris
    public float vitesseTransition = 8f; // Vitesse de transition fluide (Lerp) entre les différents états de la caméra

    [Header("Référence joueur")]
    public AstronautController astronautController; // Référence vers le script de contrôle du joueur pour connaître son état actuel

    [Header("Collision")]
    public LayerMask obstacleMask; // Couches de collision qui bloquent la vue de la caméra (murs, décors)
    public float rayonCamera = 0.3f; // Rayon de la sphère utilisée pour détecter les collisions et éviter de passer à travers les murs
    public float smoothSpeed = 10f; // Vitesse d'amortissement de la position de la caméra

    float rotationX = 10f; // Rotation actuelle de la caméra sur l'axe X (inclinaison verticale)

    float hauteurActuelle; // Valeur lissée de la hauteur actuelle de la caméra
    float distanceActuelleCamera; // Valeur lissée de la distance actuelle de recul de la caméra
    float decalageCoteActuel; // Valeur lissée du décalage latéral actuel de la caméra
    float decalageRegardActuel; // Valeur lissée du décalage latéral actuel du point de visée du regard

    // Initialise l'état du curseur et définit les valeurs de départ des dimensions de la caméra
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        hauteurActuelle = hauteurDebout;
        distanceActuelleCamera = distanceDebout;
        decalageCoteActuel = decalageCoteDebout;
        decalageRegardActuel = decalageCoteDebout;
    }

    // Calcule et applique les mouvements, les transitions d'état et la gestion des collisions de la caméra après le mouvement du joueur
    void LateUpdate()
    {
        if (joueur == null)
        {
            return;
        }

        float mouseY = Input.GetAxis("Mouse Y") * sensibiliteSouris * Time.deltaTime;

        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -10f, 30f);

        bool estCrawl = false;
        bool estEnTrainDeViser = false;

        if (astronautController != null)
        {
            estCrawl = astronautController.EstAccroupi();
            estEnTrainDeViser = astronautController.EstEnTrainDeViser();
        }

        float hauteurCible = hauteurDebout;
        float distanceCible = distanceDebout;
        float decalageCoteCible = decalageCoteDebout;
        float decalageRegardCible = decalageCoteDebout;

        if (estCrawl)
        {
            hauteurCible = hauteurCrawl;
            distanceCible = distanceCrawl;
            decalageCoteCible = decalageCoteCrawl;
            decalageRegardCible = decalageCoteCrawl;
        }
        else if (estEnTrainDeViser)
        {
            // Ici on change seulement le décalage à droite.
            // On garde la même hauteur et la même distance.
            hauteurCible = hauteurDebout;
            distanceCible = distanceDebout;
            decalageCoteCible = decalageCoteVise;
            decalageRegardCible = decalageRegardVise;
        }

        hauteurActuelle = Mathf.Lerp(
            hauteurActuelle,
            hauteurCible,
            Time.deltaTime * vitesseTransition
        );

        distanceActuelleCamera = Mathf.Lerp(
            distanceActuelleCamera,
            distanceCible,
            Time.deltaTime * vitesseTransition
        );

        decalageCoteActuel = Mathf.Lerp(
            decalageCoteActuel,
            decalageCoteCible,
            Time.deltaTime * vitesseTransition
        );

        decalageRegardActuel = Mathf.Lerp(
            decalageRegardActuel,
            decalageRegardCible,
            Time.deltaTime * vitesseTransition
        );

        float rotationY = joueur.eulerAngles.y;
        Quaternion rotation = Quaternion.Euler(rotationX, rotationY, 0f);

        Vector3 ciblePosition = joueur.position + Vector3.up * hauteurActuelle;

        Vector3 offsetCamera = new Vector3(
            decalageCoteActuel,
            0f,
            -distanceActuelleCamera
        );

        Vector3 offsetRegard = new Vector3(
            decalageRegardActuel,
            0f,
            0f
        );

        Vector3 positionVoulue = ciblePosition + rotation * offsetCamera;

        Vector3 direction = (positionVoulue - ciblePosition).normalized;
        float distanceRay = Vector3.Distance(ciblePosition, positionVoulue);

        Vector3 positionFinale;

        if (Physics.SphereCast(ciblePosition, rayonCamera, direction, out RaycastHit hit, distanceRay, obstacleMask))
        {
            positionFinale = hit.point + hit.normal * 0.2f;
        }
        else
        {
            positionFinale = positionVoulue;
        }

        float t = 1f - Mathf.Exp(-smoothSpeed * Time.deltaTime);
        transform.position = Vector3.Lerp(transform.position, positionFinale, t);

        Vector3 pointRegard = ciblePosition + rotation * offsetRegard + Vector3.up * 0.15f;

        transform.LookAt(pointRegard);
    }
}