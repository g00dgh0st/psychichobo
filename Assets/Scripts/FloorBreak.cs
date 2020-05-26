using System.Collections;
using System.Collections.Generic;
using OFR.Psycho.Utils;
using UnityEngine;

public class FloorBreak : MonoBehaviour {

  public void Break() {
    Destroy(transform.parent.gameObject);
  }
}