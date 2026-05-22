using UnityEngine;

public class ButtonInteractTwoDoors : MonoBehaviour
{
    public DoorElevator door1;
    public DoorElevator door2;
    public Transform player;
    public float interactDistance = 1f;
    public GameObject textPrompt;
    public AudioClip sound;

    /*void Start() 
    {
        
        if (textPrompt != null) // On test l'existence du prompt/ui
            textPrompt.SetActive(false); // On cache le prompt pour éviter qu'il s'affiche en permanence
    }*/

    void Update()
    {
        if (Vector2.Distance(player.position, transform.position) < interactDistance )
        {
            if(Input.GetKeyDown(KeyCode.F)){
                AudioManager.instance.PlayClipAt(sound, transform.position);
                // Inverse l'état des portes
                bool door1CurrentlyClosed =
                    Vector2.Distance(door1.transform.position, door1.closedPosition.position) < 0.01f;
                door1.AlwayClosed(); // La porte ce ferme à jamais.
                door2.ToggleDoor(); // La porte S'ouvre}

            }
            if (textPrompt != null)
            {
                textPrompt.SetActive(true);
            }
        }
        else
        {
            if (textPrompt != null)
                textPrompt.SetActive(false);
        }
    }
}