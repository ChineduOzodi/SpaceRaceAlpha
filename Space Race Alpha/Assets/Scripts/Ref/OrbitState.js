#pragma strict
//================================//
//===   Orbit State datatype   ===//
//================================//
 
/*
 The OrbitState is the initial state of the orbiter at a particular point along the ellipse
 The state contains all of the information necessary to apply a force to get the orbiter moving along the ellipse
*/
 
class OrbitState extends Object {
    var position : Vector3; // local position relative to the object we're orbiting around
	var normal : Vector3;
	var tangent : Vector3;
	var velocity : Vector3;
	private var orbiter : Orbiter;
	private var ellipse : OrbitalEllipse;	
 
	//==== Instance Methods ====//
 
	// Constructor
	function OrbitState (angle : float, orbiter : Orbiter, ellipse : OrbitalEllipse) {
	    Update(angle, orbiter, ellipse);
	}
 
	    // Update the state of the orbiter when its position along the ellipse changes
	    // Note: Make sure the ellipse is up to date before updating the orbit state
	    function Update (orbiterAngle : float, orbiter : Orbiter, ellipse : OrbitalEllipse) {
	        this.orbiter = orbiter;
	        this.ellipse = ellipse;
	        this.normal = CalcNormal(orbiterAngle);
	        this.tangent = CalcTangent(normal);
	        this.position = ellipse.GetPosition(orbiterAngle, orbiter.orbitAround.position);
	        this.velocity = CalcVelocity(orbiter.orbitSpeed * orbiter.GravityConstant(), position, orbiter.orbitAround.position);
	    }
 
 
	        //==== Private Methods ====//
 
	        // Returns the normal on the ellipse at the given angle
	        // Assumes a vertical semi-major axis, and a rotation of 0 at the top of the ellipse, going clockwise
        private function CalcNormal (rotationAngle : float) : Vector3 {
            // Part 1: Find the normal for the orbiter at its starting angle
            // Rotate an upward vector by the given starting angle around the ellipse. Gives us the normal for a circle.
            var localNormal : Vector3 = Quaternion.AngleAxis(rotationAngle, Vector3.forward*-1) * Vector3.up;
            // Sqash the normal into the shape of the ellipse
            localNormal.x *= ellipse.semiMajorAxis/ellipse.semiMinorAxis;
 
            // Part 2: Find the global rotation of the ellipse
            var ellipseAngle : float = Vector3.Angle(Vector3.up, ellipse.difference);
            if (ellipse.difference.x < 0)
                ellipseAngle = 360-ellipseAngle; // Full 360 degrees, rather than doubling back after 180 degrees
 
            // Part 3: Rotate our normal to match the rotation of the ellipse
            var globalNormal : Vector3 = Quaternion.AngleAxis(ellipseAngle, Vector3.forward*-1) * localNormal;
            return globalNormal.normalized;
        }
 
        private function CalcTangent (normal : Vector3) : Vector3 {
            var angle : float = 90;
            var direction : int = orbiter.counterclockwise ? -1 : 1;
            var tangent = Quaternion.AngleAxis(angle*direction, Vector3.forward*-1) * normal;
            return tangent;
        }
 
        private function CalcVelocity (gravity : float, orbiterPos : Vector3, orbitAroundPos : Vector3) : Vector3 {
            // Vis Viva equation
            var speed : float = Mathf.Sqrt( gravity * (2/Vector3.Distance(orbiterPos, orbitAroundPos) - 1/ellipse.semiMajorAxis ) );
            var velocityVec : Vector3 = tangent * speed;
            return velocityVec;
        }	
        }