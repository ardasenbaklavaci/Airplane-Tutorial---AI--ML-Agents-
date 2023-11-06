using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class AIArea : MonoBehaviour
{
    [Tooltip("The path the race will take")]
    public CinemachineSmoothPath checkpointsarray;

    [Tooltip("The prefab to use for checkpoints")]
    public GameObject checkpointModel;

    [Tooltip("Prefab to use for finish/start")]
    public GameObject finishModel;

    [Tooltip("If true enable training mode")]
    public bool training = true;

    public List<AIAgents> AgentsList { get; private set; }

    public List<GameObject> CheckpointsList { get; private set; }

    //Actions to performwhen the airplane wakes up
    private void Awake()
    {
        if (AgentsList == null) LoadAgents();
        if (CheckpointsList == null) LoadCheckpoints();
    }

    private void Start()
    {
        
    }
    /// <summary>
    /// Find Aircraft Agents in the Area
    ///
    /// </summary>
    private void LoadAgents()
    {
        AgentsList = transform.GetComponentsInChildren<AIAgents>().ToList();
        // debug:
        //Debug.Assert(AIAgents.Count > 0, "No AircraftAgents in List");
        Debug.Log("Found " + AgentsList.Count.ToString() + " agents");

        //Debug.LogError("Aircraft Agents Count: " + AIAgents.Count.ToString());
    }

    /// <summary>
    /// Creates the Checkpoints
    /// </summary>
    private void LoadCheckpoints()
    {
        //Create checkpoints along the path
        Debug.Assert(checkpointsarray != null, "Checkpoints list null !!!");
        CheckpointsList = new List<GameObject>();
        int checkpointscount = (int)checkpointsarray.MaxUnit(CinemachinePathBase.PositionUnits.PathUnits);
        for (int i = 0; i < checkpointscount; i++)
        {
            GameObject checkpointadd;
            if (i == checkpointscount - 1)
                checkpointadd = Instantiate<GameObject>(finishModel);
            else
                checkpointadd = Instantiate<GameObject>(checkpointModel);
            //Set position, parent and rotation
            checkpointadd.transform.SetParent(checkpointsarray.transform);
            checkpointadd.transform.localPosition = checkpointsarray.m_Waypoints[i].position;
            checkpointadd.transform.rotation = checkpointsarray.EvaluateOrientationAtUnit(i, CinemachinePathBase.PositionUnits.PathUnits);

            CheckpointsList.Add(checkpointadd);

            //Debug.Log("Checkpoint added");
        }

    }

    /// <summary>
    /// Resets the position of an agent using its current NextCheckpointIndex, unless randomize is true
    /// </summary>
    /// <param name="agent">The agent to reset</param>
    /// <param name="randomized">If true, will pick a new NextCheckpointIndex before reset</param>        
    public void ResetAgentPosition(AIAgents agent, bool randomized = true)
        {
            if (AgentsList == null) LoadAgents();
            if (randomized)
            {
                agent.NextCheckpointIndex = Random.Range(0, CheckpointsList.Count);

            }
            int prev = agent.NextCheckpointIndex - 1;
            if (prev < 0)
                prev = CheckpointsList.Count - 1;

            float startPosition = checkpointsarray.FromPathNativeUnits(prev, CinemachinePathBase.PositionUnits.PathUnits);

            Vector3 basePosition = checkpointsarray.EvaluatePosition(startPosition);
            Quaternion orientation = checkpointsarray.EvaluateOrientation(startPosition);
            //Calculate a horizontal offset so the agents are spread apart
            Vector3 positionOffset = Vector3.right * (AgentsList.IndexOf(agent) - AgentsList.Count / 2f) * Random.Range(9f,10f);
            agent.transform.position = basePosition + orientation * positionOffset;
            agent.transform.rotation = orientation;
        }

}