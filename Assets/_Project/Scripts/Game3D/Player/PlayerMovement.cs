using UnityEngine;

// Auteur: Timothy Chatelier
// Derniere date de modification: 22/05/2026
// Gere les deplacements au sol du joueur, la gravite et le saut a l'aide d'un CharacterController.
// Aligne les directions de deplacement par rapport a l'orientation de la camera.
public class PlayerControler : MonoBehaviour
{
    public CharacterController cc; // Reference vers le composant de deplacement physique de Unity
    public Transform cameraTransform; // Reference vers la camera pour orienter les mouvements selon la vue

    public float vitesseMove = 5f; // Vitesse de deplacement globale du joueur
    public float gravity = 9.81f; // Force de gravite appliquee pour attirer le joueur vers le bas
    public float jumpForce = 5f; // Impulsion verticale appliquee lors d'un saut

    private Vector3 velocity; // Stocke le vecteur de velocite actuelle, principalement pour l'axe vertical (saut et gravite)

    // Verifie chaque frame les commandes du joueur, calcule les trajectoires et applique le mouvement final
    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        float strafeFactor = 0.7f; // Reduit la vitesse de deplacement laterale (gauche/droite) pour plus de realisme

        Vector3 move = forward * vertical + right * horizontal * strafeFactor;

        if (cc.isGrounded)
        {
            if (velocity.y < 0)
                velocity.y = -2f;

            if (Input.GetButtonDown("Jump"))
            {
                velocity.y = jumpForce;
            }
        }

        velocity.y -= gravity * Time.deltaTime;

        Vector3 finalMove = move * vitesseMove + new Vector3(0, velocity.y, 0);
        cc.Move(finalMove * Time.deltaTime);
    }
}