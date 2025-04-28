using UnityEngine;

namespace PlanetTweaks.Utils;

public static class ScreenUtils {
    private const int Width = 1920;
    private const int Height = 1080;
    public static Rect Fix(this Rect rect) {
        if(Screen.width != Width) {
            float xMultiply = (float) Screen.width / Width;
            rect.x *= xMultiply;
            rect.width *= xMultiply;
        }
        if(Screen.height != Height) {
            float yMultiply = (float) Screen.height / Height;
            rect.y *= yMultiply;
            rect.height *= yMultiply;
        }
        return rect;
    }
}