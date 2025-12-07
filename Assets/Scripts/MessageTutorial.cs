using System.Collections;
using DG.Tweening;
using UnityEngine;
using TMPro;
public class MessageTutorial : MonoBehaviour
{
    [SerializeField] private float speedRunText;
    [SerializeField] private float timeDelayNextMessage;
    public int indexMessage;
    public string[] messages = new []
    {
        "Oh no. I’m just lost the way to go home.",
        "I'm going where?",
        "What should I do?",
        "Would you be able to assist me in finding the path, traveler?",
        "Press button A to make a left turn",
        "Press button D to make a right turn.",
        "You can press button W/Space to jump."
    };

    [SerializeField] private TextMeshProUGUI txtMessage;

    public string GetStringMessage(int index)
    {
        return messages[index];
    }

    private bool nextTut = true;
    private void Update()
    {
        /*
        if(GameController.Instance.State != GameController.StateGame.ShowTutorial) return;
        */
        if (Input.GetMouseButtonDown(0) && nextTut)
        {
            nextTut = false;
            DOVirtual.DelayedCall(timeDelayNextMessage, delegate
            {
                ShowMessage();
                nextTut = true;
            });
        }
    }
    public void ShowMessage()
    {
        
    }
    IEnumerator ProcessingWriteText(float timer)
    {
        string s = "";
        int id = 1;
        while (s.Length < messages[indexMessage].Length)
        {
            s = messages[indexMessage].Substring(0, id);
            id += 1;
            txtMessage.text = s;
            yield return new WaitForSeconds(timer);
        }
    }
}