using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorObjetc : GameUnit
{
    public EColorType colorType;

    [SerializeField] private ColorData colorData;
    [SerializeField] private Renderer rdColor;

    public void ChangeColor(EColorType newColorType)
    {
        colorType = newColorType;   
        rdColor.material = colorData.GetColorMat(colorType);
    }
}
