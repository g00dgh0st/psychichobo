using System.Collections;
using System.Collections.Generic;
using OFR.Psycho.Controls;
using OFR.Psycho.Utils;
using UnityEditor;
using UnityEngine;

namespace OFR.Psycho.Interactive {

  public class TelekineticObject : InteractiveObject {
    private Rigidbody rigidbody;
    private TelekinesisController ctrl;

    private bool isFrozen = false;

    private new void Awake() {
      base.Awake();
      rigidbody = GetComponent<Rigidbody>();
      ctrl = GameObject.FindGameObjectWithTag(Tags.Entities.Player).GetComponent<TelekinesisController>();
    }

    private void OnCollisionEnter(Collision other) {
      // Doesnt trigger if you unfreeze while player in standing on it
      if (other.collider.tag == Tags.Entities.Player) {
        EndInteract();
      }
    }

    protected override void Interact() {
      base.Interact();
      // if (isFrozen) Unfreeze();

      // renderer.material = telekinesisMat;
      rigidbody.useGravity = false;
      rigidbody.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;
    }

    protected override void EndInteract() {
      base.EndInteract();
      rigidbody.useGravity = true;
      rigidbody.constraints = RigidbodyConstraints.FreezePositionZ;
    }

    // private void Freeze() {
    //   isFrozen = true;
    //   rigidbody.useGravity = false;
    //   rigidbody.constraints = RigidbodyConstraints.FreezeAll;
    // }

    // private void Unfreeze() {
    //   isFrozen = false;
    //   rigidbody.useGravity = true;
    //   rigidbody.constraints = RigidbodyConstraints.FreezePositionZ;
    // }

    void FixedUpdate() {
      if (isInteracting) {
        // if (Input.GetMouseButtonDown(1)) {
        //   EndDrag();
        //   Freeze();

        //   return;
        // }

        // TODO: move this into a global input manager or something class
        Ray mouseRay = mainCam.ScreenPointToRay(Input.mousePosition);
        Plane pp = new Plane(-Vector3.forward, Vector3.zero);
        pp.Raycast(mouseRay, out float rayDist);
        Vector3 velocityVector = mouseRay.GetPoint(rayDist) - transform.position;
        float velocityMagnitude;

        if (ctrl.lastMouseDist <= velocityVector.magnitude) {
          GameObject.Find("Core").GetComponent<UIManager>().SetText("");
          // Accelerating
          if (velocityVector.magnitude < ctrl.lowMovementMaxDistance) {
            // low move threshold
            float x = velocityVector.magnitude / ctrl.lowMovementMaxDistance;
            velocityMagnitude = ctrl.lowMovementMaxVelocity * ctrl.lowMovementCurve.Evaluate(x);
          } else {
            // full move threshold
            float x = (velocityVector.magnitude - ctrl.lowMovementMaxDistance) / (ctrl.fullMovementMaxDistance - ctrl.lowMovementMaxDistance);
            velocityMagnitude = Mathf.Lerp(ctrl.lowMovementMaxVelocity, ctrl.fullMovementMaxVelocity, ctrl.fullMovementCurve.Evaluate(x));
          }
        } else if (velocityVector.magnitude < ctrl.decelMinDistance) {
          // decelerating
          GameObject.Find("Core").GetComponent<UIManager>().SetText("decel");
          float x = velocityVector.magnitude / ctrl.decelMinDistance;
          velocityMagnitude = ctrl.lowMovementMaxVelocity * ctrl.lowMovementCurve.Evaluate(x);
        } else {
          GameObject.Find("Core").GetComponent<UIManager>().SetText("");
          velocityMagnitude = rigidbody.velocity.magnitude;
        }

        ctrl.lastMouseDist = velocityVector.magnitude;

        rigidbody.velocity = velocityVector.normalized * velocityMagnitude;

        // GameObject.Find("Core").GetComponent<UIManager>().SetText(velocityVector.magnitude < lowMovementMaxDistance ? "LOW" : velocityVector.magnitude + "     " + rigidbody.velocity.magnitude);
      }
    }
  }
}