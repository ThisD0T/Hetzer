using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
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

        // essentially the exact same as ground accel, but you clip accelSpeed
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

            accelSpeed = Mathf.Min(accelSpeed, addSpeed);

            // shove it into a variable
            for (int i = 0; i < 3; i++) {
                result[i] = accelSpeed * wishdir[i];
            }

            return result;

        }

        public Vector3 NoClipAccel(Vector3 velocity, Vector3 wishdir, float wishSpeed) {
            float addSpeed, currentSpeed, accelSpeed;
            Vector3 result = Vector2.zero;

            currentSpeed = Vector3.Magnitude(velocity);
            addSpeed = currentSpeed - wishSpeed;

            accelSpeed = wishSpeed * Time.deltaTime;
            accelSpeed = Mathf.Min(accelSpeed, addSpeed);

            for (int i = 0; i < 3; i++) {
                result[i] = accelSpeed * wishdir[i];
            }
            return result;
        }

        // returns a force vector to be added to acceleration based on a few factors like distance and magnitude of blast
        public Vector3 BlastVector(Vector3 position, Vector3 blastPosition, float magnitude, float blastRadius, bool onGround) {
            Vector3 blast = position - blastPosition;
            float yIntercept = magnitude,
                  slope = (-yIntercept / 2) / blastRadius,
                  dist = Vector3.Distance(position, blastPosition), blastMag;// distance between self and blast

            // literally y = mx + b
            blastMag = (slope * dist) + yIntercept;
            blast = SetMag(blast, blastMag);

            if (!onGround) {
                blast *= 2;
            }

            return blast;
        }

        // returns a vector that opposes velocity
        // this may have to be changed to the opposite in the future becasue this way might make less sense as time goes on
        public Vector3 CalcFriction(Vector3 currentVelocity, float frictionCoefficient) {
            float friction, speed = currentVelocity.magnitude;
            Vector3 frictionVector = currentVelocity * -1;

            // so you don't get asymptote
            if (speed < 0.22) {
                return currentVelocity * -1;
            }

            friction = speed - (speed * frictionCoefficient);
            frictionVector = SetMag(frictionVector, friction);
            
            return frictionVector;
        }

        Vector3 SetMag(Vector3 vec, float mag) {
            return vec.normalized * mag;
        }
    }
}
