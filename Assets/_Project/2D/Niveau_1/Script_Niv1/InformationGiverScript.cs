using UnityEngine;

public class InformationGiverScript : MonoBehaviour
{
    
    public Transform player;
    public float interactDistance = 1f;

    public GameObject textPrompt; // "F pour interagir"
    public GameObject textInfo;   // texte panel informative

    bool playerNear;
    private bool buttonWasPressed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (textPrompt != null)// On test l'existence du prompt/ui
        {
            textPrompt.SetActive(false);
        }

        if (textInfo != null)// On test l'existence du prompt/ui
        {
            textInfo.SetActive(false); 
        }
        buttonWasPressed = false;
        
    }

    void Update()
    {
         

        if (Vector2.Distance(player.position, transform.position) < interactDistance)
        {
            playerNear = true;
            Debug.Log(playerNear);
            if (!buttonWasPressed)
            {
                textPrompt.SetActive(true);
            }
            

            if (Input.GetKeyDown(KeyCode.F))
            {
                buttonWasPressed = true;
                Debug.Log("Test Button = "+buttonWasPressed);
                textInfo.SetActive(true);
                textPrompt.SetActive(false);
            }
        }
        else
        {
            playerNear = false;
            buttonWasPressed = false;
            Debug.Log(playerNear);
            Debug.Log("Test Button = "+buttonWasPressed);
            textPrompt.SetActive(false);
            textInfo.SetActive(false);
        }
    }
}
