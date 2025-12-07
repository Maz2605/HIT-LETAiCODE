using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class Level : MonoBehaviour
{
   public Transform spawnAtOrigin;
   public CinemachineVirtualCamera _cam;

   public void SetUpdateFollow(PlayerController player)
   {
      _cam.Follow = player.transform;
   }
}
