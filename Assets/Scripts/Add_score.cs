using UnityEngine;
using TMPro;

public class Score : MonoBehaviour
{
    public TextMeshProUGUI textToModify;
    public string scorePrefix = "Score: ";
    public int currentScore = 0;
    public int score = 0;

    public void AddScore()
    {
        currentScore += score;
        textToModify.text = scorePrefix + currentScore;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}