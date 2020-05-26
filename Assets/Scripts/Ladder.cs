using System.Collections;
using System.Collections.Generic;
using OFR.Psycho.Utils;
using UnityEngine;

public class Ladder : MonoBehaviour {

  private void OnTriggerEnter(Collider other) {
    if (other.tag == Tags.Entities.Player) {
      other.GetComponent<PlayerController>().AttachLadder(gameObject.GetComponent<BoxCollider>());
    }
  }

  void OnTriggerExit(Collider other) {
    if (other.tag == Tags.Entities.Player) {
      other.GetComponent<PlayerController>().DetachLadder();
    }
  }
}