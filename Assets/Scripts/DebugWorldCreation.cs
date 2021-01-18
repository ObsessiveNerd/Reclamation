using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class DebugWorldCreation : MonoBehaviour
{
    public GameObject TilePrefab;
    World m_World;
    public bool StartNew;

    // Start is called before the first frame update
    void Start()
    {
        IEntity player = EntityFactory.CreateEntity("Dwarf");
        IEntity goblin = EntityFactory.CreateEntity("Goblin");
        
        GameObject world = new GameObject("World");
        Actor worldActor = new Actor("World");
        m_World = new World(worldActor, TilePrefab);
        worldActor.AddComponent(m_World);
        worldActor.CleanupComponents();
        worldActor.FireEvent(worldActor, new GameEvent(GameEventId.StartWorld));

        if (StartNew)
        {
            m_World.ConvertToPlayableEntity(player);
            Spawner.Spawn(player, 3, 3);
            Spawner.Spawn(goblin, 10, 12);
        }
        else
            SaveSystem.Load("Assets/TestSave.sv");
    }

    private void Update()
    {
        m_World.ProgressTime();
    }
}
