using UnityEngine;

public class GrapplingGun : MonoBehaviour
{

    private LineRenderer lr;
    private Vector3 grapplePoint;
    public LayerMask whatIsGrappleable;
    public Transform gunTip, camera1, player;
    private float maxDistance = 30f;
    private SpringJoint joint;
    private Rigidbody rig;

    private float GrapplingColdDowntime=0.5f;//cd ±≥§
    public float CDtimer = 0.5f;
    private bool canGrap = true;

    void Awake()
    {
        rig = GetComponent<Rigidbody>();
        lr = GetComponent<LineRenderer>();
    }

    void Update()
    {
        if (!canGrap)
        {
            CDtimer -= Time.deltaTime;
            if (CDtimer < 0)
            {
                CDtimer = GrapplingColdDowntime;
                canGrap = true;
            }
        }
        if (Input.GetKeyDown(KeyCode.Mouse0) && canGrap)
        {
            StartGrapple();
        }
        else if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            StopGrapple();
        }
    }

    //Called after Update
    void LateUpdate()
    {
        DrawRope();
    }

    /// <summary>
    /// Call whenever we want to start a grapple
    /// </summary>
    void StartGrapple()
    {
        RaycastHit hit;
        if (Physics.Raycast(camera1.position, camera1.forward, out hit, maxDistance, whatIsGrappleable))
        {
            grapplePoint = hit.point;
            joint = player.gameObject.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = grapplePoint;

            float distanceFromPoint = Vector3.Distance(player.position, grapplePoint);

            //The distance grapple will try to keep from grapple point. 
            joint.maxDistance = Mathf.Min(distanceFromPoint * 0.5f, 4);
            joint.minDistance = distanceFromPoint * 0.1f;

            //Adjust these values to fit your game.
            joint.spring = 1f;
            joint.damper = 1f;
            joint.massScale = 4f;

            lr.positionCount = 2;
            currentGrapplePosition = gunTip.position;

            rig.velocity += Vector3.Normalize(hit.point - transform.position) * 5;
            canGrap = false;
            GetComponent<PlayerMove1>().state = PlayerMove1.PlayerState.Grap;
            GetComponent<PlayerMove1>().isGrounded = false;
        }
    }


    /// <summary>
    /// Call whenever we want to stop a grapple
    /// </summary>
    void StopGrapple()
    {

        lr.positionCount = 0;
        GetComponent<PlayerMove1>().state = PlayerMove1.PlayerState.Fly;
        Destroy(joint);
    }

    private Vector3 currentGrapplePosition;
    
    void DrawRope()
    {
        //If not grappling, don't draw rope
        if (!joint) return;

        currentGrapplePosition = Vector3.Lerp(currentGrapplePosition, grapplePoint, Time.deltaTime * 8f);
        
        lr.SetPosition(0, gunTip.position);
        lr.SetPosition(1, currentGrapplePosition);
    }

    public bool IsGrappling()
    {
        return joint != null;
    }

    public Vector3 GetGrapplePoint()
    {
        return grapplePoint;
    }
}
