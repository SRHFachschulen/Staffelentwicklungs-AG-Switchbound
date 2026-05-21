using UnityEngine;
using System.Threading.Tasks;

// Base class for any component that needs to react to world changes.
public abstract class WorldListener : MonoBehaviour
{
    // Subscribes to world change events and calls the abstract method when the world changes.
    protected virtual async void OnEnable()
    {
        // Wait until the WorldManager instance is available before subscribing to events.
        await WaitForManagerAsync();

        WorldManager.Instance.OnWorldChanged += OnWorldSwitched;
        OnWorldSwitched(WorldManager.Instance.CurrentWorld);
    }

    // Waits asynchronously until the WorldManager instance is available, yielding control each frame to avoid blocking.
    private async Task WaitForManagerAsync()
    {
        while (WorldManager.Instance == null)
            await Task.Yield();
    }

    protected virtual void OnDisable()
    {
        if (WorldManager.Instance != null)
            WorldManager.Instance.OnWorldChanged -= OnWorldSwitched;
    }

    // Abstract method that derived classes must implement to define their behavior when the world changes.
    public abstract void OnWorldSwitched(WorldType newWorld);
}
