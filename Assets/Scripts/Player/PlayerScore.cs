using TMPro;
using UnityEngine;

public class PlayerScore : MonoBehaviour
{
    public int count;
    public TextMeshProUGUI countText;
    public GameObject winTextObject;
    public int scoreToWin = 12;

    private void Start()
    {
        count = 0;
        SetCountText();
        winTextObject.SetActive(false);
    }

    public void SetCountText()
    {
        countText.text = "Count: " + count.ToString();

        if (count >= scoreToWin)
        {
            winTextObject.SetActive(true);
        }
    }

}
