using System.Collections.Generic;
using UnityEngine;

/*
 * Nom du script : LightningCollider
 * Auteur : Timothy Chatelier
 * Date : 03/03/2026
 * Projet : AlphaOmega Inversion - Jeu 2D
 * 
 * Description globale :
 * Ce script permet de mettre à jour automatiquement
 * le PolygonCollider2D d'un éclair animé.
 * 
 * Puisque l'animation change constamment de sprite,
 * le collider doit être recalculé à chaque frame afin
 * de toujours correspondre parfaitement à la forme visuelle
 * de l'éclair.
 * 
 * Informations pertinentes :
 * - Le script utilise les Physics Shapes des sprites.
 * - Chaque sprite de l'animation doit avoir une Physics Shape.
 * - Le collider est reconstruit automatiquement en temps réel.
 * - Le script nécessite :
 *      - un PolygonCollider2D
 *      - un SpriteRenderer
 *      - un Animator
 */

public class LightningCollider : MonoBehaviour
{
    // Référence vers le collider polygonal utilisé par l'éclair.
    private PolygonCollider2D col;

    // Référence vers l'Animator de l'éclair.
    private Animator animator;

    // Référence vers le SpriteRenderer affichant le sprite actuel.
    private SpriteRenderer sr;

    /*
     * Fonction : Start
     * Description :
     * Initialise les différentes composantes nécessaires
     * au fonctionnement du script.
     * 
     * Cette fonction est appelée automatiquement
     * au début de la scène.
     */
    void Start()
    {
        // Récupère le PolygonCollider2D attaché à l'objet.
        col = GetComponent<PolygonCollider2D>();

        // Récupère le SpriteRenderer attaché à l'objet.
        sr = GetComponent<SpriteRenderer>();

        // Récupère l'Animator attaché à l'objet.
        animator = GetComponent<Animator>();
    }

    /*
     * Fonction : Update
     * Description :
     * Met à jour le collider de l'éclair à chaque frame.
     * 
     * Le script récupère la forme physique (Physics Shape)
     * du sprite actuellement affiché puis applique cette forme
     * au PolygonCollider2D.
     * 
     * Cela permet au collider de toujours suivre précisément
     * l'animation visuelle de l'éclair.
     */
    void Update()
    {
        // Récupère le sprite actuellement affiché.
        Sprite spriteActuel = sr.sprite;

        // Définit le nombre de formes physiques du collider.
        col.pathCount = spriteActuel.GetPhysicsShapeCount();

        // Parcourt toutes les formes physiques du sprite.
        for (int i = 0; i < spriteActuel.GetPhysicsShapeCount(); i++)
        {
            // Liste contenant les points du collider.
            List<Vector2> points = new List<Vector2>();

            // Récupère les points de la forme physique du sprite.
            spriteActuel.GetPhysicsShape(i, points);

            // Applique les points au PolygonCollider2D.
            col.SetPath(i, points);
        }
    }
}