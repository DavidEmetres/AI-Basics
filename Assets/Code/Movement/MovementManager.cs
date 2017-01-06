using UnityEngine;
using System.Collections;

public class MovementManager : MonoBehaviour {

    //GENERAL VARIABLES
    Vector2 target;
    Vector2 position;
    Vector2 velocity;
    Quaternion rotation;
    Vector2 desiredVelocity;

    [Header("General Variables")]
    public bool seek;
    public bool flee;
    public bool wander;
    public bool pursuit;
    public float maxVelocity;
    public float smoothMov;
    public float smoothRot;

    //SEEK VARIABLES
    bool seeking;

    [Header("Seek Variables")]
    public float minDistanceTarget;

    //FLEE VARIABLES
    bool fleeing;

    [Header("Flee Variables")]
    public float safeDistanceTarget;

    //ARRIVAL VARIABLES
    float arrivalForce;

    [Header("Arrival Variables")]
    public float slowDownRadius;

    //WANDER VARIABLES
    bool wandering;
    Vector2 circleCenter;
    Vector2 displacement;
    Vector2 wanderForce;
    float wanderTimer;

    [Header("Wander Variables")]
    public float circleWanderDistance;
    public float circleWanderRadius;
    public float maxWanderAngle;
    public float maxWanderTime;

    //PURSUIT VARIABLES
    [Header("Pursuit Variables")]
    public MovementManager movTarget;
    
	void Start () {
        position = new Vector2(transform.position.x, transform.position.z);
        target = position;
        velocity = Vector2.zero;
        fleeing = false;
        seeking = false;
        circleWanderDistance = maxVelocity / 2f;
        wanderTimer = maxWanderTime + 1;
	}
	
	void Update () {
        //EDITOR VELOCITY RAY
        Debug.DrawRay(transform.position, new Vector3(velocity.x, 0f, velocity.y) * maxVelocity, Color.green);
        Debug.DrawRay(transform.position, new Vector3(desiredVelocity.x, 0f, desiredVelocity.y) * maxVelocity, Color.red);

        //START RANDOM WANDERING
        if(wander)
        {
            wander = false;
            if (!wandering)
            {
                wandering = true;
                desiredVelocity = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
            }
            else
                wandering = false;
        }

        //ARRIVAL STEERING BEHAVIOUR CONTROLLER
        if (seeking)
        {
            arrivalForce = (Vector2.Distance(position, target) <= slowDownRadius) ? Vector2.Distance(position, target) : slowDownRadius;
            arrivalForce = arrivalForce / slowDownRadius;
        }
        if (fleeing || wandering || pursuit)
            arrivalForce = 1f;

        //UPDATE STERRING BEHAVIOURS
        if (seeking)
            Seek(target);

        if (fleeing)
            Flee(target);

        if (wandering)
            Wander();

        if (pursuit)
            Pursuit(movTarget.GetPosition(), movTarget.GetVelocity(), movTarget.maxVelocity);

        //UPDATE KINEMATIC CHARACTERISTICS
        velocity = Vector2.Lerp(velocity, desiredVelocity, smoothMov * Time.deltaTime);
        position += velocity * maxVelocity * arrivalForce * Time.deltaTime;
        if(velocity != Vector2.zero)
            rotation = Quaternion.LookRotation(new Vector3(velocity.x, 0f, velocity.y), transform.up);

        //SET TARGET WITH MOUSE LEFT CLICK
        if(Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if(Physics.Raycast(ray, out hit))
            {
                if(hit.transform.tag == "Terrain")
                {
                    Vector2 newPos = new Vector2(hit.point.x, hit.point.z);
                    Instantiate(Resources.Load("Prefabs/MovementMark"), hit.point, Quaternion.identity);

                    target = newPos;

                    seeking = (seek) ? true : false;
                    fleeing = (flee) ? true : false;
                }
            }
        }

        //UPDATE CHARACTER TRANSFORM POSITION
        transform.position = new Vector3(position.x, transform.position.y, position.y);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, smoothRot * Time.deltaTime);
	}

    void Seek(Vector2 target)
    {
        //SEEKING TARGET
        if (Vector2.Distance(position, target) > minDistanceTarget)
            desiredVelocity = (target - position).normalized;
        //TARGET REACHED
        else
        {
            desiredVelocity = Vector2.zero;
            seeking = false;
        }
    }

    void Flee(Vector2 target)
    {
        //FLEEING FROM TARGET
        if(Vector2.Distance(position, target) < safeDistanceTarget)
            desiredVelocity = (position - target).normalized;
        //SAFE DISTANCE REACHED
        else
        {
            desiredVelocity = Vector2.zero;
            fleeing = false;
        }
    }

    void Wander()
    {
        //RANDOM WANDER DIRECTION EVERY WANDER TIME
        if (wanderTimer >= maxWanderTime)
        {
            //GET CIRCLE CENTER
            circleCenter = velocity;
            circleCenter.Normalize();
            circleCenter.Scale(new Vector2(circleWanderDistance, circleWanderDistance));

            //GET RANDOM DISPLACEMENT
            displacement = circleCenter;
            displacement.Scale(new Vector2(circleWanderRadius, circleWanderRadius));
            displacement = Hacks.RotateVector2(displacement, Random.Range(-maxWanderAngle, maxWanderAngle));

            //ADD VECTOR FORCES
            wanderForce = circleCenter + displacement;
            desiredVelocity = wanderForce.normalized;

            //REINITIALIZE WANDER TIMER
            wanderTimer = 0f;
        }

        else
            wanderTimer += Time.deltaTime;

        Debug.DrawRay(transform.position, new Vector3(circleCenter.x, 0f, circleCenter.y), Color.blue);
        Debug.DrawRay(transform.position + new Vector3(circleCenter.x, 0f, circleCenter.y), new Vector3(displacement.x, 0f, displacement.y), Color.magenta);
    }

    void Pursuit(Vector2 targetPos, Vector2 targetVel, float targetMaxVel)
    {
        //COMPUTE GOOD PREDICTION VALUE
        float predictionPrecision = (Vector2.Distance(position, targetPos) / targetMaxVel) * 20f;
        Debug.Log(predictionPrecision);
        //GET PREDICTED POSITION
        Vector2 futurePos = targetPos + (targetVel * targetMaxVel * predictionPrecision * Time.deltaTime);

        desiredVelocity = (futurePos - position).normalized;
    }

    public Vector2 GetPosition()
    {
        return position;
    }

    public Vector2 GetVelocity()
    {
        return velocity;
    }

    void OnGUI()
    {
        string behaviourString = (seeking) ? "SEEKING" : "";
        behaviourString = (fleeing) ? "FLEEING" : behaviourString;
        behaviourString = (wandering) ? "WANDERING" : behaviourString;
        GUI.contentColor = Color.black;
        GUI.Label(new Rect(new Vector2(10, 10), new Vector2(100, 100)), behaviourString);
    }
}
