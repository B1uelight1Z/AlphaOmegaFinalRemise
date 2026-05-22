using UnityEngine;

// Auteur: Timothy Chatelier
// Dernière date de modification: 22/05/2026
// Affiche ou cache l'indicateur de progression du puzzle de code dans l'UI
// selon la présence du joueur dans la zone et l'état de résolution.
public class ZoneCode : MonoBehaviour
{
    // Affiche l'indicateur de progression du code quand le joueur entre dans la zone,
    // seulement si la combinaison n'est pas encore résolue
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!GestionnaireCode.instance.resolu)
                UIManager.instance.ShowCode(GestionnaireCode.instance.GetIndicateur());
        }
    }

    // Cache l'affichage du code quand le joueur quitte la zone
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            UIManager.instance.HideCode();
    }
}