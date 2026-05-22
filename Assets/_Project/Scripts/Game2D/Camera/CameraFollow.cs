using UnityEngine;

/*
 * Nom du script : CameraFollow
 * Auteur : Michael Proulx
 * Date : 03/03/2026
 * Projet : AlphaOmega Inversion - Jeu 2D
 * 
 * Description globale :
 * Ce script permet à la caméra de suivre l'astronaute dans le jeu 2D.
 * Il ajuste aussi la position de la caméra selon l'état de la gravité.
 * Lorsque la gravité est normale, la caméra utilise un décalage normal.
 * Lorsque la gravité est inversée, elle utilise un décalage différent afin
 * de mieux suivre le joueur.
 * 
 * Informations pertinentes :
 * - Il dépend de PlayerMovement.instance.gravityInverted pour savoir si la gravité est inversée.
 */

public class CameraFollow : MonoBehaviour
{
    // Référence vers l'objet joueur que la caméra doit suivre.
    public GameObject astronaut;

    // Temps utilisépour adoucir le déplacement de la caméra.
    public float timeOffset;

    // Décalage de la caméra lorsque la gravité est normale.
    public Vector3 normalOffset;

    // Décalage de la caméra lorsque la gravité est inversée.
    public Vector3 invertedOffset;

    // Vitesse à laquelle la caméra passe d'un offset à l'autre.
    public float offsetSmoothSpeed = 4f;

    // Variable utilisée pour calculer la vélocité interne du mouvement.
    private Vector3 velocity;

    // Offset actuellement utilisé par la caméra.
    private Vector3 currentOffset;

    /*
     * Fonction : Start
     * Description :
     * Initialise l'offset actuel de la caméra avec l'offset normal.
     * Cette fonction est appelée automatiquement par Unity au début de la scène.
     */
    void Start()
    {
        currentOffset = normalOffset;
    }

    /*
     * Fonction : Update
     * Description :
     * Met à jour la position de la caméra à chaque frame.
     * La fonction choisit l'offset approprié selon l'état de la gravité,
     
     */
    void Update()
    {
        Vector3 targetOffset = PlayerMovement.instance.gravityInverted ? invertedOffset : normalOffset;

        currentOffset = Vector3.Lerp(currentOffset, targetOffset, offsetSmoothSpeed * Time.deltaTime);

        transform.position = Vector3.SmoothDamp(
            transform.position,
            astronaut.transform.position + currentOffset,
            ref velocity,
            timeOffset
        );
    }
}