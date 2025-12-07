using UnityEngine;
using UnityEngine.UI;

namespace Duc
{
    public class CheckSizeCanvas : MonoBehaviour
    {
        [SerializeField] private Camera cam;
        [SerializeField] private CanvasScaler canvasScaler;

        void Awake()
        {
            if (canvasScaler == null)
            {
                canvasScaler = GetComponent<CanvasScaler>();
            }

            canvasScaler.matchWidthOrHeight = cam.aspect < 0.55f ? 0 : 1;
        }
    }
}