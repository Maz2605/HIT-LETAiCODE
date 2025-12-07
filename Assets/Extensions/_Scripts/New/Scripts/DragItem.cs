using UnityEngine;
using UnityEngine.UI;

namespace Extensions.New.Scripts
{
    public class DragItem : MonoBehaviour
    {
        [SerializeField] private Transform originParent;
        [SerializeField] private Image img;
        [SerializeField] private Collider2D collider;

        public Transform OriginParent
        {
            get => originParent;
            set { originParent = value; }
        }

#if UNITY_EDITOR
        
        [ContextMenu("Get Component")]
        public void GetComponent()
        {
            Debug.LogError(name);
            if (originParent == null) originParent = transform.parent;
            if (img == null) img = GetComponent<Image>();
            collider = GetComponent<Collider2D>();
        }
#endif
        public void OnSelected()
        {
            if (img != null) img.raycastTarget = false;
            originParent = transform.parent;
            collider.enabled = false;
        }

        public void OnDrag(Vector3 position)
        {
            position.z = 0;
            transform.SetParent(transform.root);
            transform.position = position;
        }
        public void OnDrop()
        {
            img.raycastTarget = true;
            transform.SetParent(originParent);
            collider.enabled = true;
        }
    }
}