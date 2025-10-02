using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorObjetc : GameUnit
{
    public EColorType colorType;

    [SerializeField] private ColorData colorData;
    [SerializeField] private Renderer renderer;

    public void ChangeColor(EColorType newColorType)
    {
        colorType = newColorType;
        renderer.material = colorData.GetColorMat(colorType);
    }
}
