using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootScene : MonoBehaviour
{
    public int x, z;

    void Start()
    {
        transform.position = new Vector3(x, 0, z) * 40f;
    }

}
