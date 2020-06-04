using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using OFR.Psycho.Utils;
using UnityEngine;

namespace OFR.Psycho.Cams {
  public class CameraZone : MonoBehaviour {
    [SerializeField]
    private CinemachineVirtualCamera vCam;

    /// <summary>
    /// OnTriggerEnter is called when the Collider other enters the trigger.
    /// </summary>
    /// <param name="other">The other Collider involved in this collision.</param>
    void OnTriggerEnter(Collider other) {
      if (other.tag == Tags.Entities.Player) {
        vCam.m_Priority = 20;
      }
    }

    /// <summary>
    /// OnTriggerExit is called when the Collider other has stopped touching the trigger.
    /// </summary>
    /// <param name="other">The other Collider involved in this collision.</param>
    void OnTriggerExit(Collider other) {
      if (other.tag == Tags.Entities.Player) {
        vCam.m_Priority = 0;
      }
    }
  }
}