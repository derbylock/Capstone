var simulatePhysics : boolean = true;
var updatePosition : boolean = true;
var physInterp = 0.1;
var netInterp = 0.2;
var ping : float;
var jitter : float;
var localPlayer : GameObject;       //The "Player" GameObject for which this game instance is authoritative. Used to determine if we should be calculating physics on the object this script is controlling, in case it could be colliding with this game instance's "player"
var isResponding = false;           //Updated by the script for diagnostic feedback of the status of this NetworkView
var netCode = " (No Connection)";   //Updated by the script for diagnostic feedback of the status of this NetworkView
private var m : int;
private var p : Vector3;
private var r : Quaternion;
private var states = new State[15];
private var stateCount : int;

class State extends System.Object {
    var p : Vector3;
    var r : Quaternion;
    var t : float;
    function State(p : Vector3, r : Quaternion, t : float) {
        this.p = p;
        this.r = r;
        this.t = t;
    }
}

function Start() {
    networkView.observed = this;
}

function FixedUpdate() {
    if(!updatePosition || !states[10]) return;
    
    simulatePhysics = (localPlayer && Vector3.Distance(localPlayer.rigidbody.position, rigidbody.position) < 30);
    jitter = Mathf.Lerp(jitter, Mathf.Abs(ping - (Network.time - states[0].t)), Time.deltaTime * .3);
    ping = Mathf.Lerp(ping, Network.time - states[0].t, Time.deltaTime * .3);
    
    rigidbody.isKinematic = !simulatePhysics;
    rigidbody.interpolation = (simulatePhysics ? RigidbodyInterpolation.Interpolate : RigidbodyInterpolation.None);
    
    //Interpolation
    var interpolationTime : float = Network.time - netInterp;
    if (states[0].t > interpolationTime) {                                              // Target playback time should be present in the buffer
        var i : int;
        for (i=0; i<stateCount; i++) {                                                  // Go through buffer and find correct state to play back
            if (states[i] && (states[i].t <= interpolationTime || i == stateCount-1)) {
                var rhs : State = states[Mathf.Max(i-1, 0)];                            // The state one slot newer than the best playback state
                var lhs : State = states[i];                                            // The best playback state (closest to .1 seconds old)
                var l : float = rhs.t - lhs.t;                                          // Use the time between the two slots to determine if interpolation is necessary
                var t : float = 0.0;                                                    // As the time difference gets closer to 100 ms, t gets closer to 1 - in which case rhs is used
                if (l > 0.0001) t = ((interpolationTime - lhs.t) / l);                  // if t=0 => lhs is used directly
                if(simulatePhysics) {
                    rigidbody.position = Vector3.Lerp(rigidbody.position, Vector3.Lerp(lhs.p, rhs.p, t), physInterp);
                    rigidbody.rotation = Quaternion.Slerp(rigidbody.rotation, Quaternion.Slerp(lhs.r, rhs.r, t), physInterp);
                    rigidbody.velocity = ((rhs.p - states[i + 1].p) / (rhs.t - states[i + 1].t));
                }
                else {
                    rigidbody.position = Vector3.Lerp(lhs.p, rhs.p, t);
                    rigidbody.rotation = Quaternion.Slerp(lhs.r, rhs.r, t);
                }
                isResponding = true;
                netCode = "";
                return;
            }
        }
    }
    
    //Extrapolation
    else  {
        var extrapolationLength : float = (interpolationTime - states[0].t);
        if (extrapolationLength < 1 && states[0] && states[1]) {
            if(!simulatePhysics) {
                rigidbody.position = states[0].p + (((states[0].p - states[1].p) / (states[0].t - states[1].t)) * extrapolationLength);
                rigidbody.rotation = states[0].r;
            }
            isResponding = true;
            if(extrapolationLength < .5) netCode = ">";
            else netCode = " (Delayed)";
        }
        else {
            netCode = " (Not Responding)";
            isResponding = false;
        }
    }
    if(simulatePhysics && states[0].t > states[2].t) rigidbody.velocity = ((states[0].p - states[2].p) / (states[0].t - states[2].t));
}

function OnSerializeNetworkView(stream : BitStream, info : NetworkMessageInfo) {
    //We are the server, and have to keep track of relaying messages between connected clients
    if(stream.isWriting) {
        if(stateCount == 0) return;
        p = states[0].p;
        r = states[0].r;
        m = (Network.time - states[0].t) * 1000;    //m is the number of milliseconds that transpire between the packet's original send time and the time it is resent from the server to all the other clients
        stream.Serialize(p);
        stream.Serialize(r);
        stream.Serialize(m);
    }
    
    //New packet recieved - add it to the states array for interpolation!
    else {
        stream.Serialize(p);
        stream.Serialize(r);
        stream.Serialize(m);
        var state : State = new State(p, r, info.timestamp - (m > 0 ? (parseFloat(m) / 1000) : 0));
        if(stateCount == 0) states[0] = state;
        else if(state.t > states[0].t) {
            for (k=states.Length-1;k>0;k--) states[k] = states[k-1];
            states[0] = state;
        }
        //else Debug.Log(gameObject.name + ": Out-of-order state received and ignored (" + ((states[0].t - state.t) * 1000) + ")" + states[0].t + "---" + state.t + "---" + m + "---" + states[0].p.x + "---" + state.p.x);
        stateCount = Mathf.Min(stateCount + 1, states.Length);
    }
}