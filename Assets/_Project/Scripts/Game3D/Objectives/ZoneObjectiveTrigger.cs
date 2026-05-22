using UnityEngine;

public class ZoneTextActivator : MonoBehaviour
{
    public GameObject texteObjectif;

    private void Start()
    {
        if (texteObjectif != null)
        {
            texteObjectif.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        if (texteObjectif != null)
        {
            texteObjectif.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        if (texteObjectif != null)
        {
            texteObjectif.SetActive(false);
        }
    }
}