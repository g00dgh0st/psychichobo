using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OFR.Psycho.Controls {
  public class TelekinesisController : MonoBehaviour {

    [SerializeField]
    public AnimationCurve lowMovementCurve;
    [SerializeField]
    public AnimationCurve fullMovementCurve;

    public float maxVelocity = 15f;
    public float lowMovementMaxDistance = 1f;
    public float lowMovementMaxVelocity = 3f;
    public float fullMovementMaxDistance = 6f;
    public float fullMovementMaxVelocity = 20f;
    public float decelMinDistance = 1.3f;

    public float lastMouseDist;

  }
}