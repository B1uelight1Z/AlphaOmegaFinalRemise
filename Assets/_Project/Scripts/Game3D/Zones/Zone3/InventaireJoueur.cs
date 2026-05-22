using UnityEngine;
using System.Collections.Generic;

// Auteur: Timothy Chatelier
// Dernière date de modification: 22/05/2026
// Gère l'inventaire de clés du joueur.
// Notifie la ZoneObjectif à chaque ajout et déclenche un message de nouvel objectif
// quand toutes les clés sont collectées.
public class InventaireJoueur : MonoBehaviour
{
    [Header("Objectif clés")]
    public int totalCles = 3; // Nombre total de clés à collecter pour compléter l'objectif

    private List<string> _cles = new List<string>(); // Liste des noms de clés actuellement dans l'inventaire

    // Ajoute une clé à l'inventaire si elle n'est pas déjà présente,
    // met à jour l'objectif UI et vérifie si toutes les clés sont collectées
    public void AjouterCle(string nomCle)
    {
        if (_cles.Contains(nomCle))
        {
            Debug.Log("Clé déjà récupérée : " + nomCle);
            return;
        }

        _cles.Add(nomCle);
        Debug.Log("AjouterCle appelé : " + nomCle + ", total : " + _cles.Count + "/" + totalCles);

        // Notifie la ZoneObjectif pour mettre à jour le compteur affiché
        ZoneObjectif zone = FindObjectOfType<ZoneObjectif>();
        if (zone != null)
        {
            Debug.Log("ZoneObjectif trouvée : " + zone.name);
            zone.MettreAJourObjectif(_cles.Count);
        }
        else
        {
            Debug.LogWarning("ZoneObjectif INTROUVABLE");
        }

        if (_cles.Count >= totalCles)
        {
            ObjectifClesComplete();
        }
    }

    // Affiche le message de nouvel objectif quand toutes les clés sont ramassées.
    // NOTE : ne pas appeler JeuComplete() ici, c'est le rôle exclusif d'AppareilFinal
    void ObjectifClesComplete()
    {
        Debug.Log("Toutes les clés ont été récupérées. Aller à l'ordinateur central.");

        if (UIManager.instance != null)
        {
            UIManager.instance.ShowObjectif("Objectif : allez à l'ordinateur central");
            UIManager.instance.HideInteract();
        }
    }

    // Retourne vrai si la clé donnée est présente dans l'inventaire
    public bool PossedeCle(string nomCle)
    {
        return _cles.Contains(nomCle);
    }

    // Retire une clé de l'inventaire par son nom
    public void RetirerCle(string nomCle)
    {
        _cles.Remove(nomCle);
    }

    // Retourne le nombre de clés actuellement dans l'inventaire
    public int NombreCles()
    {
        return _cles.Count;
    }
}