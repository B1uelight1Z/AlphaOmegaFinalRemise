using UnityEngine;
using System.Collections;

// Auteur: Timothy Chatelier
// Dernière date de modification: 22/05/2026
// Gère le comportement d'une caméra de surveillance : rotation entre deux angles,
// détection du joueur par raycast et déclenchement de l'alarme si détecté.
public class SurveillanceCamera : MonoBehaviour
{
    [Header("Mouvement")]
    public float angleA = -45f;   // Premier angle limite de la rotation (en degrés)
    public float angleB = 45f;    // Deuxième angle limite de la rotation (en degrés)
    public float speed = 2.0f;    // Vitesse de rotation de la caméra
    public float waitTime = 1.5f; // Temps de pause en secondes à chaque angle limite

    [Header("Detection")]
    public GameObject coneDetection;                        // GameObject représentant le cône de détection visuel
    public float detectionRange = 20f;                      // Portée maximale du raycast de détection
    public Color normalColor = new Color(1f, 1f, 0f, 0.3f); // Couleur du cône en état normal (jaune semi-transparent)
    public Color alertColor = new Color(1f, 0f, 0f, 0.3f);  // Couleur du cône quand le joueur est détecté (rouge semi-transparent)
    public LayerMask masqueDetection;                        // Masque de layers utilisé par le raycast pour filtrer les collisions

    private float targetAngle;      // Angle cible actuel vers lequel la caméra tourne
    private bool isWaiting = false; // Vrai si la caméra est en pause à un angle limite
    private Material coneMaterial;  // Instance unique du matériau du cône pour changer sa couleur sans affecter les autres
    private float rotationX;        // Angle de rotation X initial conservé pendant toute la rotation

    // Initialise l'angle de départ, crée une instance unique du matériau
    // du cône de détection et applique la couleur normale
    void Start()
    {
        rotationX = transform.localEulerAngles.x;
        float currentAngle = transform.localEulerAngles.y;

        // Normalise l'angle entre -180 et 180
        if (currentAngle > 180f)
        {
            currentAngle -= 360f;
        }

        targetAngle = angleB;
        transform.localRotation = Quaternion.Euler(rotationX, currentAngle, 0f);

        // Crée un matériau unique pour le cône afin d'éviter de modifier
        // le matériau partagé entre tous les objets
        if (coneDetection != null)
        {
            Renderer renderer = coneDetection.GetComponent<Renderer>();
            if (renderer != null)
            {
                coneMaterial = new Material(renderer.material);
                renderer.material = coneMaterial;
                coneMaterial.color = normalColor;
            }
            else
            {
                Debug.LogWarning("Le coneDetection n'a pas de Renderer.");
            }
        }
        else
        {
            Debug.LogWarning("Assigne le coneDetection dans l'inspecteur.");
        }
    }

    // Vérifie la détection du joueur et gère la rotation de la caméra chaque frame
    void Update()
    {
        CheckForPlayer();

        if (isWaiting) return;

        float currentAngle = transform.localEulerAngles.y;

        // Normalise l'angle entre -180 et 180
        if (currentAngle > 180f)
        {
            currentAngle -= 360f;
        }

        float newAngle = Mathf.MoveTowards(
            currentAngle,
            targetAngle,
            speed * Time.deltaTime * 10f
        );

        transform.localRotation = Quaternion.Euler(rotationX, newAngle, 0f);

        // Lance la pause une fois l'angle cible atteint
        if (Mathf.Abs(newAngle - targetAngle) < 0.1f)
        {
            StartCoroutine(WaitAtPoint());
        }
    }

    // Lance un raycast vers l'avant pour détecter le joueur.
    // Change la couleur du cône et déclenche l'alarme si le joueur est touché
    void CheckForPlayer()
    {
        if (coneMaterial == null) return;

        Debug.DrawRay(transform.position, transform.forward * detectionRange, Color.red);

        bool hasHit = Physics.Raycast(
            transform.position,
            transform.forward,
            out RaycastHit hit,
            detectionRange,
            masqueDetection
        );

        if (hasHit)
        {
            if (hit.collider.CompareTag("Player"))
            {
                coneMaterial.color = alertColor;

                if (GestionnaireAlarme.instance != null)
                {
                    GestionnaireAlarme.instance.DeclencherAlarme();
                }
            }
            else
            {
                coneMaterial.color = normalColor;
            }
        }
        else
        {
            coneMaterial.color = normalColor;
        }
    }

    // Remet la couleur du cône de détection à la couleur normale (appelé lors de la réinitialisation de l'alarme)
    public void ResetCouleurDetection()
    {
        if (coneMaterial != null)
        {
            coneMaterial.color = normalColor;
        }
    }

    // Attend à l'angle actuel pendant waitTime secondes, puis inverse la direction de rotation
    IEnumerator WaitAtPoint()
    {
        isWaiting = true;
        yield return new WaitForSeconds(waitTime);

        targetAngle = (targetAngle == angleA) ? angleB : angleA;

        isWaiting = false;
    }
}