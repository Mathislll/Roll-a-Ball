using UnityEngine;

public class PlayerScore : MonoBehaviour
{
    public int score = 0;

    public void AddScore(int points)
    {
        score += points;
        Debug.Log("Score: " + score);
    }

}
