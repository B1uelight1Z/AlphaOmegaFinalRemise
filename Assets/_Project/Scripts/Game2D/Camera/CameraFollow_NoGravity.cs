using UnityEngine;

/*
 * Nom du script : CameraFollow_NoGravitySwitch
 * Auteur : David Champagne
 * Date : 21/05/2026
 * Projet : AlphaOmega Inversion - Jeu 2D
 *
 * Description globale :
 * Ce script permet à la caméra de suivre l'astronaute dans un niveau où
 * la gravité ne change pas. Contrairement à CameraFollow, ce script utilise
 * un seul offset fixe pour placer la caméra par rapport au joueur.
 *
 * Informations pertinentes :
 * - Ce script est utilisé dans les niveaux sans inversion de gravité.
 */

public class CameraFollow_NoGravitySwitch : MonoBehaviour
{
    // Référence vers l'objet joueur que la caméra doit suivre.
    public GameObject astronaut;

    // Temps utilisé pour adoucir le déplacement de la caméra.
    public float timeOffset = 0.2f;

    // Décalage fixe de la caméra par rapport au joueur.
    public Vector3 offset;

    // Vitesse prévue pour adoucir un changement d'offset.
    public float offsetSmoothSpeed = 4f;

    // Variable utilisée pour calculer la vélocité interne du mouvement.
    private Vector3 velocity;

    /*
     * Fonction : Update
     * Description :
     * Met à jour la position de la caméra à chaque frame.
     */
    void Update()
    {
        if (astronaut == null)
        {
            return;
        }

        Vector3 targetPosition = astronaut.transform.position + offset;

        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPosition,
            ref velocity,
            timeOffset
        );
    }
}