using UnityEngine;

public static class Steering
{
      public static Vector3 Seek(Transform target, Vector3 targetPos, Rigidbody current, float speed)
      {
            Vector3 targetDirection = (targetPos - current.position).normalized;
            Vector3 currentVelocity = current.velocity;
            Vector3 desiredVelocity = targetDirection * speed - currentVelocity;
            return desiredVelocity;
      }

      public static Vector3 Flee(Transform target, Vector3 targetPos, Rigidbody current, float speed)
      {
            Vector3 targetDirection = (target.position - current.position).normalized;
            Vector3 currentVelocity = current.velocity;
            Vector3 desiredVelocity = targetDirection * speed - currentVelocity;
            return -desiredVelocity;
      }

      public static Vector3 Pursue(Transform target, Vector3 targetPos, Rigidbody current, float speed)
      {
            return Seek(target, targetPos + target.GetComponent<Rigidbody>().velocity, current, speed);
      }
      
      public static Vector3 Evade(Transform target, Vector3 targetPos, Rigidbody current, float speed)
      {
            return -Seek(target, targetPos + target.GetComponent<Rigidbody>().velocity, current, speed);
      }
      
      public static Vector3 Arrive(Transform target, Rigidbody current, float slowRadius, float speed)
      {
            Vector3 targetVelocity;

            float distance = (target.position - current.position).magnitude;

            if (distance < slowRadius)
            {
                  Vector3 targetDir = (target.position - current.position).normalized;

                  targetVelocity = targetDir * (speed * (distance / slowRadius));
            }
            else
            {
                  Vector3 targetDir = (target.position - current.position).normalized;

                  targetVelocity = targetDir * speed;
            }
            Vector3 currentVelocity = current.velocity;
            Vector3 steeringVector = targetVelocity - currentVelocity;

            return steeringVector;
      }
}
