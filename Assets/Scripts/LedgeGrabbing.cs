using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LedgeGrabbing : MonoBehaviour
{
    [Header("References")]
    public PlayerMovement pm;
    public Transform orientation;
    public Transform cam;
    public Rigidbody rb;

    [Header("Ledge Grabbing")]
    public float moveToLedgeSpeed;
    public float maxLedgeGrabDistance;

    public float minTimeOnLedge;
    private float timeOnLedge;

    private float distanceToLedge;

    public bool holding;

    [Header("Ledge Detection")]
    public float ledgeDetectionLength;
    public float ledgeSphereCastRadius;
    public LayerMask whatIsLedge;

    private Transform lastLedge;
    private Transform currLedge;

    private RaycastHit ledgeHit;

    private void Update() 
    {
        LedgeDetection();
        SubStateMachine();
    }
    private void SubStateMachine()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");
        bool anyInputKeyPressed = horizontalInput != 0 || verticalInput !=0;

        if (holding)
        {
            FreezeRigidbodyOnLedge();

            timeOnLedge += Time.deltaTime;

            if(timeOnLedge > minTimeOnLedge && anyInputKeyPressed) ExitLedgeHold();
        }
    }

    private void LedgeDetection()
    {
        bool ledgeDetected = Physics.SphereCast(transform.position, ledgeSphereCastRadius, cam.forward, out ledgeHit, ledgeDetectionLength, whatIsLedge);
        if(!ledgeDetected){return;}
        float distanceToLedge = Vector3.Distance(transform.position, ledgeHit.transform.position);

        if(ledgeHit.transform == lastLedge) return;

        if (distanceToLedge < maxLedgeGrabDistance && !holding) EnterLedgeHold();

    }
    private void EnterLedgeHold()
    {
        holding = true;

        pm.unlimited = true;
        pm.restricted = true;

        currLedge = ledgeHit.transform;
        lastLedge = ledgeHit.transform;

        rb.useGravity = false;
        rb.velocity = Vector3.zero;
    }
    private void FreezeRigidbodyOnLedge()
    {
        rb.useGravity = false;
        Vector3 directionToLedge = currLedge.position - transform.position;

        if(distanceToLedge > 1f)
        {
            if(rb.velocity.magnitude < moveToLedgeSpeed)
            {
                rb.AddForce(directionToLedge.normalized * moveToLedgeSpeed * 1000f * Time.deltaTime);
            }
        }
        else
        {
            if(!pm.freeze) pm.freeze = true;
            if(pm.unlimited) pm.unlimited = false;
        }

        if (distanceToLedge > maxLedgeGrabDistance) ExitLedgeHold();
    }
    private void ExitLedgeHold()
    {
        holding = false;
        timeOnLedge = 0f;

        pm.restricted = false;
        pm.freeze = false;

        rb.useGravity = true;

        StopAllCoroutines();
        Invoke(nameof(ResetLastLedge),1f);
    }

    private void ResetLastLedge()
    {
        lastLedge = null;
    }
}
