using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeekerLab : MonoBehaviour
{
    public Transform target;
    public float speed;
    private Rigidbody rb;

    float proximity = 10.0f;

    //[Range(0.0f, 1.0f)]
    //public float t = 0.0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Last week's work -- hard-coding steering behaviours directly in Update().
		//Vector3 targetDirection = (target.position - rb.position).normalized;
		//Vector3 currentVelocity = rb.velocity;
		//Vector3 desiredVelocity = targetDirection * speed - currentVelocity;
		//rb.AddForce(desiredVelocity);     // Seek
		//rb.AddForce(-desiredVelocity);    // Flee

        // This week's work -- combining modular steering behaviours using lerp!
		Vector3 steeringForce = Vector3.zero;
        float t = 1.0f - SteeringLab.Attenuate(target.position, rb.position, proximity);
        Vector3 a = -rb.velocity;
        Vector3 b = -SteeringLab.Seek(target.position, rb, speed);
		steeringForce = Vector3.Lerp(a, b, t);
		rb.AddForce(steeringForce);
    }
}
