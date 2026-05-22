using UnityEngine;

public class ButtonInteract : MonoBehaviour
{
    public DoorElevator door;   // La porte que ce bouton contrôle
    public Transform player;    // Joueur
    public AudioClip sound;
    public float interactDistance = 1f; // Distance pour appuyer

    public GameObject textPrompt;
    
    
    
    void Start()
    {
        
        if (textPrompt != null) // On test l'existence du prompt/ui
            textPrompt.SetActive(false); // On cache le prompt pour éviter qu'il s'affiche en permanence
    }
    void Update()
    {
        // Vérifie si le joueur est proche et appuie sur F
        if (Vector2.Distance(player.position, transform.position) < interactDistance )
        {
            if (textPrompt != null)
            {
                textPrompt.SetActive(true);
            }

            if (Input.GetKeyDown(KeyCode.F))
            {
                AudioManager.instance.PlayClipAt(sound, transform.position);
                door.ToggleDoor(); // Appelle la fonction de la porte
            }
            
        }
        else
        {
            if (textPrompt != null)
                textPrompt.SetActive(false);
        }
    }
}