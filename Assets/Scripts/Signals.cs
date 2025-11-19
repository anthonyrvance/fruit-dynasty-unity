using System;

public static class Signals
{
    public static event Action OnTilePressed; // after Action include <int> for data

    public static void TilePressed()
    {
        OnTilePressed?.Invoke();
    }
}
