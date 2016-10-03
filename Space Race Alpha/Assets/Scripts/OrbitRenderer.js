#pragma strict
@script RequireComponent(Orbiter)
 
//===============================//
//===     Orbit Renderer      ===//
//===============================//
 
/*
  Optional component. Display the Orbiter component's properties in the editor. Does nothing in-game.
*/
 
var orbitPointsColor : Color = Color(1,1,0,0.5); // Yellow
var orbitPointsSize : float = 0.5;
var ellipseResolution : float = 24;
//var renderAsLines : boolean = false;
 
var startPointColor : Color = Color(1,0,0,0.7); // Red
var startPointSize : float = 1.0;
 
private var orbiter : Orbiter;
private var ellipse : OrbitalEllipse;
 
function Awake () {
    // Remove the component in the compiled game. Likely not a noticeable optimization, just an experiment.
    if (!Application.isEditor)
        Destroy(this);
}
 
function Reset () {
    orbiter = GetComponent(Orbiter);
}
function OnApplicationQuit () {
    orbiter = GetComponent(Orbiter);
}
 
 
function OnDrawGizmosSelected () {
    if (!orbiter)
        orbiter = GetComponent(Orbiter);
 
    // Bail out if there is no object to orbit around
    if (!orbiter.orbitAround)
        return;
 
    // Recalculate the ellipse only when in the editor
    if (!Application.isPlaying) {
        if (!orbiter.Ellipse())
            return;
        orbiter.Ellipse().Update(orbiter.orbitAround.position, transform.position, orbiter.apsisDistance, orbiter.circularOrbit);
    }
 
    DrawEllipse();
    DrawStartingPosition();
}
 
function DrawEllipse () {
    for (var angle = 0; angle < 360; angle += 360 / ellipseResolution) {
        Gizmos.color = orbitPointsColor;
        Gizmos.DrawSphere(orbiter.Ellipse().GetPosition(angle, orbiter.orbitAround.position), orbitPointsSize);
    }
}
 
function DrawStartingPosition () {	
    Gizmos.color = startPointColor;
    Gizmos.DrawSphere(orbiter.Ellipse().GetPosition(orbiter.startingAngle, orbiter.orbitAround.position), startPointSize);
}