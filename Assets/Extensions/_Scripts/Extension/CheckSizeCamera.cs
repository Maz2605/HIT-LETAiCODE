using UnityEngine;

namespace Duc
{
    public class CheckSizeCamera : MonoBehaviour
    {
        public Camera cam;
        public float defaultSize = 9.6f;

        // Start is called before the first frame update
        void Awake()
        {
            cam = GetComponent<Camera>();
            
            float screenRatio = (float)Screen.width / (float)Screen.height;
            float targetRatio = 10.8f / 19.2f;

            if (screenRatio >= targetRatio)
            {
                cam.orthographicSize = defaultSize;
            }
            else
            {
                float changeSize = targetRatio / screenRatio;
                cam.orthographicSize = defaultSize * changeSize;
            }
        }

#if UNITY_EDITOR
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                float screenRatio = (float)Screen.width / (float)Screen.height;
                float targetRatio = 10.8f / 19.2f;

                if (screenRatio >= targetRatio)
                {
                    cam.orthographicSize = defaultSize;
                }
                else
                {
                    float changeSize = targetRatio / screenRatio;
                    cam.orthographicSize = defaultSize * changeSize;
                }
            }
        }
#endif
    }
}
