using System;
using System.IO;
using UnityEngine;
[DefaultExecutionOrder(-1)]
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
    void Start()
    {
        /*
        if (_loadDataOnStart)
            SandManager.Instance.SetSandData(LoadSandManagedData())
        */
    }
    void OnApplicationQuit()
    {
        /*
        if (_saveDataOnQuit)
            SaveSandManagerData(SandManager.Instance.GetSandData());
        */
    }
    string _sandManagerSaveDataName = "/SandManagerData.json";
    [SerializeField] bool _loadDataOnStart = false;
    [SerializeField] bool _saveDataOnQuit = false;
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
#if UNITY_EDITOR
    [ContextMenu("Delete Sand Save Data")]
    public void DeleteSandData()
    {
        if (File.Exists(_sandManagerSaveDataName))
        {
            File.Delete(_sandManagerSaveDataName);
        }
    }
    #endif
}
