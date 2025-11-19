using UnityEngine;

public static class Tile_Properties
{
    static Color TILE_COLOR_A = Color.red;
    static Color TILE_COLOR_B = Color.blue;
    static Color TILE_COLOR_C = Color.yellow;
    static Color TILE_COLOR_D = Color.green;
    static Color TILE_COLOR_E = new Color(1.0f, 0.64f, 0f); // orange
    static Color TILE_COLOR_F = new Color(0.5f, 0f, 0.5f); // purple
    static Color TILE_COLOR_G = Color.cyan;

    public static Color[] colors =
    {
        TILE_COLOR_A,
        TILE_COLOR_B,
        TILE_COLOR_C,
        TILE_COLOR_D,
        TILE_COLOR_E,
        TILE_COLOR_F,
        TILE_COLOR_G
    };

    static Texture2D orange = null;
}