using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LaserColors
{
    red, blue, purple, white
}

public class Colors : MonoBehaviour
{
    public static Color LASER_RED { get { return HexToColor("#FF7E75"); } }
    public static Color LASER_BLUE { get { return HexToColor("#7083FF"); } }
    public static Color LASER_PURPLE { get { return HexToColor("#5876FF"); } }
    public static Color LASER_WHITE { get { return HexToColor("#5876FF"); } }

    public static Color RED { get { return HexToColor("#5876FF"); } }
    public static Color BLUE { get { return HexToColor("#5876FF"); } }
    public static Color PURPLE { get { return HexToColor("#5876FF"); } }
    public static Color WHITE { get { return HexToColor("#5876FF"); } }
    public static Color GREEN { get { return HexToColor("#5876FF"); } }

    public static Color HexToColor(string hex)
    {
        ColorUtility.TryParseHtmlString(hex, out Color color);
        return color;
    }
    public static Color? GetLaserColor(LaserColors color)
    {
        switch (color)
        {
            case LaserColors.red:
                return LASER_RED;
            case LaserColors.blue:
                return LASER_BLUE;
            case LaserColors.purple:
                return LASER_PURPLE;
            case LaserColors.white:
                return LASER_WHITE;
            default:
                return null;
        }
    }
}
