using System.Collections;
using System.Collections.Generic;
using OFR.Psycho.Utils;
using UnityEngine;

public class FloorBreak : MonoBehaviour {

  void OnCollisionEnter(Collision other) {
    print("breaky");
    if (other.collider.tag == Tags.Player) {
      Destroy(transform.parent.gameObject);
    }
  }
}