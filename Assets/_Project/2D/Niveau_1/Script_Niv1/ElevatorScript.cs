using UnityEngine;

public class ElevatorConsole : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    public Transform player;
    public Transform elevatorSwitch;
    public Transform downPosition;
    public Transform upperPosition;
    public float speed = 5f;
    bool isElevatorDown;
    public GameObject textPrompt;
    public float interactDistance = 1f;
    public AudioClip sound;
    void Start()
    {
        
        if (textPrompt != null) // On test l'existence du prompt/ui
            textPrompt.SetActive(false); // On cache le prompt pour éviter qu'il s'affiche en permanence
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector2.Distance(player.position,elevatorSwitch.position)< interactDistance)
        {
            if (textPrompt != null) // On test l'existence du prompt/ui
           {
               textPrompt.SetActive(true); // On cache le prompt pour éviter qu'il s'affiche en permanence
           }
            
        }else
        {
            if (textPrompt != null) // On test l'existence du prompt/ui
            {
                textPrompt.SetActive(false); // On cache le prompt pour éviter qu'il s'affiche en permanence
            }
        }
        StartElevator();
        
        
    }
    void StartElevator()
    {
        if(Vector2.Distance(player.position,elevatorSwitch.position)< interactDistance && Input.GetKeyDown(KeyCode.F)) 
        {
            AudioManager.instance.PlayClipAt(sound, transform.position);
            if (transform.position.y <= downPosition.position.y)
            {
                isElevatorDown = true;
            }
            else if (transform.position.y >= upperPosition.position.y)
            {
                isElevatorDown = false;
            }
            
        }

        if (isElevatorDown)
        {
            transform.position = Vector2.MoveTowards(transform.position, upperPosition.position, speed * Time.deltaTime);
        }
        else
        {
            transform.position = Vector2.MoveTowards(transform.position, downPosition.position, speed * Time.deltaTime);
        }
        
    }
}