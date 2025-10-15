using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
public class Player : MonoBehaviour
{
    [SerializeField] private GameObject playerBrickPrefabs;
    [SerializeField] private Rigidbody rbPlayer;
    [SerializeField] private Transform tfBrickStack;
    [SerializeField] private FixedJoystick fJoyStick;
    [SerializeField] private MeshRenderer mrPlayer;
    [SerializeField] private MeshRenderer mrPlayerBrick;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float brickHeight = 1f;
    [SerializeField] private float pickupMoveDuration = 0.15f;
    public EColorType colorType;
    public bool canMove = true;
    private List<Brick> collectedBricks = new List<Brick>();

    void Update()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        if (!canMove)
        {
            rbPlayer.velocity = new Vector3(0f, rbPlayer.velocity.y, 0f);
            return;
        }

        rbPlayer.velocity = new Vector3(fJoyStick.Horizontal * speed, rbPlayer.velocity.y, fJoyStick.Vertical * speed);

        if (fJoyStick.Horizontal != 0 || fJoyStick.Vertical != 0)
        {
            transform.rotation = Quaternion.LookRotation(new Vector3(fJoyStick.Horizontal, 0, fJoyStick.Vertical));
        }
    }
    public void SetCanMove(bool v)
    {
        canMove = v;
        if (!v)
        {
            rbPlayer.velocity = Vector3.zero;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Addbrick(other);
    }

    private void Addbrick(Collider col)
    {
        Brick brick = col.GetComponent<Brick>();
        if (brick == null)
        {
            return;
        }
        if (brick.colorType != this.colorType)
        {
            return;
        }
        if (collectedBricks.Contains(brick))
        {
            return;
        }

        Collider brickCollider = brick.GetComponent<Collider>();
        if (brickCollider != null)
        {
            brickCollider.enabled = false;
        }

        Rigidbody brickRb = brick.GetComponent<Rigidbody>();
        if (brickRb != null)
        {
            brickRb.velocity = Vector3.zero;
            brickRb.angularVelocity = Vector3.zero;
            brickRb.isKinematic = true;
        }

        int stackIndex = collectedBricks.Count;
        Vector3 worldTarget = tfBrickStack.position + tfBrickStack.up * (stackIndex * brickHeight);
        Quaternion targetRot = tfBrickStack.rotation;
        StartCoroutine(StackBrick(brick.transform, worldTarget, targetRot, stackIndex));
        collectedBricks.Add(brick);
    }

    private IEnumerator StackBrick(Transform brickTf, Vector3 worldTarget, Quaternion targetRot, int stackIndex)
    {
        Vector3 startPos = brickTf.position;
        Quaternion startRot = brickTf.rotation;
        float t = 0f;

        while (t < pickupMoveDuration)
        {
            t += Time.deltaTime;
            float k = Mathf.Clamp01(t / pickupMoveDuration);
            brickTf.position = Vector3.Lerp(startPos, worldTarget, k);
            brickTf.rotation = Quaternion.Slerp(startRot, targetRot, k);
            yield return null;
        }

        brickTf.position = worldTarget;
        brickTf.rotation = targetRot;

        brickTf.SetParent(tfBrickStack, true);
        brickTf.localScale = Vector3.one;
        brickTf.localPosition = new Vector3(0f, stackIndex * brickHeight, 0f);
        brickTf.localRotation = Quaternion.identity;

        Rigidbody rb = brickTf.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;

        Collider c = brickTf.GetComponent<Collider>();
        if (c != null) c.enabled = false;
    }



    public int GetStackCount()
    {
        return collectedBricks.Count;
    }

    public void SetColor(Material mat, EColorType type)
    {
        colorType = type;
        if (mrPlayer != null && mrPlayerBrick != null && mat != null)
        {
            mrPlayer.material = mat;
            mrPlayerBrick.material = mat;
            colorType = type;
        }
    }
    public bool PlaceBrickOnTile(Vector3 worldPos, Quaternion worldRot)
    {
        if (collectedBricks == null || collectedBricks.Count == 0) return false;

        int lastIndex = collectedBricks.Count - 1;
        Brick topBrick = collectedBricks[lastIndex];
        collectedBricks.RemoveAt(lastIndex);

        Transform brickTf = topBrick.transform;

        brickTf.SetParent(null, true);

        brickTf.localScale = Vector3.one;

        brickTf.position = worldPos;
        brickTf.rotation = worldRot;

        Rigidbody rb = brickTf.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        Collider col = brickTf.GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = true;
        }
        return true;
    }
    public bool PlaceTopBrickOnTransform(Transform target, bool usePlaceholderVisual = true)
    {
        if (collectedBricks == null || collectedBricks.Count == 0) return false;

        int lastIndex = collectedBricks.Count - 1;
        Brick topBrick = collectedBricks[lastIndex];
        collectedBricks.RemoveAt(lastIndex);

        Transform brickTf = topBrick.transform;

        if (usePlaceholderVisual)
        {
            Renderer placeholderRenderer = target.GetComponentInChildren<Renderer>(true);
            Renderer brickRenderer = brickTf.GetComponentInChildren<Renderer>(true);

            if (placeholderRenderer != null && brickRenderer != null)
            {
                placeholderRenderer.enabled = true;
                placeholderRenderer.material = brickRenderer.material;

                brickTf.gameObject.SetActive(false);

                return true;
            }
        }

        brickTf.SetParent(target, true);
        brickTf.localScale = Vector3.one;

        brickTf.position = target.position;
        brickTf.rotation = target.rotation;

        Rigidbody rb = brickTf.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        Collider c = brickTf.GetComponent<Collider>();
        if (c != null) c.enabled = true;

        return true;
    }

    public void PushBack(float distance)
    {
        Vector3 back = -transform.forward * distance;

        transform.position += back;
        if (rbPlayer != null)
        {
            rbPlayer.velocity = Vector3.zero;
        }
    }
}

