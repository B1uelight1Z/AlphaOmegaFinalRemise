using UnityEngine;

// Auteur: Timothy Chatelier
// Derni�re date de modification: 22/05/2026
// G�re l'interaction avec la machine permettant de d�sactiver l'alarme.
// Affiche le message d'interaction seulement si l'alarme est active
// et permet au joueur de la d�sactiver en appuyant sur E.
public class MachineAlarme : MonoBehaviour
{
    [Header("Message")]
    public string messageInteraction = "Appuyez sur F pour d�sactiver l'alarme"; // Message affich� quand le joueur est proche et l'alarme active

    private bool joueurAProximite = false; // Vrai si le joueur se trouve dans la zone de trigger

    // Chaque frame, affiche le message et permet la d�sactivation si le joueur est proche
    // et que l'alarme est active. Cache le message si l'alarme est d�j� inactive.
    void Update()
    {
        if (joueurAProximite && GestionnaireAlarme.instance.AlarmeActive)
        {
            UIManager.instance.ShowInteract(messageInteraction);

            if (Input.GetKeyDown(KeyCode.F))
            {
                GestionnaireAlarme.instance.DesactiverAlarme();
                UIManager.instance.HideInteract();
            }
        }
        else if (joueurAProximite && !GestionnaireAlarme.instance.AlarmeActive)
        {
            // Cache le message si le joueur est proche mais que l'alarme n'est plus active
            UIManager.instance.HideInteract();
        }
    }

    // D�tecte quand le joueur entre dans la zone de la machine
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            joueurAProximite = true;
    }

    // D�tecte quand le joueur quitte la zone et cache le message d'interaction
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            joueurAProximite = false;
            UIManager.instance.HideInteract();
        }
    }
}