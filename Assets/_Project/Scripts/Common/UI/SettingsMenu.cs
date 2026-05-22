using UnityEngine;
using UnityEngine.Audio;
using TMPro;
using System.Collections.Generic;
using System.Linq;

/*
 * Nom du script : SettingsMenu
 * Auteur : Michael Proulx
 * Date : 03/03/2026
 * Projet : AlphaOmega Inversion
 * 
 * Description globale :
 * Ce script permet de gérer les paramètres
 * du jeu depuis le menu des options.
 * 
 * Le système permet :
 * - de modifier le volume de la musique
 * - de modifier le volume des effets sonores
 * - d'activer ou désactiver le plein écran
 * - de changer la résolution du jeu
 * 
 * Informations pertinentes :
 * - Le script utilise AudioMixer pour gérer le son.
 * - Les résolutions disponibles sont récupérées automatiquement.
 * - L'interface utilise TextMeshPro Dropdown.
 * - Les paramètres peuvent être reliés directement
 *   à des boutons, sliders ou dropdowns UI.
 */

public class SettingsMenu : MonoBehaviour
{
    /*
     * =========================
     * SECTION : Audio
     * =========================
     */

    // AudioMixer utilisé pour contrôler les volumes du jeu.
    public AudioMixer audioMixer;

    /*
     * =========================
     * SECTION : Résolution
     * =========================
     */

    // Dropdown affichant les résolutions disponibles.
    public TMP_Dropdown resolutionDropdown;

    // Tableau contenant toutes les résolutions détectées.
    Resolution[] resolutions;

    /*
     * Fonction : Start
     * Description :
     * Initialise le menu des résolutions.
     * 
     * Cette fonction :
     * - récupère les résolutions disponibles
     * - supprime les doublons
     * - remplit le dropdown
     * - sélectionne la résolution actuelle
     */
    public void Start()
    {
        /*
         * Récupère toutes les résolutions disponibles
         * puis retire les doublons.
         */
        resolutions = Screen.resolutions
            .Select(resolution => new Resolution
            {
                width = resolution.width,
                height = resolution.height
            })
            .Distinct()
            .ToArray();

        // Supprime toutes les anciennes options du dropdown.
        resolutionDropdown.ClearOptions();

        // Liste contenant les textes affichés dans le dropdown.
        List<string> options = new List<string>();

        // Index de la résolution actuellement utilisée.
        int currentResolutionIndex = 0;

        // Parcourt toutes les résolutions disponibles.
        for (int i = 0; i < resolutions.Length; i++)
        {
            // Crée le texte de la résolution.
            string option =
                resolutions[i].width + "x" + resolutions[i].height;

            // Ajoute l'option dans la liste.
            options.Add(option);

            /*
             * Vérifie si cette résolution
             * correspond à la résolution actuelle.
             */
            if (resolutions[i].width == Screen.width &&
                resolutions[i].height == Screen.height)
            {
                currentResolutionIndex = i;
            }
        }

        // Ajoute les options au dropdown.
        resolutionDropdown.AddOptions(options);

        // Sélectionne la résolution actuelle.
        resolutionDropdown.value = currentResolutionIndex;

        // Rafraîchit l'affichage du dropdown.
        resolutionDropdown.RefreshShownValue();
    }

    /*
     * Fonction : SetMusicVolume
     * Description :
     * Modifie le volume de la musique du jeu.
     * 
     * Paramètre :
     * - volume : nouvelle valeur du volume
     */
    public void SetMusicVolume(float volume)
    {
        // Modifie le volume de la musique dans l'AudioMixer.
        audioMixer.SetFloat("Music", volume);
    }

    /*
     * Fonction : SetSoundVolume
     * Description :
     * Modifie le volume des effets sonores du jeu.
     * 
     * Paramètre :
     * - volume : nouvelle valeur du volume
     */
    public void SetSoundVolume(float volume)
    {
        // Modifie le volume des effets sonores dans l'AudioMixer.
        audioMixer.SetFloat("Sound", volume);
    }

    /*
     * Fonction : SetFullScreen
     * Description :
     * Active ou désactive le mode plein écran.
     * 
     * Paramètre :
     * - isFullScreen : état du plein écran
     */
    public void SetFullScreen(bool isFullScreen)
    {
        // Vérifie si le plein écran doit être activé.
        if (isFullScreen)
        {
            // Active le mode plein écran fenêtré.
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
        }
        else
        {
            // Active le mode fenêtré classique.
            Screen.fullScreenMode = FullScreenMode.Windowed;
        }
    }

    /*
     * Fonction : SetResolution
     * Description :
     * Change la résolution du jeu.
     * 
     * Paramètre :
     * - resolutionIndex : index de la résolution choisie
     */
    public void SetResolution(int resolutionIndex)
    {
        // Récupère la résolution sélectionnée.
        Resolution resolution = resolutions[resolutionIndex];

        /*
         * Applique la nouvelle résolution
         * en conservant le mode plein écran actuel.
         */
        Screen.SetResolution(
            resolution.width,
            resolution.height,
            Screen.fullScreen
        );
    }
}