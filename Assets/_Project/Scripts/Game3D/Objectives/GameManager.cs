using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Score")]
    public int score = 0;

    [Header("Écran de fin")]
    public GameObject ecranVictoire;
    public float delaiAvantMenu = 5f;
    public TextMeshProUGUI scoreText;

    private bool jeuDejaComplete = false;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        if (ecranVictoire != null)
        {
            ecranVictoire.SetActive(false);
        }
    }

    void Start()
    {
        ResetScore();
    }

    public void AddScore(int amount)
    {
        score += amount;
        UpdateScoreUI();
    }

    public void ResetScore()
    {
        score = 0;
        UpdateScoreUI();
    }

    void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "" + score;
        }
    }

    public void JeuComplete()
    {
        if (jeuDejaComplete)
        {
            return;
        }

        jeuDejaComplete = true;

        if (ecranVictoire != null)
        {
            ecranVictoire.SetActive(true);
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        StartCoroutine(RetourMenu());
    }

    IEnumerator RetourMenu()
    {
        yield return new WaitForSeconds(delaiAvantMenu);
        SceneManager.LoadScene(0);
    }
}