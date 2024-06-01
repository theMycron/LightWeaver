using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotTextureController : MonoBehaviour
{
    // sets an offset on the texture atlas to apply different colors
    public const float ROBOT_GREEN = 0.075f;
    public const float ROBOT_BLUE = 1.215f;
    public const float ROBOT_PURPLE = 1.385f;
    public const float ROBOT_RED = 0.67f;
    public const float ROBOT_GREY = -0.47f;

    Renderer[] renderers;

    [HideInInspector]
    public float defaultColor;

    private void Awake()
    {
        defaultColor = ROBOT_GREEN;
    }

    private void Start()
    {
        renderers = GetComponentsInChildren<Renderer>();
        if (GetComponent<PlayerController>() == null)
        {
            SetRobotColor(ROBOT_GREY);
        }
    }

    // sets an offset on the texture atlas to apply different colors
    public void SetRobotColor(float robotColor)
    {
        foreach (Renderer renderer in renderers)
        {
            foreach (Material mat in renderer.materials)
            {
                if (!mat.name.Contains("RobotLight"))
                    continue;
                mat.mainTextureOffset = new Vector2(robotColor, 0);
            }
        }
    }

    public void SetDefaultColor()
    {
        SetRobotColor(defaultColor);
    }
}
