#pragma strict
@script RequireComponent(Rigidbody)
 
//==============================//
//===        Orbiter         ===//
//==============================//
 
/*
  Required component. Add Orbiter.js to the object that you would like to put into orbit.
 
  Dependencies:
    OrbitalEllipse.js - calculates the shape, orientation, and offset of an orbit
    OrbitState.js - calculates the initial state of the orbiter
*/
 
var orbitAround : Transform;
var orbitSpeed : float = 10.0; // In the original orbital equations this is gravity, not speed
var apsisDistance : float; // By default, this is the periapsis (closest point in its orbit)
var startingAngle : float = 0; // 0 = starting apsis, 90 = minor axis, 180 = ending apsis
var circularOrbit : boolean = false;
var counterclockwise : boolean = false;
 
private var gravityConstant : float = 100;
private var rb : Rigidbody;
private var trans : Transform;
private var ellipse : OrbitalEllipse;
private var orbitState : OrbitState;
 
// Accessor
function Ellipse () : OrbitalEllipse {
    return ellipse;
}
 
    function Transform() : Transform {
        return trans;
    }
        function GravityConstant () : float {
            return gravityConstant;
        }
 
 
            // Setup the orbit when the is added
            function Reset () {
                if (!orbitAround)
                    return;
                ellipse = new OrbitalEllipse(orbitAround.position, transform.position, apsisDistance, circularOrbit);
                apsisDistance = ellipse.endingApsis; // Default to a circular orbit by setting both apses to the same value
            }
            function OnApplicationQuit () {
                ellipse = new OrbitalEllipse(orbitAround.position, transform.position, apsisDistance, circularOrbit);
            }
 
            function OnDrawGizmosSelected () {
                if (!orbitAround)
                    return;
                // This is required for the OrbitRenderer. For some reason the ellipse var is always null
                // if it's set anywhere else, even including OnApplicationQuit;
                if (!ellipse)
                    ellipse = new OrbitalEllipse(orbitAround.position, transform.position, apsisDistance, circularOrbit);
                // Never allow 0 apsis. Start with a circular orbit.
                if (apsisDistance == 0) {
                    apsisDistance = ellipse.startingApsis;
                }
            }
 
 
            function Start () {
                // Cache transform
                trans = transform;	
                // Cache & set up rigidbody
                rb = GetComponent.<Rigidbody>();
                rb.drag = 0;
                rb.useGravity = false;
                rb.isKinematic = false;
 
                // Bail out if we don't have an object to orbit around
                if (!orbitAround) {
                    Debug.LogWarning("Satellite has no object to orbit around");
                    return;
                }
 
                // Update the ellipse with initial value
                if (!ellipse)
                    Reset();
                ellipse.Update(orbitAround.position, transform.position, apsisDistance, circularOrbit);
 
                // Calculate starting orbit state
                orbitState = new OrbitState(startingAngle, this, ellipse);
 
                // Position the orbiter
                trans.position = ellipse.GetPosition(startingAngle, orbitAround.position);
 
                // Add starting velocity
                rb.AddForce(orbitState.velocity, ForceMode.VelocityChange);
                StartCoroutine("Orbit");
            }
 
            // Coroutine to apply gravitational forces on each fixed update to keep the object in orbit
            function Orbit () {
                while (true) {
                    // Debug.DrawLine(orbitState.position - orbitState.tangent*4, orbitState.position + orbitState.tangent*4);
                    var difference = trans.position - orbitAround.position;
                    rb.AddForce(-difference.normalized * orbitSpeed * gravityConstant * Time.fixedDeltaTime / difference.sqrMagnitude, ForceMode.VelocityChange);
                    yield WaitForFixedUpdate();
                }
            }