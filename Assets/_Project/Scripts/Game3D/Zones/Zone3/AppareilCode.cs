using UnityEngine;

// Auteur: Timothy Chatelier
// Dernière date de modification: 22/05/2026
// Gère l'interaction du joueur avec un appareil à activer dans la séquence de code.
// Détecte la proximité du joueur et transmet l'activation au GestionnaireCode.
public class AppareilCode : MonoBehaviour
{
    [Header("Identifiant de cet appareil (1, 2, 3 ou 4)")]
    public int id; // Identifiant unique de cet appareil dans la combinaison

    [Header("Message")]
    public string messageInteraction = "Appuyez sur F pour activer la console"; // Message affiché quand le joueur est proche

    private bool joueurAProximite = false; // Vrai si le joueur se trouve dans la zone de trigger

    // Vérifie chaque frame si le joueur est proche et appuie sur E pour activer l'appareil
    void Update()
    {
        if (joueurAProximite && Input.GetKeyDown(KeyCode.F))
        {
            GestionnaireCode.instance.AppuyerAppareil(id);
        }
    }

    // Détecte quand le joueur entre dans la zone de l'appareil et affiche le message d'interaction
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            joueurAProximite = true;
            UIManager.instance.ShowInteract(messageInteraction);
        }
    }

    // Détecte quand le joueur quitte la zone de l'appareil et cache le message d'interaction
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            joueurAProximite = false;
            UIManager.instance.HideInteract();
        }
    }
}