using TMPro;
using UnityEngine;

public class PinKnockdown : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public float lowerLimit = 250f;
    public float upperLimit = 290f;

    private bool hasScored = false;

    void Update()
    {
        float xAngle = transform.eulerAngles.x;

        // Check if the pin has tipped beyond the allowed angle and hasn’t scored yet
        if (!hasScored && (xAngle < lowerLimit || xAngle > upperLimit))
        {
            IncrementScore();
            hasScored = true;
        }
    }

    void IncrementScore()
    {
        int currentScore = int.Parse(scoreText.text);
        currentScore += 1;
        scoreText.text = currentScore.ToString(); // No wrapping, no formatting limitation
    }
}
