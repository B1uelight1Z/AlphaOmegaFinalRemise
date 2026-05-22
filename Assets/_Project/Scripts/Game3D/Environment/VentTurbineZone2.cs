using UnityEngine;

/*

 * Auteur : David Champagne
 * Date : 28/04/2026
 * Projet : Alpha Omega Inversion - Jeu 3D
 *
 * Description globale :
 * Ce script crée une zone de vent qui pousse le joueur dans une direction.
 * Lorsqu'un joueur ce déplace et reste dans le trigger, une force est appliquée à son Rigidbody.
 *
 * Informations pertinentes :
 * - Ce script est utilisé pour la turbine de la zone 1,2 et 3.
 * - L'objet doit avoir un Collider avec "Is Trigger" activé.
 * - Le joueur doit avoir le tag "Player" et un Rigidbody.
 */

public class VentTurbineZone2 : MonoBehaviour
{
    // Force appliquée au joueur lorsqu'il est dans la zone de vent.
    public float forceVent = 200f;

    // Applique une force au joueur tant qu'il reste dans la zone de vent.
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            Rigidbody rb = other.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.AddForce(transform.forward * forceVent);
            }
        }
    }
}