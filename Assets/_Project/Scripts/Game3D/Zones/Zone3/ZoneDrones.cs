using UnityEngine;

// Auteur: Timothy Chatelier
// Dernière date de modification: 22/05/2026
// Suit le nombre de drones vivants dans une zone et met à jour l'UI
// tant que le joueur est présent. Cache l'UI quand tous les drones sont éliminés.
public class ZoneDrones : MonoBehaviour
{
    [Header("Ennemis de la zone")]
    public GameObject[] drones; // Tableau de tous les drones à éliminer dans cette zone

    private bool joueurDansZone = false; // Vrai si le joueur se trouve dans la zone de trigger
    private bool zoneTerminee = false;   // Vrai si tous les drones ont été éliminés

    // Met à jour le compteur de drones restants dans l'UI tant que le joueur est dans la zone.
    // Cache l'UI et marque la zone comme terminée quand tous les drones sont éliminés
    void Update()
    {
        if (!joueurDansZone || zoneTerminee) return;

        int restants = CompterDronesVivants();
        UIManager.instance.UpdateDrones(restants, drones.Length);

        if (restants <= 0)
        {
            zoneTerminee = true;
            UIManager.instance.HideDrones();
        }
    }

    // Compte et retourne le nombre de drones encore actifs (non nuls) dans le tableau
    int CompterDronesVivants()
    {
        int count = 0;
        foreach (GameObject drone in drones)
            if (drone != null) count++;
        return count;
    }

    // Détecte quand le joueur entre dans la zone et active le suivi des drones
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            joueurDansZone = true;
    }

    // Détecte quand le joueur quitte la zone et cache l'UI des drones
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            joueurDansZone = false;
            UIManager.instance.HideDrones();
        }
    }
}