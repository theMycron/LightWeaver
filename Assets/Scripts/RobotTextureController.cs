
using UnityEngine;


public enum FaceColors
{
    BLACK,
    BLUE,
    RED,
    PURPLE,
    GREEN
}

public class RobotTextureController : MonoBehaviour
{

    [Header("Face Materials")]
    [SerializeField] GameObject head;
    [SerializeField] private Material blueFaceMaterial;
    [SerializeField] private Material redFaceMaterial;
    [SerializeField] private Material purpleFaceMaterial;
    [SerializeField] private Material blackFaceMaterial;
    [SerializeField] private Material greenFaceMaterial;

    // sets an offset on the texture atlas to apply different colors
    public const float ROBOT_GREEN = 0.075f;
    public const float ROBOT_BLUE = 1.215f;
    public const float ROBOT_PURPLE = 1.385f;
    public const float ROBOT_RED = 0.67f;
    public const float ROBOT_GREY = -0.47f;

    Renderer[] renderers;

    [HideInInspector]
    public float defaultColor;
    [HideInInspector]
    public FaceColors defaultFaceColor;

    private void Awake()
    {
        defaultColor = ROBOT_GREEN;
        defaultFaceColor = FaceColors.GREEN;
    }

    private void Start()
    {
        renderers = GetComponentsInChildren<Renderer>();

        if (GetComponent<PlayerController>() == null)
        {
            SetRobotColor(ROBOT_GREY);
            SetFaceColor(FaceColors.BLACK);
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

    public void SetFaceColor(FaceColors color)
    {

        var headMaterials = head.GetComponent<Renderer>().materials;
        for (int i = 0; i < headMaterials.Length; i++)
        {
            if (headMaterials[i].name.Contains("Face"))
            {
                switch (color)
                {
                    case FaceColors.BLACK:
                        headMaterials[i] = blackFaceMaterial;
                        break;
                    case FaceColors.BLUE:
                        headMaterials[i] = blueFaceMaterial;
                        break;
                    case FaceColors.GREEN:
                        headMaterials[i] = greenFaceMaterial;
                        break;
                    case FaceColors.RED:
                        headMaterials[i] = redFaceMaterial;
                        break;
                    case FaceColors.PURPLE:
                        headMaterials[i] = purpleFaceMaterial;
                        break;
                }
                Debug.Log(headMaterials[i]);
            }
        }
        head.GetComponent<Renderer>().materials = headMaterials;

    }

    public void SetDefaultColor()
    {
        SetRobotColor(defaultColor);
        SetFaceColor(defaultFaceColor);
    }
}
