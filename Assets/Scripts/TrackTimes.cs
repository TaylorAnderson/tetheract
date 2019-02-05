using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class TrackTimesWrapper {
    public float[] times;
    public TrackTimesWrapper(float[] times) {
        this.times = times;
    }
}
public static class TrackTimes {
    public static float[] times = new float[] { 0, 0, 0, 0 };

    public static void Save() {
        string path = Application.persistentDataPath + "/";
        File.WriteAllText(path + "times.json", JsonUtility.ToJson(new TrackTimesWrapper(TrackTimes.times), true));
    }
    public static void Load() {
        string path = Application.persistentDataPath + "/" + "times.json";
        try {
            if (File.Exists(path)) {
                TrackTimes.times = JsonUtility.FromJson<TrackTimesWrapper>(File.ReadAllText(path)).times;
            }
            
        }	
        catch (System.Exception ex) {
            Debug.Log(ex.Message);
        }
    }
}

