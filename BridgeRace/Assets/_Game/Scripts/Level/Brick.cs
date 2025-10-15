using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brick : GameUnit
{
    [SerializeField] private Renderer brickRenderer;

    public EColorType colorType;

    [HideInInspector] public Vector3 originalPosition;
    [HideInInspector] public Quaternion originalRotation;
    [HideInInspector] public Transform originalParent;

    public void SetColor(Material mat, EColorType colorType)
    {
        if (brickRenderer != null && mat != null)
        {
            brickRenderer.material = mat;
        }
        this.colorType = colorType;
    }

    public void SaveOriginalState()
    {
        originalPosition = transform.position;
        originalRotation = transform.rotation;
        originalParent = transform.parent;
    }

    public void RestoreOriginalState()
    {
        gameObject.SetActive(true);
        transform.SetParent(originalParent, true);
        transform.position = originalPosition;
        transform.rotation = originalRotation;

        Collider c = GetComponent<Collider>();
        if (c != null) c.enabled = true;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = false;
    }

    private void OnEnable()
    {
        Collider c = GetComponent<Collider>();
        if (c != null) c.enabled = true;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = false;
    }

    private void OnDisable()
    {

    }
}
