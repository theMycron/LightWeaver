using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RobotHUD : MonoBehaviour
{
    [SerializeField ]private Image[] robotImages;


    // New method to update the UI image of the selected robot
    public void UpdateRobotImage(int robotNumber)
    {
        foreach (var image in robotImages)
        {
            // Grey out the image
            image.color = new Color(image.color.r, image.color.g, image.color.b, 0.5f); // Adjust the alpha value to make the image semi-transparent
        }

        // Select the new robot
        robotImages[robotNumber].color = Color.white; // Change color to white
    }
}
