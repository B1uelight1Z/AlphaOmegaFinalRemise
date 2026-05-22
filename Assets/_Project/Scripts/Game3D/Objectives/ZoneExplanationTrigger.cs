using UnityEngine;

public class ZoneExplanationTrigger : MonoBehaviour
{
    [Header("UI")]
    public GameObject panelExplication;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip sonIntro;

    [Header("Contrôle")]
    public KeyCode toucheContinuer = KeyCode.Space;

    private bool dejaActive = false;
    private bool enPauseExplication = false;

    private AstronautController joueurControle;

    private void Start()
    {
        if (panelExplication != null)
        {
            panelExplication.SetActive(false);
        }
    }

    private void Update()
    {
        if (!enPauseExplication)
        {
            return;
        }

        if (Input.GetKeyDown(toucheContinuer))
        {
            FermerExplication();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (dejaActive)
        {
            return;
        }

        if (!other.CompareTag("Player"))
        {
            return;
        }

        dejaActive = true;
        OuvrirExplication();
    }

    void OuvrirExplication()
    {
        enPauseExplication = true;

        GameObject joueur = GameObject.FindGameObjectWithTag("Player");

        if (joueur != null)
        {
            joueurControle = joueur.GetComponent<AstronautController>();

            if (joueurControle != null)
            {
                joueurControle.BloquerControle(true);
            }
        }

        if (panelExplication != null)
        {
            panelExplication.SetActive(true);
        }

        if (audioSource != null && sonIntro != null)
        {
            audioSource.PlayOneShot(sonIntro);
        }

        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void FermerExplication()
    {
        enPauseExplication = false;

        if (panelExplication != null)
        {
            panelExplication.SetActive(false);
        }

        Time.timeScale = 1f;

        if (joueurControle != null)
        {
            joueurControle.BloquerControle(false);
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Collider col = GetComponent<Collider>();

        if (col != null)
        {
            col.enabled = false;
        }
    }
}