using UnityEngine;

public class ZoneButtonManager : MonoBehaviour
{
    [Header("Boutons de la zone")]
    public BoutonInteract[] boutonsDeLaZone;

    public int GetBoutonsActives()
    {
        int total = 0;

        foreach (BoutonInteract bouton in boutonsDeLaZone)
        {
            if (bouton != null && bouton.isActivated)
            {
                total++;
            }
        }

        return total;
    }

    public int GetBoutonsNecessaires()
    {
        return boutonsDeLaZone.Length;
    }

    public int GetBoutonsRestants()
    {
        return Mathf.Max(0, GetBoutonsNecessaires() - GetBoutonsActives());
    }

    public bool TousLesBoutonsSontActives()
    {
        return GetBoutonsActives() >= GetBoutonsNecessaires();
    }
}