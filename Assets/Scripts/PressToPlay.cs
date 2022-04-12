using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PressToPlay : MonoBehaviour
{
    public Text text;
    float timeElapsed;
    float lerpDuration = 1;
    float black = 0;
    float white = 1;
    float currentColor;
    bool isBlack;

    void Update()
    {
        timeElapsed += Time.deltaTime;
        text.color = new Color(currentColor, currentColor, currentColor, 1);
        if (isBlack)
        {
            currentColor = Mathf.Lerp(black, white, timeElapsed / lerpDuration);
            if (timeElapsed >= lerpDuration)
            {
                timeElapsed = 0;
                isBlack = false;
            }
        }
        else
        {
            currentColor = Mathf.Lerp(white, black, timeElapsed / lerpDuration);
            if (timeElapsed >= lerpDuration)
            {
                timeElapsed = 0;
                isBlack = true;
            }
        }
    }
}
