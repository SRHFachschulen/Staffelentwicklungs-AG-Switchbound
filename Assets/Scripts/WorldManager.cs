using System;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    public static WorldManager Instance { get; private set; }
    // Event that is invoked whenever the world type changes, allowing other components to react accordingly.
    public event Action<WorldType> OnWorldChanged;
    // Stores the current world type and invokes the OnWorldChanged event when it changes.
    [SerializeField] private WorldType currentWorld = WorldType.Light;
    // Provides a public property to get or set the current world type, ensuring that changes trigger the appropriate events.
    public WorldType CurrentWorld
    {
        get => currentWorld;
        set
        {
            if (currentWorld == value) return;
            currentWorld = value;
            OnWorldChanged?.Invoke(currentWorld);
        }
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    // Method to switch to the next world type in a cyclic manner, allowing for easy toggling between different world states.
    public void SwitchWorld()
    {
        var values = (WorldType[])Enum.GetValues(typeof(WorldType));
        int next = ((int)CurrentWorld + 1) % values.Length;
        CurrentWorld = values[next];
    }
}
// Enum representing the different world types, which can be expanded in the future to include more types as needed.
public enum WorldType
{
    Light,
    Dark
}