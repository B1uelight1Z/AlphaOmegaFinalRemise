using UnityEngine;

// Auteur: David Champagne, Michael Proulx
// Dernière date de modification: 22/05/2026
// Gère le comportement d'une caméra à la première personne (FPS).
// Contrôle l'orientation verticale via la souris et ajuste dynamiquement la hauteur de la vue selon que le joueur est debout ou accroupi.
public class CameraFPS : MonoBehaviour
{
    [Header("Référence joueur")]
    public AstronautController astronautController; // Référence vers le script de contrôle du joueur pour vérifier s'il est accroupi

    [Header("Rotation")]
    public float sensibiliteSouris = 200f; // Vitesse de rotation verticale de la caméra avec la souris
    public float limiteVerticale = 90f; // Angle maximal de rotation vers le haut et vers le bas (évite de retourner la caméra)

    [Header("Hauteur caméra")]
    public float hauteurDebout = 1.6f; // Position locale en Y de la caméra lorsque le joueur est debout
    public float hauteurAccroupi = 0.9f; // Position locale en Y de la caméra lorsque le joueur est accroupi
    public float vitesseTransitionHauteur = 8f; // Vitesse de la transition fluide (Lerp) entre la hauteur debout et accroupie

    float rotationX = 0f; // Rotation actuelle cumulée sur l'axe X (inclinaison verticale)
    float hauteurActuelle; // Valeur lissée de la hauteur de la caméra en cours de transition

    // Verrouille le curseur et initialise la hauteur de la caméra en position debout
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        hauteurActuelle = hauteurDebout;
    }

    // Appelle à chaque frame les fonctions de gestion de la rotation et de la hauteur de la caméra
    void Update()
    {
        GererRotation();
        GererHauteur();
    }

    // Calcule et applique l'inclinaison verticale de la caméra en fonction des mouvements verticaux de la souris
    void GererRotation()
    {
        float mouseY = Input.GetAxis("Mouse Y") * sensibiliteSouris * Time.deltaTime;

        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -limiteVerticale, limiteVerticale);

        transform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);
    }

    // Détecte l'état du joueur et ajuste de manière fluide la hauteur locale de la caméra
    void GererHauteur()
    {
        bool estAccroupi = false;

        if (astronautController != null)
        {
            estAccroupi = astronautController.EstAccroupi();
        }

        float hauteurCible = estAccroupi ? hauteurAccroupi : hauteurDebout;

        hauteurActuelle = Mathf.Lerp(
            hauteurActuelle,
            hauteurCible,
            Time.deltaTime * vitesseTransitionHauteur
        );

        transform.localPosition = new Vector3(
            transform.localPosition.x,
            hauteurActuelle,
            transform.localPosition.z
        );
    }
}