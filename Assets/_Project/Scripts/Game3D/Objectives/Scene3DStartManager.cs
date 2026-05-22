using UnityEngine;

public class Scene3DStartManager : MonoBehaviour
{
    [Header("Joueur")]
    public Transform player;
    public Rigidbody playerRigidbody;

    [Header("Spawn points 3D")]
    public Transform spawnZone1;
    public Transform spawnZone2;
    public Transform spawnZone3;

    [Header("Planchers de sécurité seulement si départ depuis le menu")]
    public GameObject plancherSecuriteZone2;
    public GameObject plancherSecuriteZone3;

    void Start()
    {
        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

            if (playerObject != null)
            {
                player = playerObject.transform;
            }
        }

        if (playerRigidbody == null && player != null)
        {
            playerRigidbody = player.GetComponent<Rigidbody>();
        }

        int zoneDepart = PlayerPrefs.GetInt("ZoneDepart3D", 1);

        Debug.Log("ZoneDepart3D lue : " + zoneDepart);

        DesactiverTousLesPlanchersSecurite();
        PlacerJoueur(zoneDepart);
        ActiverPlancherSiDepartDepuisMenu(zoneDepart);
    }

    void PlacerJoueur(int zoneDepart)
    {
        Transform spawnChoisi = spawnZone1;

        if (zoneDepart == 2)
        {
            spawnChoisi = spawnZone2;
        }
        else if (zoneDepart == 3)
        {
            spawnChoisi = spawnZone3;
        }

        if (player == null || spawnChoisi == null)
        {
            Debug.LogWarning("Player ou spawn point manquant dans Scene3DStartManager.");
            return;
        }

        if (playerRigidbody != null)
        {
            playerRigidbody.linearVelocity = Vector3.zero;
            playerRigidbody.angularVelocity = Vector3.zero;
        }

        player.position = spawnChoisi.position;
        player.rotation = spawnChoisi.rotation;

        Physics.SyncTransforms();

        if (GameOverManager.instance != null)
        {
            GameOverManager.instance.SetCheckpoint(spawnChoisi.position, spawnChoisi.rotation);
        }

        Debug.Log("Joueur placé à la zone 3D : " + zoneDepart);
    }

    void ActiverPlancherSiDepartDepuisMenu(int zoneDepart)
    {
        if (zoneDepart == 2)
        {
            ActiverPlancher(plancherSecuriteZone2);
            Debug.Log("Plancher de sécurité Zone 2 activé.");
        }
        else if (zoneDepart == 3)
        {
            ActiverPlancher(plancherSecuriteZone3);
            Debug.Log("Plancher de sécurité Zone 3 activé.");
        }
        else
        {
            Debug.Log("Zone 1 choisie : aucun plancher de sécurité activé.");
        }
    }

    void ActiverPlancher(GameObject plancher)
    {
        if (plancher == null)
        {
            return;
        }

        plancher.SetActive(true);

        Collider colliderPlancher = plancher.GetComponent<Collider>();

        if (colliderPlancher != null)
        {
            colliderPlancher.enabled = true;
        }
    }

    void DesactiverTousLesPlanchersSecurite()
    {
        if (plancherSecuriteZone2 != null)
        {
            plancherSecuriteZone2.SetActive(false);
        }

        if (plancherSecuriteZone3 != null)
        {
            plancherSecuriteZone3.SetActive(false);
        }
    }
}