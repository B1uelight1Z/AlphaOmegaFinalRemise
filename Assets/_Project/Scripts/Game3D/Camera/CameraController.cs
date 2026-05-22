using UnityEngine;

// Auteur: David Champagne, Michael Proulx
// Dernière date de modification: 22/05/2026
// Gère l'alternance et l'activation des différents systèmes de caméras (FPS et TPS).
// Permet au joueur de basculer entre la première et la troisième personne via des touches dédiées et informe le contrôleur du joueur du changement.
public class CameraController : MonoBehaviour
{
    public GameObject camFPS; // Référence vers l'objet ou la caméra Première Personne
    public GameObject camTPS; // Référence vers l'objet ou la caméra Troisième Personne
    public AstronautController joueurController; // Référence vers le script de contrôle du joueur pour lui synchroniser la caméra active

    private bool changementCameraAutorise = true; // Détermine si le joueur a le droit de changer de vue à ce moment précis

    // Active la caméra première personne par défaut au lancement du script
    void Start()
    {
        ActiverCamera(camFPS);
    }

    // Surveille les entrées du joueur à chaque frame pour basculer de caméra si l'autorisation est active
    void Update()
    {
        if (!changementCameraAutorise)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.V))
        {
            if (camFPS.activeSelf)
            {
                ActiverCamera(camTPS);
            }
            else
            {
                ActiverCamera(camFPS);
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ActiverCamera(camFPS);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ActiverCamera(camTPS);
        }
    }

    // Désactive toutes les caméras pour activer uniquement celle passée en paramètre, puis notifie le contrôleur du joueur
    public void ActiverCamera(GameObject cameraChoisie)
    {
        if (camFPS != null)
        {
            camFPS.SetActive(false);
        }

        if (camTPS != null)
        {
            camTPS.SetActive(false);
        }

        if (cameraChoisie != null)
        {
            cameraChoisie.SetActive(true);
        }

        if (joueurController != null && cameraChoisie != null)
        {
            bool estTPS = cameraChoisie == camTPS;
            joueurController.ChangerCameraActive(cameraChoisie.transform, estTPS);
        }
    }

    // Alerte le système pour forcer l'activation immédiate de la caméra FPS
    public void ForcerFPS()
    {
        ActiverCamera(camFPS);
    }

    // Alerte le système pour forcer l'activation immédiate de la caméra TPS
    public void ForcerTPS()
    {
        ActiverCamera(camTPS);
    }

    // Permet de verrouiller ou de déverrouiller dynamiquement la possibilité de changer de caméra depuis un autre script
    public void AutoriserChangementCamera(bool autoriser)
    {
        changementCameraAutorise = autoriser;
    }
}