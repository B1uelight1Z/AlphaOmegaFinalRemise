using UnityEngine;
using UnityEngine.InputSystem;

/*
 * Auteur : Timothy Chatelier
 * Date : -/03/2026
 * Projet : Alpha Omega Inversion - Jeu 2D
 *
 * Description globale :
 * Ce script gère le lance-flammes du joueur.
 * Il permet d'équiper l'arme, d'afficher son visuel, de tirer un jet de feu
 * et de retourner l'arme selon la direction du joueur.
 *
 * Informations pertinentes :
 * - Le script utilise le nouveau Input System de Unity.
 * - Le joueur doit posséder un SpriteRenderer pour déterminer sa direction.
 * - Le prefab fireJetPrefab représente le jet de feu instancié lors du tir.
 * - La touche W est utilisée pour activer le lance-flammes.
 */

public class Flamethrower : MonoBehaviour
{
    [Header("Tir")]
    // Prefab du jet de feu instancié lorsque le joueur tire.
    public GameObject fireJetPrefab;

    // Point d'où le jet de feu apparaît.
    public Transform firePoint;

    // Longueur visuelle du jet de feu.
    public float fireJetLength = 3f;

    // Délai entre deux tirs du lance-flammes.
    public float fireRate = 0.1f;

    // Son joué lorsque le lance-flammes tire.
    public AudioClip flameSound;

    [Header("Visuel")]
    // SpriteRenderer du lance-flammes affiché près du joueur.
    public SpriteRenderer flamethrowerSprite;

    [Header("Référence joueur")]
    // SpriteRenderer du joueur, utilisé pour savoir s'il regarde à gauche ou à droite.
    public SpriteRenderer playerSpriteRenderer;

    // Moment où le prochain tir est permis.
    private float nextFireTime = 0f;

    // Indique si le lance-flammes est équipé.
    private bool isEquipped = false;

    // Référence vers le jet de feu actuellement actif.
    private GameObject currentFireJet;

    // Instance statique du lance-flammes.
    public static Flamethrower instance;

    // Initialise l'instance du lance-flammes.
    void Awake()
    {
        instance = this;
    }

    // Cache le sprite du lance-flammes au début et vérifie la référence au joueur.
    void Start()
    {
        if (flamethrowerSprite != null)
        {
            flamethrowerSprite.gameObject.SetActive(false);
        }

        if (playerSpriteRenderer == null)
        {
            Debug.LogWarning("Player SpriteRenderer non assigné dans Flamethrower.");
        }
    }

    // Gère l'affichage, l'orientation et le tir du lance-flammes lorsque l'arme est équipée.
    void Update()
    {
        if (!isEquipped)
        {
            return;
        }

        if (playerSpriteRenderer == null)
        {
            return;
        }

        if (Keyboard.current == null)
        {
            return;
        }

        bool facingRight = !playerSpriteRenderer.flipX;

        FlipFlamethrower(!facingRight);

        if (Keyboard.current.wKey.isPressed)
        {
            if (flamethrowerSprite != null)
            {
                flamethrowerSprite.gameObject.SetActive(true);
                flamethrowerSprite.enabled = true;
            }

            UpdateFirePointPosition(facingRight);

            if (Time.time >= nextFireTime)
            {
                Shoot(facingRight);
                nextFireTime = Time.time + fireRate;
            }
        }
        else
        {
            if (flamethrowerSprite != null)
            {
                flamethrowerSprite.gameObject.SetActive(false);
            }

            if (currentFireJet != null)
            {
                Destroy(currentFireJet);
                currentFireJet = null;
            }
        }
    }

    // Place le point de tir du bon côté selon la direction du joueur.
    void UpdateFirePointPosition(bool facingRight)
    {
        if (firePoint == null)
        {
            return;
        }

        firePoint.localPosition = new Vector3(
            facingRight ? Mathf.Abs(firePoint.localPosition.x) : -Mathf.Abs(firePoint.localPosition.x),
            firePoint.localPosition.y,
            firePoint.localPosition.z
        );
    }

    // Crée un jet de feu, ajuste sa direction et joue le son du lance-flammes.
    void Shoot(bool facingRight)
    {
        if (fireJetPrefab == null || firePoint == null)
        {
            return;
        }

        if (currentFireJet != null)
        {
            Destroy(currentFireJet);
        }

        currentFireJet = Instantiate(fireJetPrefab, firePoint.position, Quaternion.identity);

        currentFireJet.transform.localScale = new Vector3(
            facingRight ? fireJetLength : -fireJetLength,
            1f,
            1f
        );

        currentFireJet.transform.SetParent(firePoint);

        if (AudioManager.instance != null && flameSound != null)
        {
            AudioManager.instance.PlayClipAt(flameSound, transform.position);
        }
    }

    // Active le lance-flammes et affiche son sprite.
    public void Equip()
    {
        isEquipped = true;

        if (flamethrowerSprite != null)
        {
            flamethrowerSprite.gameObject.SetActive(true);
            flamethrowerSprite.enabled = true;
        }

        Debug.Log("Flamethrower équipé.");
    }

    // Désactive le lance-flammes, cache son sprite et détruit le jet de feu actif.
    public void Unequip()
    {
        isEquipped = false;

        if (flamethrowerSprite != null)
        {
            flamethrowerSprite.gameObject.SetActive(false);
        }

        if (currentFireJet != null)
        {
            Destroy(currentFireJet);
            currentFireJet = null;
        }
    }

    // Retourne visuellement le lance-flammes selon la direction du joueur.
    public void FlipFlamethrower(bool facingLeft)
    {
        if (flamethrowerSprite == null)
        {
            return;
        }

        flamethrowerSprite.flipX = facingLeft;

        Vector3 pos = flamethrowerSprite.transform.localPosition;

        flamethrowerSprite.transform.localPosition = new Vector3(
            facingLeft ? -Mathf.Abs(pos.x) : Mathf.Abs(pos.x),
            pos.y,
            pos.z
        );
    }
}