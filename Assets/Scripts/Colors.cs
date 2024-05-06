using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}
