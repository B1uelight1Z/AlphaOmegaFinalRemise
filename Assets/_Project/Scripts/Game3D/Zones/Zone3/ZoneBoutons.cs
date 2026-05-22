using UnityEngine;

// Auteur: Timothy Chatelier
// Dernière date de modification: 22/05/2026
// Gère le compteur de boutons activés dans une zone.
// Fournit des méthodes pour activer, vérifier et réinitialiser l'état des boutons.
public class ZoneButton : MonoBehaviour
{
    [Header("Objectif boutons")]
    public int boutonsNecessaires = 4; // Nombre de boutons à activer pour compléter l'objectif

    private int boutonsActives = 0; // Nombre de boutons actuellement activés

    // Incrémente le compteur de boutons activés sans dépasser le maximum requis
    public void ActiverBouton()
    {
        boutonsActives++;
        if (boutonsActives > boutonsNecessaires)
        {
            boutonsActives = boutonsNecessaires;
        }
        Debug.Log("Boutons activés : " + boutonsActives + "/" + boutonsNecessaires);
    }

    // Retourne vrai si le nombre de boutons activés atteint ou dépasse le seuil requis
    public bool TousLesBoutonsSontActives()
    {
        return boutonsActives >= boutonsNecessaires;
    }

    // Retourne le nombre de boutons actuellement activés
    public int GetBoutonsActives()
    {
        return boutonsActives;
    }

    // Retourne le nombre total de boutons nécessaires pour compléter l'objectif
    public int GetBoutonsNecessaires()
    {
        return boutonsNecessaires;
    }

    // Retourne le nombre de boutons qu'il reste à activer
    public int GetBoutonsRestants()
    {
        return Mathf.Max(0, boutonsNecessaires - boutonsActives);
    }

    // Remet le compteur de boutons activés à zéro
    public void ResetBoutons()
    {
        boutonsActives = 0;
    }
}