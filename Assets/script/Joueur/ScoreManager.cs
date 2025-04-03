using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance; // Pour y accéder depuis d'autres scripts
    public int score = 0;
    public TextMeshProUGUI scoreText; // Texte pour afficher le score

    void Awake()
    {
        Instance = this; // Initialise le singleton
    }

    public void AddScore(int points)
    {
        score += points;
        UpdateScoreUI();
    }

    void UpdateScoreUI()
    {
        scoreText.text = $"Score: {score}";
    }
}