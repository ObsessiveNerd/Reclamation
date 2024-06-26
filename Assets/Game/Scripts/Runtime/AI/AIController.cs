using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Aoiti.Pathfinding; //import the pathfinding library 
using static UnityEngine.RuleTile.TilingRuleOutput;
using System.IO;
using UnityEngine.Analytics;
using static UnityEngine.GraphicsBuffer;

public class AIController : MonoBehaviour
{ 
    public GameObject Target;
    
    AIVision m_AIVision;
    AIAction m_Action;

    AttackType m_AttackType;
    MeleeArea m_MeleeArea;
    Equipment m_Equipment;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.AddComponent<MoveAction>();
        gameObject.AddComponent<AttackAction>();

        m_AIVision = GetComponentInChildren<AIVision>();
        m_AttackType = GetComponent<Equipment>().MainHand.GetAttackType();
        m_MeleeArea = GetComponentInChildren<MeleeArea>();
        m_Equipment = GetComponent<Equipment>();
    }

    // Update is called once per frame
    void Update()
    {
        Target = m_AIVision.GetTarget();
        if(Target == null && m_Action != null)
        {
            m_Action.Shutdown();
            m_Action = null;
        }

        if (Target == null)
            return;

        if (m_AttackType == AttackType.Melee && m_MeleeArea.ObjectsInRange.Count > 0)
        {
            var atk = GetComponent<AttackAction>();
            atk.Set(m_Equipment.MainHand);
            m_Action = atk;
        }
        else if (Vector2.Distance(transform.position, Target.transform.position) <= 1.0f)
        {
            //ranged
        }
        else
            m_Action = GetComponent<MoveAction>();

        m_MeleeArea.ManualUpdate(Camera.main.WorldToScreenPoint(Target.transform.position));
        m_Action.Invoke(Target);
    }
}

public abstract class AIAction : MonoBehaviour
{
    public virtual void Invoke(GameObject target) { }
    public virtual void Shutdown() { }
}

public class MoveAction : AIAction
{
    [Header("Navigator options")]
    [SerializeField] float gridSize = 0.5f; //increase patience or gridSize for larger maps
    
    Pathfinder<Vector2> pathfinder; //the pathfinder object that stores the methods and patience
    [Tooltip("The layers that the navigator can not pass through.")]
    [SerializeField] LayerMask obstacles;
    [Tooltip("Deactivate to make the navigator move along the grid only, except at the end when it reaches to the target point. This shortens the path but costs extra Physics2D.LineCast")] 
    [SerializeField] bool searchShortcut =false; 
    [Tooltip("Deactivate to make the navigator to stop at the nearest point on the grid.")]
    [SerializeField] bool snapToGrid =false; 
    Vector2 targetNode; //target in 2D space
    List <Vector2> path;
    List<Vector2> pathLeftToGo= new List<Vector2>();
    [SerializeField] bool drawDebugLines;

    IMovement m_Move;

    void Start() 
    {
        pathfinder = new Pathfinder<Vector2>(GetDistance,GetNeighbourNodes,1000); //increase patience or gridSize for larger maps
        m_Move = GetComponent<IMovement>();
    }
    public override void Invoke(GameObject target)
    {
        GetMoveCommand(target.transform.position);

        if (pathLeftToGo.Count > 0) //if the target is not yet reached
        {
            Vector3 dir =  ((Vector3)pathLeftToGo[0]-transform.position).normalized;
            m_Move.Move(dir.x, dir.y);
            if (((Vector2)transform.position - pathLeftToGo[0]).sqrMagnitude < 0.01f)
            {
                transform.position = pathLeftToGo[0];
                pathLeftToGo.RemoveAt(0);
            }
        }
    }

    public override void Shutdown()
    {
        m_Move.Move(0f, 0f);
    }

    void GetMoveCommand(Vector2 target)
    {
        Vector2 closestNode = GetClosestNode(transform.position);
        if (pathfinder.GenerateAstarPath(closestNode, GetClosestNode(target), out path)) //Generate path between two points on grid that are close to the transform position and the assigned target.
        {
            if (searchShortcut && path.Count>0)
                pathLeftToGo = ShortenPath(path);
            else
            {
                pathLeftToGo = new List<Vector2>(path);
                if (!snapToGrid) pathLeftToGo.Add(target);
            }
        }
    }


    /// <summary>
    /// Finds closest point on the grid
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    Vector2 GetClosestNode(Vector2 target) 
    {
        return new Vector2(Mathf.Round(target.x/gridSize)*gridSize, Mathf.Round(target.y / gridSize) * gridSize);
    }

    /// <summary>
    /// A distance approximation. 
    /// </summary>
    /// <param name="A"></param>
    /// <param name="B"></param>
    /// <returns></returns>
    float GetDistance(Vector2 A, Vector2 B) 
    {
        return (A - B).sqrMagnitude; //Uses square magnitude to lessen the CPU time.
    }

    /// <summary>
    /// Finds possible conenctions and the distances to those connections on the grid.
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    Dictionary<Vector2,float> GetNeighbourNodes(Vector2 pos) 
    {
        Dictionary<Vector2, float> neighbours = new Dictionary<Vector2, float>();
        for (int i=-1;i<2;i++)
        {
            for (int j=-1;j<2;j++)
            {
                if (i == 0 && j == 0) continue;

                Vector2 dir = new Vector2(i, j)*gridSize;
                if (!Physics2D.Linecast(pos,pos+dir, obstacles))
                {
                    neighbours.Add(GetClosestNode( pos + dir), dir.magnitude);
                }
            }

        }
        return neighbours;
    }

    
    List<Vector2> ShortenPath(List<Vector2> path)
    {
        List<Vector2> newPath = new List<Vector2>();
        
        for (int i=0;i<path.Count;i++)
        {
            newPath.Add(path[i]);
            for (int j=path.Count-1;j>i;j-- )
            {
                if (!Physics2D.Linecast(path[i],path[j], obstacles))
                {
                    
                    i = j;
                    break;
                }
            }
            newPath.Add(path[i]);
        }
        newPath.Add(path[path.Count - 1]);
        return newPath;
    }

    //if (drawDebugLines)
    //    {
    //        for (int i=0;i<pathLeftToGo.Count-1;i++) //visualize your path in the sceneview
    //        {
    //            Debug.DrawLine(pathLeftToGo[i], pathLeftToGo[i+1]);
    //        }
    //    }
}

public class AttackAction : AIAction
{
    SO_Weapon m_Attack;
    bool m_CanAttack = true;

    public void Set(SO_Weapon attack)
    {
        m_Attack = attack;
    }

    public override void Invoke(GameObject target)
    {
        if(m_CanAttack)
        {
            m_Attack.Attack(transform.gameObject, transform.GetComponentInChildren<MeleeArea>(),
                Camera.main.WorldToScreenPoint(target.transform.position));
            m_CanAttack = false;
            StartCoroutine(ResetAttackTimer());
        }
    }

    IEnumerator ResetAttackTimer()
    {
        yield return new WaitForSeconds(0.5f);
        m_CanAttack = true;
    }
}
