using System.Collections;
using System.Collections.Generic;
using OFR.Psycho.Controls;
using OFR.Psycho.Utils;
using UnityEngine;

namespace OFR.Psycho.Interactive {
  public class RotateableObject : InteractiveObject {
    private Rigidbody rigidbody;
    private TelekinesisController ctrl;

    new void Awake() {
      base.Awake();
      rigidbody = GetComponent<Rigidbody>();

      // TODO: move this to some global service?
      ctrl = GameObject.FindGameObjectWithTag(Tags.Entities.Player).GetComponent<TelekinesisController>();

    }

    protected override void Interact() {
      base.Interact();

    }

    protected override void EndInteract() {
      base.EndInteract();
      rigidbody.velocity = Vector3.zero;
      rigidbody.angularVelocity = Vector3.zero;
    }

    void FixedUpdate() {
      if (isInteracting) {

        // TODO: move this into a global input manager or something class
        Ray mouseRay = mainCam.ScreenPointToRay(Input.mousePosition);
        Plane pp = new Plane(-Vector3.forward, Vector3.zero);
        pp.Raycast(mouseRay, out float rayDist);
        Vector3 velocityVector = mouseRay.GetPoint(rayDist) - transform.position;

        rigidbody.AddForce(velocityVector);
      }
    }
  }
}