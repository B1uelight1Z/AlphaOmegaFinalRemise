using UnityEngine;

// Auteur: Timothy Chatelier
// Dernière date de modification: 22/05/2026
// Affiche le message d'objectif de collecte de clés dans l'UI
// quand le joueur entre dans la zone, et le met à jour à chaque clé ramassée.
public class ZoneObjectif : MonoBehaviour
{
    [Header("Message affiché dans la zone")]
    public string prefixeObjectif = "Objectif :\nCollecter les clés"; // Texte de base affiché avant le compteur de clés
    public int totalCles = 3;                                          // Nombre total de clés à collecter pour compléter l'objectif

    private int clesCollectees = 0;      // Nombre de clés collectées par le joueur jusqu'à présent
    private bool joueurDansZone = false; // Vrai si le joueur se trouve dans la zone de trigger

    // Met à jour le compteur de clés collectées et rafraîchit l'affichage si le joueur est dans la zone
    public void MettreAJourObjectif(int nbCles)
    {
        clesCollectees = nbCles;
        Debug.Log("Objectif clés : " + clesCollectees + "/" + totalCles);
        AfficherObjectif();
    }

    // Affiche le message d'objectif avec le compteur de clés uniquement si le joueur est dans la zone
    void AfficherObjectif()
    {
        if (!joueurDansZone) return;

        if (UIManager.instance != null)
        {
            UIManager.instance.ShowObjectif(
                prefixeObjectif + " " + clesCollectees + "/" + totalCles
            );
        }
    }

    // Détecte quand le joueur entre dans la zone et affiche l'objectif actuel
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            joueurDansZone = true;
            AfficherObjectif();
        }
    }

    // Détecte quand le joueur quitte la zone et cache l'affichage de l'objectif
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            joueurDansZone = false;
            if (UIManager.instance != null)
            {
                UIManager.instance.HideObjectif();
            }
        }
    }
}