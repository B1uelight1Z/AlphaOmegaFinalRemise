using UnityEngine;

public class TestMove : MonoBehaviour
{
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        rb.AddForce(Vector3.right * 500f, ForceMode.Force);
        Debug.Log($"vel={rb.linearVelocity} | pos={transform.position}");
    }
}