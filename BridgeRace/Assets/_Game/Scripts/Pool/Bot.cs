using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bot : GameUnit
{
    [SerializeField] private Renderer rdBot;
    [SerializeField] private Renderer rbBotBrick;
    public EColorType colorType = EColorType.Default;

    public void SetColor(Material mat, EColorType type)
    {
        if (rbBotBrick != null && rdBot != null && mat != null)
        {
            rdBot.material = mat;
            rbBotBrick.material = mat;
            colorType = type;
        }
    }
}
