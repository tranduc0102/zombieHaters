using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicObstacleTransparency : MonoBehaviour
{
   public Stack<GameObject> obstructions = new Stack<GameObject>();
   struct In
   {
      public float hor;
      public float ver;
      public float pan;
      public float tilt;
   }

   private In input;
   private Rigidbody rb;
   private Vector3 newDir;
   private GameObject player;

   void Start(){
      player = GameObject.Find ("Player");
      rb = GetComponent<Rigidbody> ();
      newDir = Vector3.zero;
   }
   private void Update()
   {
      SetTransparentViewToPlayer();
   }
   void SetTransparentViewToPlayer()
   {
      RaycastHit[] hits;
      Vector3 camToPlayer = player.transform.position - transform.position;
      Debug.DrawRay (transform.position, camToPlayer * 20, Color.red, 0.1f);
      int layerMask = 1 << 0;
      hits = Physics.RaycastAll (transform.position, camToPlayer, 200f, layerMask);
      if (hits.Length > obstructions.Count)
      {
         for (int i = obstructions.Count;  i < hits.Length; i++)
         {
            GameObject o = hits[i].rigidbody.gameObject;
            ChangeObjectColor(ref o, 0.5f);
            obstructions.Push (o);
         }
      }else if (hits.Length < obstructions.Count)
      {
         while (obstructions.Count > hits.Length)
         {
            GameObject o = obstructions.Pop();
            ChangeObjectColor(ref o, 1f);
         }
      }
   }
   void ChangeObjectColor(ref GameObject o, float p)
   {
      Renderer renderer = o.GetComponent<Renderer>();
      Color c = renderer.material.color;
      renderer.material.color = new Color(c.r, c.g, c.b, p);
   }
}
