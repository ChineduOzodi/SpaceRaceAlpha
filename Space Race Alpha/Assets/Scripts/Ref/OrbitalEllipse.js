#pragma strict

//===================================//
//===  Elliptical orbit datatype  ===//
//===================================//
 
/*
  Calculates an ellipse to use as an orbital path
*/
 
class OrbitalEllipse extends Object {
 
    // "Starting" apsis is the position of the transform.position of the orbiter.
    // "Ending" apsis is the distance that we've defined in the inspector.
    // Each apsis defines the distance from the object we're orbiting to the orbiter
    var startingApsis : float;
	var endingApsis : float;
 
	var semiMajorAxis : float;
	var semiMinorAxis : float;
	var focalDistance : float;
	var difference : Vector3; // difference between the object we're orbiting and the orbiter
 
 
	//==== Instance Methods ====//
 
	// Constructor
	function OrbitalEllipse (orbitAroundPos : Vector3, orbiterPos : Vector3, endingApsis : float, circular : boolean) {
	    Update(orbitAroundPos, orbiterPos, endingApsis, circular);
	}
 
	    // Update ellipse when orbiter properties change
	    function Update (orbitAroundPos : Vector3, orbiterPos : Vector3, endingApsis : float, circular : boolean) {
	        this.difference = orbiterPos - orbitAroundPos;
	        this.startingApsis = difference.magnitude;
	        if (endingApsis == 0 || circular)
	            this.endingApsis = this.startingApsis;
	        else
	            this.endingApsis = endingApsis;
	        this.semiMajorAxis = CalcSemiMajorAxis(this.startingApsis, this.endingApsis);
	        this.focalDistance = CalcFocalDistance(this.semiMajorAxis, this.endingApsis);
	        this.semiMinorAxis = CalcSemiMinorAxis(this.semiMajorAxis, this.focalDistance);
	    }
 
	        // The global position
	        function GetPosition (degrees : float, orbitAroundPos : Vector3) : Vector3 {
	            // Use the difference between the orbiter and the object it's orbiting around to determine the direction
	            // that the ellipse is aimed
	            // Angle is given in degrees
	            var ellipseDirection : float = Vector3.Angle(Vector3.left, difference); // the direction the ellipse is rotated
	            if (difference.y < 0) {
	                ellipseDirection = 360-ellipseDirection; // Full 360 degrees, rather than doubling back after 180 degrees
	            }
 
	            var beta : float = ellipseDirection * Mathf.Deg2Rad;
	            var sinBeta : float = Mathf.Sin(beta);
	            var cosBeta : float = Mathf.Cos(beta);
 
	            var alpha = degrees * Mathf.Deg2Rad;
	            var sinalpha = Mathf.Sin(alpha);
	            var cosalpha = Mathf.Cos(alpha);
 
	            // Position the ellipse relative to the "orbit around" transform
	            var ellipseOffset : Vector3 = difference.normalized * (semiMajorAxis - endingApsis);
 
	            var finalPosition : Vector3 = new Vector3();
	            finalPosition.x = ellipseOffset.x + (semiMajorAxis * cosalpha * cosBeta - semiMinorAxis * sinalpha * sinBeta) * -1;
	            finalPosition.y = ellipseOffset.y + (semiMajorAxis * cosalpha * sinBeta + semiMinorAxis * sinalpha * cosBeta);
 
	            // Offset entire ellipse proportional to the position of the object we're orbiting around
	            finalPosition += orbitAroundPos;
 
	            return finalPosition;
	        }
 
 
	            //==== Private Methods ====//
 
            private function CalcSemiMajorAxis (startingApsis : float, endingApsis : float) : float {
                return (startingApsis + endingApsis) * 0.5;
            }
            private function CalcSemiMinorAxis (semiMajorAxis : float, focalDistance : float) : float {
                var distA : float = semiMajorAxis + focalDistance*0.5;
                var distB : float = semiMajorAxis - focalDistance*0.5;
                return Mathf.Sqrt( Mathf.Pow(distA+distB,2) - focalDistance*focalDistance ) * 0.5;
            }
                // private function CalcEccentricity (semiMajorAxis : float, focalDistance : float) : float {
                // 	return focalDistance / (semiMajorAxis * 2);
                // }
            private function CalcFocalDistance (semiMajorAxis : float, endingApsis : float) : float {
                return (semiMajorAxis - endingApsis) * 2;
            }			
            }