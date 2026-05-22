using UnityEngine;
using TMPro;

// Auteur: Timothy Chatelier, David Champagne, Michael Proulx
// Dernière date de modification: 22/05/2026
// Gestionnaire centralisé de l'interface utilisateur (UI) utilisant un patron Singleton.
// Centralise l'affichage, la mise à jour et le masquage des différents éléments textuels du jeu.
public class UIManager : MonoBehaviour
{
    public static UIManager instance; // Instance unique accessible globalement (Singleton)

    [Header("Interaction")]
    public TextMeshProUGUI interactText; // Élément textuel affiché lors des interactions possibles en jeu

    [Header("Drones")]
    public TextMeshProUGUI dronesText; // Élément textuel affichant l'état des drones de surveillance

    [Header("Objectif")]
    public TextMeshProUGUI objectifText; // Élément textuel dédié aux instructions et objectifs actuels du joueur

    [Header("Code Console")]
    public TextMeshProUGUI codeText; // Élément textuel affichant la combinaison ou l'état de la console de code

    [Header("Intercom")]
    public TextMeshProUGUI intercomText; // Élément textuel affichant les messages de dialogue ou les alertes de l'intercom

    // Initialise l'instance Singleton à l'éveil du script
    void Awake()
    {
        instance = this;
    }

    // Réinitialise l'interface en masquant tous les éléments textuels au lancement de la scène
    void Start()
    {
        HideInteract();
        HideDrones();
        HideObjectif();
        HideCode();
        HideIntercom();
    }

    // Affiche le message d'interaction textuel spécifié à l'écran
    public void ShowInteract(string message)
    {
        if (interactText == null)
        {
            return;
        }

        interactText.text = message;
        interactText.gameObject.SetActive(true);
    }

    // Masque l'élément textuel d'interaction
    public void HideInteract()
    {
        if (interactText != null)
        {
            interactText.gameObject.SetActive(false);
        }
    }

    // Met à jour et affiche le compteur textuel des drones restants sur le total
    public void UpdateDrones(int restants, int total)
    {
        if (dronesText == null)
        {
            return;
        }

        dronesText.gameObject.SetActive(true);
        dronesText.text = $"Drones : {restants} / {total}";
    }

    // Masque l'élément textuel du compteur de drones
    public void HideDrones()
    {
        if (dronesText != null)
        {
            dronesText.gameObject.SetActive(false);
        }
    }

    // Met à jour et affiche la progression de l'objectif de collecte des clés
    public void UpdateObjectifCles(int clesCollectees, int total)
    {
        if (objectifText == null)
        {
            return;
        }

        objectifText.gameObject.SetActive(true);
        objectifText.text = $"Objectif : {clesCollectees} / {total} clés collectées";
    }

    // Affiche un message d'objectif textuel personnalisé à l'écran
    public void ShowObjectif(string message)
    {
        if (objectifText == null)
        {
            return;
        }

        objectifText.gameObject.SetActive(true);
        objectifText.text = message;
    }

    // Masque l'élément textuel dédié aux objectifs
    public void HideObjectif()
    {
        if (objectifText != null)
        {
            objectifText.gameObject.SetActive(false);
        }
    }

    // Affiche le message ou le code de la console spécifié à l'écran
    public void ShowCode(string message)
    {
        if (codeText == null)
        {
            return;
        }

        codeText.gameObject.SetActive(true);
        codeText.text = message;
    }

    // Masque l'élément textuel du code de la console
    public void HideCode()
    {
        if (codeText != null)
        {
            codeText.gameObject.SetActive(false);
        }
    }

    // Affiche la transmission ou le message textuel de l'intercom à l'écran
    public void ShowIntercom(string message)
    {
        if (intercomText == null)
        {
            return;
        }

        intercomText.gameObject.SetActive(true);
        intercomText.text = message;
    }

    // Masque l'élément textuel de l'intercom
    public void HideIntercom()
    {
        if (intercomText != null)
        {
            intercomText.gameObject.SetActive(false);
        }
    }
}