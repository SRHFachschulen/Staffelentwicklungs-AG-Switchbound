using UnityEngine;

public class WorldBasedMovingGround : WorldBasedActive
{
    [SerializeField] bool worldBased = true;
    [HideInInspector] public GameObject player;

    public override void OnWorldSwitched(WorldType newWorld)
    {
        if (!worldBased)
            return;
        if (player != null)
            player.transform.SetParent(null);
        base.OnWorldSwitched(newWorld);
    }
}
