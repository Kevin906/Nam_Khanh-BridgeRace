using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ColorData", menuName = "ScriptableObjects/ColorData", order = 1)]
public class ColorData : ScriptableObject
{
    [SerializeField] Material[] colorMats;

    public Material GetColorMat(EColorType colorType)
    {
        return colorMats[(int)colorType];
    }
}

public enum EColorType
{
    Default,
    Red,
    Blue,
    Green,
    Yellow,
    Purple,
    Orange
}