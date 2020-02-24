using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class MyGameManager : MonoBehaviourPunCallbacks
{
    public GameObject playerPrefab;

    public static MyGameManager gameManagerInstance;

    // Start is called before the first frame update
    void Start()
    {
        if (playerPrefab!=null)
        {
            if (PlayerFireController.playerInstance == null)
            {
                Debug.Log("Instantiate the player!");
                System.Random rd = new System.Random();
                GameObject player = PhotonNetwork.Instantiate(playerPrefab.name, SpawnPoints.positions[rd.Next(0, SpawnPoints.positions.Length-1)].position, Quaternion.identity, 0);
            }
            else
            {
                Debug.Log("Already have prefab.");
            }
        }
        else
        {
            Debug.Log("no player prefab.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
