using System.Collections;
using System.Collections.Generic;
using OFR.Psycho.Utils;
using UnityEngine;

namespace OFR.Psycho.Platforms {
  public class FloorBreak : MonoBehaviour {

    public void Break() {
      Destroy(transform.parent.gameObject);
    }
  }
}