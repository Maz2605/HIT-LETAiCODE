using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

namespace _Scripts.Extension
{
    public class ClosePanel : MonoBehaviour
    {
        [SerializeField] List<UIAppear> uIAppears;
        [SerializeField] UnityEvent onOpenEvt;
        [SerializeField] UnityEvent onStartCloseEvt;
        [SerializeField] UnityEvent onCloseEvt;


        void Reset()
        {
            uIAppears = new List<UIAppear>(transform.GetComponentsInChildren<UIAppear>());
        }

        public void ActiveAll()
        {
            gameObject.SetActive(true);
            for (int i = 0; i < uIAppears.Count; i++)
            {
                uIAppears[i].gameObject.SetActive(true);
            }
        }

        void OnEnable()
        {
            if (onOpenEvt != null)
                onOpenEvt.Invoke();
        }

        public void UpdateUIAppear(UIAppear ui, bool add = true)
        {
            if (add)
            {
                if (!uIAppears.Contains(ui))
                    uIAppears.Add(ui);
            }
            else
            {
                if (uIAppears.Contains(ui))
                    uIAppears.Remove(ui);
            }
        }

        public void _close(bool ignoreMainEvt = false, bool activeGameObject = false) //  1/2: = ignore open/close
        {
            if (gameObject.activeInHierarchy)
                StartCoroutine(closing(ignoreMainEvt, activeGameObject));
        }

        public void Close(UnityAction finishCallback, bool ignoreMainEvt = false, bool activeGameObject = false, float closeDelay = 0)
        {
            if (gameObject.activeInHierarchy)
                StartCoroutine(closing(ignoreMainEvt, activeGameObject, finishCallback, closeDelay));
            else if (finishCallback != null)
                finishCallback();
        }

        IEnumerator closing(bool ignoreEvt, bool activeGameObject = false, UnityAction customFinishCallback = null, float closeDelay = 0)
        {
            float t = closeDelay;
            while (t > 0)
            {
                t -= Time.deltaTime;
                yield return null;
            }

            if (!ignoreEvt && onStartCloseEvt != null)
                onStartCloseEvt.Invoke();

            t = 0;
            for (int i = 0; i < uIAppears.Count; i++)
            {
                uIAppears[i].Close(null);
                if (uIAppears[i].closeDuration > t)
                {
                    t = uIAppears[i].closeDuration;
                }
            }
            while (t > 0)
            {
                t -= Time.deltaTime;
                yield return null;
            }

            if (!activeGameObject)
            {
                gameObject.SetActive(false);
            }
            
            if (!ignoreEvt && onCloseEvt != null)
                onCloseEvt.Invoke();
            if (customFinishCallback != null)
                customFinishCallback();
        }

    }
}