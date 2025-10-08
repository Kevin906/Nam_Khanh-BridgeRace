using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bot : GameUnit
{
    [SerializeField] private Renderer rdBot;
    public EColorType colorType = EColorType.Default;

    public void SetColor(Material mat, EColorType type)
    {
        if (rdBot != null && mat != null)
        {
            rdBot.material = mat;
            colorType = type;
        }
    }
}
