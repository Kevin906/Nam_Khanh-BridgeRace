using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class BridgeStep : MonoBehaviour
{
    public Transform placeholder;

    private bool built = false;

    private void Reset()
    {
        Collider c = GetComponent<Collider>();
        c.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (built) return;

        Player p = other.GetComponent<Player>();
        if (p == null) return;

        bool placed = p.PlaceTopBrickOnTransform(placeholder != null ? placeholder : transform, true);
        if (placed)
        {
            built = true;
            Collider c = GetComponent<Collider>();
            if (c != null) c.enabled = false;
        }
        else
        {
            p.PushBack(0.4f);
            p.SetCanMove(false);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Player p = other.GetComponent<Player>();
        if (p == null) return;

        p.SetCanMove(true);
    }
}
