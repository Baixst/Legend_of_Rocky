using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public int sceneIndex;
    public float[] playerPosition;

    public SaveData(Player player, float playerPositionY, int sceneIndex)
    {
        playerPosition = new float[3];
        playerPosition[0] = player.transform.position.x;
        playerPosition[1] = playerPositionY;
        playerPosition[2] = player.transform.position.z;

        this.sceneIndex = sceneIndex;
    }
}
