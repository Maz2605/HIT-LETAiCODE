
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public GameObject shadowPrefab;
    public Transform respawnPoint;

    InputRecorder recorder;

    void Awake()
    {
        recorder = GetComponent<InputRecorder>();
    }

    public void Kill()
    {
        var shadow = Instantiate(shadowPrefab, respawnPoint.position, Quaternion.identity);
        var replay = shadow.GetComponent<ShadowReplayInput>();
        replay.LoadInputs(recorder.GetInputs());

        transform.position = respawnPoint.position;
        recorder.Clear();
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Trap"))
            Kill();
    }
}
