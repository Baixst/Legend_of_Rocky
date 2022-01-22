using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveSettings
{
    public float masterVol;
    public float musicVol;
    public float soundVol;

    public SaveSettings(float masterVol, float musicVol, float soundVol)
    {
        this.masterVol = masterVol;
        this.musicVol = musicVol;
        this.soundVol = soundVol;
    }
}
