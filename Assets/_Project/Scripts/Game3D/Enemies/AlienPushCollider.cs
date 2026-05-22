using UnityEngine;

// Auteur: David Champagne, Michael Proulx
// Dernière date de modification: 22/05/2026
// Gère l'application d'une force de poussée sur le joueur par un alien via un système de trigger.
// Détecte la présence continue ou immédiate du joueur dans la zone et applique une impulsion tout en respectant un intervalle de temps minimum.
public class AlienPushCollider : MonoBehaviour
{
    public float delaiEntrePoussees = 0.5f; // Temps d'attente minimal en secondes requis entre deux impulsions successives

    private float _tempsProchainePoussee = 0f; // Marqueur temporel indiquant à partir de quel moment la prochaine poussée devient valide

    // Déclenche une tentative de poussée dès que le collider d'un autre objet pénètre dans la zone de trigger
    private void OnTriggerEnter(Collider other)
    {
        EssayerPousser(other);
    }

    // Continue de tenter d'appliquer la poussée tant qu'un collider reste stationnaire ou en mouvement à l'intérieur du trigger
    private void OnTriggerStay(Collider other)
    {
        EssayerPousser(other);
    }

    // Vérifie le chronomètre ainsi que les tags de l'objet détecté pour valider et transmettre la force de recul au contrôleur du joueur
    private void EssayerPousser(Collider other)
    {
        if (Time.time < _tempsProchainePoussee)
        {
            return;
        }

        if (!other.CompareTag("Player"))
        {
            return;
        }

        AstronautController joueur = other.GetComponentInParent<AstronautController>();

        if (joueur == null)
        {
            return;
        }

        joueur.RecevoirPoussee(transform.position);

        _tempsProchainePoussee = Time.time + delaiEntrePoussees;
    }
}