
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public GameObject shadowPrefab;
    public Transform respawnPoint;

    PlayerStateMachine sm;
    ActionRecorderOptimized recorder;

    void Awake()
    {
        sm = GetComponent<PlayerStateMachine>();
        recorder = GetComponent<ActionRecorderOptimized>();
    }

    public void Kill()
    {
        sm.Change(PlayerState.Die);

        var shadow = Instantiate(shadowPrefab, transform.position, Quaternion.identity);
        var replay = shadow.GetComponent<ShadowReplayAdvanced>();
        replay.LoadFrames(recorder.GetFrames());

        transform.position = respawnPoint.position;
        sm.Change(PlayerState.Idle);

        recorder.Clear();
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Trap"))
            Kill();
    }
}
