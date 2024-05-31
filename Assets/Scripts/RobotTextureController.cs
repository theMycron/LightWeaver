using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotTextureController : MonoBehaviour
{
    
    public const float ROBOT_GREEN = 0.075f;
    public const float ROBOT_BLUE = 1.215f;
    public const float ROBOT_PURPLE = 1.385f;
    public const float ROBOT_RED = 0.67f;
    public const float ROBOT_GREY = -0.47f;

    Renderer[] renderers;

    private void Start()
    {
        renderers = GetComponentsInChildren<Renderer>();
        if (GetComponent<PlayerController>() == null)
        {
            SetRobotColor(ROBOT_GREY);
        }
    }

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
}
