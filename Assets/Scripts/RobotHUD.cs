using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RobotHUD : MonoBehaviour
{
    [SerializeField] private Image qKeyImage; // Image for the Q key
    [SerializeField] private Image[] robotImages; // Array of robot images
    [SerializeField] private Image eKeyImage; // Image for the E key
    [SerializeField] private GameObject robotBar; // Parent object of the robot images
    [SerializeField] private HorizontalLayoutGroup layoutGroup; // Layout group for automatic resizing

    private List<GameObject> activeRobotImages = new List<GameObject>(); // List to track active robot images

    // Method to initialize the HUD based on the number of robots in the current level
    public void InitializeHUD(int numberOfRobots)
    {
        // Clear any previously active robot images
        ClearActiveRobotImages();

        // Hide all robot images initially
        foreach (var image in robotImages)
        {
            image.gameObject.SetActive(false);
        }

        // Show the required number of robot images and add them to the active list
        for (int i = 0; i < numberOfRobots; i++)
        {
            robotImages[i].gameObject.SetActive(true);
            activeRobotImages.Add(robotImages[i].gameObject);
        }

        // Hide the robot bar if there is only one robot
        if (numberOfRobots <= 1)
        {
            robotBar.SetActive(false);
        }
        else
        {
            robotBar.SetActive(true);
        }

        // Adjust the size of the images based on the number of active images
        AdjustImageSizes(numberOfRobots);
    }

    // Method to update the UI image of the selected robot
    public void UpdateRobotImage(int robotNumber)
    {
        foreach (var image in robotImages)
        {
            // Grey out the image
            image.color = new Color(image.color.r, image.color.g, image.color.b, 0.5f); // Adjust the alpha value to make the image semi-transparent
        }

        // Select the new robot
        if (robotNumber >= 0 && robotNumber < robotImages.Length)
        {
            robotImages[robotNumber].color = Color.white; // Change color to white
        }
        else
        {
            Debug.LogError("Robot number out of range. Unable to update robot image.");
        }
    }

    // Method to clear active robot images from the list
    private void ClearActiveRobotImages()
    {
        foreach (var robotImage in activeRobotImages)
        {
            robotImage.SetActive(false);
        }
        activeRobotImages.Clear();
    }

    // Method to adjust the size of the images based on the number of active images
    private void AdjustImageSizes(int numberOfRobots)
    {
        if (numberOfRobots <= 1)
        {
            qKeyImage.gameObject.SetActive(false);
            eKeyImage.gameObject.SetActive(false);
            return;
        }

        qKeyImage.gameObject.SetActive(true);
        eKeyImage.gameObject.SetActive(true);

        float totalWidth = layoutGroup.GetComponent<RectTransform>().rect.width;
        float imageWidth = (totalWidth - layoutGroup.padding.left - layoutGroup.padding.right
                            - layoutGroup.spacing * (numberOfRobots + 1)) / (numberOfRobots + 2); // +2 for Q and E images

        qKeyImage.GetComponent<LayoutElement>().preferredWidth = imageWidth;
        eKeyImage.GetComponent<LayoutElement>().preferredWidth = imageWidth;

        foreach (var image in robotImages)
        {
            image.GetComponent<LayoutElement>().preferredWidth = imageWidth;
        }
    }
}