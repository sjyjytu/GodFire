using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoints : MonoBehaviour
{
    public static Transform[] positions;

    private void Awake()
    {
        positions = new Transform[transform.childCount];
        for (int i = 0; i < positions.Length; i++)
        {
            positions[i] = transform.GetChild(i);
        }
    }
}
