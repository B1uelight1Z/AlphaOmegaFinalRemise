using UnityEngine;
using TMPro;

public class ObjectiveStatusText : MonoBehaviour
{
    public ZoneButtonManager zoneButtonManager;
    public TextMeshProUGUI texteObjectif;

    public string texteAvantComplet = "Boutons activés : ";
    public string texteComplet = "Objectif complété : allez vers l’ascenseur";

    void Start()
    {
        if (texteObjectif == null)
        {
            texteObjectif = GetComponent<TextMeshProUGUI>();
        }
    }

    void Update()
    {
        if (zoneButtonManager == null || texteObjectif == null)
        {
            return;
        }

        if (zoneButtonManager.TousLesBoutonsSontActives())
        {
            texteObjectif.text = texteComplet;
        }
        else
        {
            int actives = zoneButtonManager.GetBoutonsActives();
            int necessaires = zoneButtonManager.GetBoutonsNecessaires();

            texteObjectif.text = texteAvantComplet + actives + "/" + necessaires;
        }
    }
}