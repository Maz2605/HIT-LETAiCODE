using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(PlayerController))]
public class InputRecorder : MonoBehaviour
{
    [Header("Recording")]
    public float sampleInterval = 0.016f; // ~60Hz
    private float sampleTimer = 0f;

    private List<PlayerInputData> recorded = new List<PlayerInputData>();
    public Vector3 OriginPos { get; private set; }

    private bool isRecording = false;

    PlayerController controller;

    [Header("Spawn")]
    public GameObject shadowPrefab; // assign prefab that has PlayerController + ShadowReplayInput
    public bool spawnAtOrigin = true; // spawn at originPos or at player's current pos

    void Awake()
    {
        controller = GetComponent<PlayerController>();
        OriginPos = transform.position;
        if (controller == null)
            Debug.LogError("InputRecorder requires a PlayerController on the same GameObject.");
    }

    void Update()
    {
        // Start / stop recording with J
        if (Input.GetKeyDown(KeyCode.J))
        {
            if (!isRecording)
                StartRecord();
            else
                StopAndSpawn();
        }


        if (!isRecording) return;

        sampleTimer += Time.deltaTime;
        if (sampleTimer >= sampleInterval)
        {
            sampleTimer -= sampleInterval;
            RecordSample();
        }
    }

    void StartRecord()
    {
        if (CloneManager.Instance == null)
        {
            Debug.LogWarning("No CloneManager in scene — cannot record/spawn clones properly.");
            return;
        }

        if (!CloneManager.Instance.CanSpawn())
        {
            Debug.Log("Max clones reached - cannot start recording.");
            return;
        }

        isRecording = true;
        recorded.Clear();
        sampleTimer = 0f;
        OriginPos = transform.position;
        Debug.Log("InputRecorder: Start Recording. originPos = " + OriginPos);
    }

    public void StopAndSpawn()
    {
        if (!isRecording) return;

        isRecording = false;
        Debug.Log("InputRecorder: Stop Recording. Recorded frames = " + recorded.Count);

        SpawnClone();
    }

    void RecordSample()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        bool run = Input.GetKey(KeyCode.LeftShift);
        bool jump = Input.GetButtonDown("Jump");
        bool dash = Input.GetKeyDown(KeyCode.LeftControl);

        PlayerInputData data = new PlayerInputData(h, v, run, jump, dash);
        recorded.Add(data);
    }

    void SpawnClone()
    {
        if (CloneManager.Instance == null)
        {
            Debug.LogWarning("No CloneManager in scene - cannot spawn clone.");
            return;
        }

        if (!CloneManager.Instance.CanSpawn())
        {
            Debug.Log("Cannot spawn clone - limit reached.");
            return;
        }

        if (shadowPrefab == null)
        {
            Debug.LogError("shadowPrefab is not assigned on InputRecorder.");
            return;
        }

        Vector3 spawnPos = spawnAtOrigin ? OriginPos : transform.position;

        GameObject shadow = CloneManager.Instance.SpawnClone(GameManager.Instance.CurrentPosLevel);

        // Ensure component exists
        ShadowReplayInput replay = shadow.GetComponent<ShadowReplayInput>();
        PlayerController shadowPc = shadow.GetComponent<PlayerController>();
        if (replay == null || shadowPc == null)
        {
            Debug.LogError("shadowPrefab must have ShadowReplayInput and PlayerController components.");
            Destroy(shadow);
            return;
        }

        shadowPc.IsClone = true;

        replay.LoadInputs(recorded);


        Debug.Log("InputRecorder: Spawned clone at " + spawnPos + " with " + recorded.Count + " frames.");
    }

    public List<PlayerInputData> GetInputs() => recorded;

    public void Clear()
    {
        recorded.Clear();
        isRecording = false;
        sampleTimer = 0f;
    }
    public void HardReset()
    {
        recorded.Clear();
        isRecording = false;
        sampleTimer = 0f;
        OriginPos = Vector3.zero;
    }

}
