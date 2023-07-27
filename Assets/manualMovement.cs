using UnityEngine;

public class manualMovement : MonoBehaviour
{
    public Rigidbody rb;
    public float forwardForce = 10f;
    public float sidewaysForce = 5f;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // To get user input for key presses: Input.GetKey()
        if (Input.GetKey("d"))
        {
            rb.AddForce(sidewaysForce, 0, 0, ForceMode.VelocityChange);
        }
        if (Input.GetKey("a"))
        {
            rb.AddForce(-sidewaysForce, 0, 0, ForceMode.VelocityChange);
        }
        if (Input.GetKey("w"))
        {
            rb.AddForce(0, 0, forwardForce, ForceMode.VelocityChange);
        }
        if (Input.GetKey("s"))
        {
            rb.AddForce(0, 0, -forwardForce, ForceMode.VelocityChange);
        }
    }
}