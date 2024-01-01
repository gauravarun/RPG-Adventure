using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RpgAdventure
{
  public class FixedUpdateFollow : MonoBehaviour
  {
    public Transform toFollow;
    void FixedUpdate()
    {
      if (toFollow == null) { return; }
      transform.position = toFollow.position;
      transform.rotation = toFollow.rotation;
    }

    public void SetFollowee(Transform followee)
    {
      toFollow = followee;
    }
  }
}

