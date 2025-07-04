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
        AudioSource[] audios = GetComponents<AudioSource>();
        foreach (var col in audios)
        {
            DestroyImmediate(col);
        }
        foreach (var component in transform.GetComponents<LODGroup>()) DestroyImmediate(component);
        foreach (var component in transform.GetComponents<Rigidbody>()) DestroyImmediate(component);
    }
}