using UnityEngine;
// This component enables or disables its child GameObjects based on the current world type.
public class WorldBasedActive : WorldListener
{
    //Select the world type in which the child GameObjects should be active.
    [SerializeField] private WorldType activeInWorld = WorldType.Light;

    public override void OnWorldSwitched(WorldType newWorld)
    {
        foreach (Transform child in transform)
            child.gameObject.SetActive(newWorld == activeInWorld);
    }
}
