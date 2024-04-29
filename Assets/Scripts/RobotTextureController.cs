using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotTextureController : MonoBehaviour
{
    
    public const float ROBOT_GREEN = 0f;
    public const float ROBOT_BLUE = 0.17f;
    public const float ROBOT_PURPLE = 0.37f;
    public const float ROBOT_RED = 0.63f;

    Renderer[] renderers;

    private void Start()
    {
        renderers = GetComponentsInChildren<Renderer>();
        SetRobotColor(ROBOT_RED);
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
