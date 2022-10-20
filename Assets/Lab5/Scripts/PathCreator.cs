using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathCreator : MonoBehaviour
{
     public Path Path { get; private set; }

     private void Awake()
     {
         CreatePath();
     }

     public void CreatePath()
    {
        Path = new Path(transform.position);
    }
}
