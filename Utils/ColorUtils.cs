using System.Drawing;
using System.Numerics;
namespace Utils;

public static class ColorUtils
{
    public static Vector4 ToVector(this Color color) => 
        new(color.R / 255f, color.G / 255f, color.B / 255f, color.A / 255f);
    
    public static Vector3 SrgbToLinear(this in Vector3 color) => color.Pow(2.2f);
    public static Vector4 SrgbToLinear(this in Vector4 color) => color.Pow(2.2f);
    
    public static Vector3 LinearToSrgb(this in Vector3 color) => color.Pow(1.0f / 2.2f);
    public static Vector4 LinearToSrgb(this in Vector4 color) => color.Pow(1.0f / 2.2f);

    public static Vector4 Red = Color.Red.ToVector();
    public static Vector4 Green = Color.Green.ToVector();
    public static Vector4 Blue = Color.Blue.ToVector();
    public static Vector4 White = Color.White.ToVector();
    public static Vector4 Black = Color.Black.ToVector();
    public static Vector4 Yellow = Color.Yellow.ToVector();
    public static Vector4 Cyan = Color.Cyan.ToVector();
    public static Vector4 Magenta = Color.Magenta.ToVector();
    public static Vector4 Gray = Color.Gray.ToVector();
    public static Vector4 DarkGray = Color.DarkGray.ToVector();
    public static Vector4 LightGray = Color.LightGray.ToVector();
    public static Vector4 Orange = Color.Orange.ToVector();
    public static Vector4 Pink = Color.Pink.ToVector();
    public static Vector4 Violet = Color.Violet.ToVector();
    public static Vector4 Purple = Color.Purple.ToVector();
    public static Vector4 SlateGray = Color.SlateGray.ToVector();
}
