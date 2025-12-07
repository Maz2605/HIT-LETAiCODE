using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace _Scripts.UI
{
    public class TextureOffsetAnimator : MonoBehaviour
    {
        public Vector2 velocity;

        private RawImage rawImageComponent;

        private Renderer rendererComponent;

        private void Awake()
        {
            rawImageComponent = GetComponent<RawImage>();
        }

        private void Update()
        {
            rawImageComponent.uvRect = new Rect(rawImageComponent.uvRect.position + velocity * Time.fixedDeltaTime, rawImageComponent.uvRect.size);
        }
    }
}