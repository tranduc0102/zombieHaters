using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveCollider : MonoBehaviour
{
    [ContextMenu("Remove Collider")]
    public void RemoveCollder()
    {
        Collider[] colliders = gameObject.GetComponentsInChildren<Collider>();
        foreach (var col in colliders)
        {
            DestroyImmediate(col);
        }
    }
}