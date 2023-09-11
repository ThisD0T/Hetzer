using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scamper.Movement {
    public class MovePhysics
    {
        public Vector3 Accelerate(Vector3 currentVelocity, Vector3 wishdir, float wishSpeed, float accel) {
            float currentSpeed, addSpeed, accelSpeed;
            Vector3 result = Vector2.zero;

            // intentionally incorrect current speed calculation
            currentSpeed = Vector3.Dot(currentVelocity, wishdir);

            addSpeed = wishSpeed - currentSpeed;

            if (addSpeed <= 0) {
                return Vector3.zero;
            }

            // this is where you factor in the accelSpeed which changes your ability to air strafe
            accelSpeed = accel * Time.deltaTime * wishSpeed;

            if (accelSpeed > addSpeed) {
                accelSpeed = addSpeed;
            }

            // shove it into a variable
            for (int i = 0; i < 3; i++) {
                result[i] = accelSpeed * wishdir[i];
            }

            return result;
        }

        public Vector3 AirAccelerate(Vector3 currentVelocity, Vector3 wishdir, float wishSpeed, float airAccel) {
            float currentSpeed, addSpeed, accelSpeed;
            Vector3 result = Vector2.zero;

            float wishspd = wishSpeed;


            // intentionally incorrect current speed calculation
            currentSpeed = Vector3.Dot(currentVelocity, wishdir);

            addSpeed = wishspd - currentSpeed;

            if (addSpeed <= 0) {
                return Vector3.zero;
            }

            // this is where you factor in the accelSpeed which changes your ability to air strafe
            accelSpeed = airAccel * wishSpeed * Time.deltaTime;

            if (accelSpeed > addSpeed) {
                accelSpeed = addSpeed;
            }

            // shove it into a variable
            for (int i = 0; i < 3; i++) {
                result[i] = accelSpeed * wishdir[i];
            }

            return result;

        }
    }
}
