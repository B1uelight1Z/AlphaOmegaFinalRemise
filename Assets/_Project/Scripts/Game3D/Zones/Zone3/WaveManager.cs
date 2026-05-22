using UnityEngine;

// Auteur: Timothy Chatelier
// Dernière date de modification: 22/05/2026
// Surveille un groupe d'ennemis et ouvre une porte coulissante
// dès que tous les ennemis de la vague ont été éliminés.
public class WaveManager : MonoBehaviour
{
    [Header("Ennemis à éliminer")]
    public GameObject[] ennemis; // Tableau de tous les ennemis à éliminer pour ouvrir la porte

    [Header("Porte à ouvrir")]
    public PorteCoulissante porte; // Référence à la porte déverrouillée quand tous les ennemis sont éliminés

    private bool porteOuverte = false; // Vrai si la porte a déjà été ouverte pour éviter de la réactiver

    // Vérifie chaque frame si tous les ennemis sont éliminés pour ouvrir la porte
    void Update()
    {
        if (porteOuverte) return;

        if (TousElimines())
        {
            porteOuverte = true;
            porte.Activer();
            Debug.Log("Tous les ennemis éliminés — porte ouverte !");
        }
    }

    // Retourne vrai si tous les GameObjects ennemis du tableau sont null (détruits)
    bool TousElimines()
    {
        foreach (GameObject ennemi in ennemis)
        {
            if (ennemi != null) return false;
        }
        return true;
    }
}