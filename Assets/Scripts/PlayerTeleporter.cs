using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTeleporter : MonoBehaviour
{
    [SerializeField] CaveGenerator caveGen;
    [SerializeField] GameObject player;

    void Start()
    {
        player.transform.position = new Vector2(caveGen.width/2, caveGen.height);
    }
}
