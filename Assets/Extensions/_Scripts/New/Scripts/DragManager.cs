using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Extensions.New.Scripts
{
    public class DragManager : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerUpHandler
    {
        public static DragManager Instance;
        private DragItem currentItem;

        private void Awake()
        {
            Instance = this;
        }

        private Vector3 MouseWorldPosition(Vector3 position)
        {
            return Camera.main.ScreenToWorldPoint(position);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            Vector3 position = eventData.position;
            position.z = 0f;
            RaycastHit2D hit = Physics2D.Raycast(MouseWorldPosition(position), Vector3.down);
            if (hit && hit.transform.TryGetComponent(out DragItem item))
            { 
                currentItem = item;
                currentItem.OnSelected();
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!currentItem) return;
            transform.SetParent(transform.root);
            Vector3 pos = Camera.main.ScreenToWorldPoint(eventData.position);
            currentItem.OnDrag(pos);
            RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.down);
            if (hit)
            {
                if (hit.transform.TryGetComponent(out DragItem item))
                {
                    /*SwapItem(currentItem, item);*/
                }
            }
        }

        public void SwapItem(DragItem item1, DragItem item2)
        {
            Transform tmpParent = item1.OriginParent;
            item1.OriginParent = item2.OriginParent;
            item2.OriginParent = tmpParent;
            item2.transform.SetParent(tmpParent);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            ActionEndDrag(eventData.position);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            ActionEndDrag(eventData.position);
        }

        private void ActionEndDrag(Vector3 position)
        {
            if (currentItem)
            {
                Vector3 pos = Camera.main.ScreenToWorldPoint(position);
                RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.down);
                if (hit)
                {
                    if (hit.transform.TryGetComponent(out DragItem item))
                    {
                        SwapItem(currentItem, item);
                    }
                }
                currentItem.OnDrop();
                currentItem = null;
            }
        }
    }
}