using System;
using UnityEngine;
namespace UnityStandardAssets._2D
{
	public class LinearCameraFollower : MonoBehaviour
	{
		public Rigidbody2D target;
        private float velocityMatchDelay = 0;
        private float currentVel = 1;

        // Use this for initialization
        private void Start()
		{
			transform.parent = null;
            transform.position = target.transform.position;
        }


		private void Update()
		{
            //we use this system so when the ship boosts, it can temporarily "outrun" the camera to help create that feel of speed.
            Vector2 dir = target.transform.position - transform.position;

            velocityMatchDelay -= Time.deltaTime;

            if (velocityMatchDelay <= 0) {
                currentVel = 1.5f;
            }
			else currentVel = 0f;

            float vel = Math.Min(dir.magnitude, currentVel);

            dir = dir.normalized * vel;

            transform.position += (Vector3) dir;


        }

		public void SetVelocityMatchDelay(float delayTime) {
            velocityMatchDelay = delayTime;
        }
	}
}
