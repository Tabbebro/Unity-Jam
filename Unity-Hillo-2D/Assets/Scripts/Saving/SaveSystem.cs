using System.IO;
using UnityEngine;

public class SaveSystem : MonoBehaviour
{
    #region Instance
    public static SaveSystem Instance { get; private set; }
    void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
        _sandManagerSaveDataName = Path.Combine(Application.persistentDataPath + _sandManagerSaveDataName);
    }
    #endregion
    string _sandManagerSaveDataName = "/SandManagerData.json";

    public void SaveSandManagerData(SandManagerData sandManagerData)
    {
        string json = JsonUtility.ToJson(sandManagerData, true);
        File.WriteAllText(_sandManagerSaveDataName, json);
        Debug.Log("Saved data");
    }
    public SandManagerData LoadSandManagedData()
    {
        if (!File.Exists(_sandManagerSaveDataName))
        {
            return new SandManagerData();
        }

        string json = File.ReadAllText(_sandManagerSaveDataName);
        SandManagerData data = JsonUtility.FromJson<SandManagerData>(json);

        return data;
    }
}
