using System;
using UnityEngine;

public class CleanupBroom : MonoBehaviour
{
   private const string LITTER_TAG = "Litter";

   private void OnTriggerEnter(Collider other)
   {
      if (other.CompareTag(LITTER_TAG))
      {
         Destroy(other.gameObject);
      }
      else if (other.transform.parent.TryGetComponent(out Wastepool wastePool))
      {
         wastePool.ReduceWastePool();
      }
   }
}
