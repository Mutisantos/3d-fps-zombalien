using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    private Dictionary<string, int> KeyInventory = new Dictionary<string, int>();
    public List<Checkpoint> AllCheckpoints;
    public Checkpoint LastCheckpoint;
    public PlayerShooter PlayerReference;
    public static GameManager instance = null;

    void Awake()
    {
        MakeSingleton();
    }

    private void MakeSingleton()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void AddItemToInventory(string keyName, int amount)
    {
        KeyInventory.Add(keyName.ToUpper(), amount);
    }

    public bool HasItemOnInventory(string keyName)
    {
        return KeyInventory.ContainsKey(keyName.ToUpper());
    }


    public void RespawnPlayerToLastCheckpoint()
    {
        PlayerReference.Respawn();
    }
}
