using System;
using UnityEngine;

public class CleanupBroom : MonoBehaviour
{
   [SerializeField] private CleanupTool cleanupTool;
   private const string LITTER_TAG = "Litter";

   private void OnTriggerEnter(Collider other)
   {
      if (other.CompareTag(LITTER_TAG))
      {
         cleanupTool.CollectTrash(other.gameObject);
      }
   }
}
