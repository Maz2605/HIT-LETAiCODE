using UnityEngine;

namespace DesignPattern
{
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;
        public static bool DontDestroy { get; set; }
        public static T Instance
        {
            get
            {
                // nếu null thì tìm object có component T đang được active
                if (_instance == null)
                {
                    _instance = (T)FindObjectOfType<T>(true);
                    // nếu null tiếp thì sinh ra một game object mới
                    if(_instance == null)
                    {
                        GameObject singletonObject = new GameObject();
                        _instance = singletonObject.AddComponent<T>();
                        singletonObject.name = typeof(T).Name + " Singleton";
                        if (DontDestroy)
                        {
                            DontDestroyOnLoad(singletonObject);
                        }
                    }
                }
                return _instance;
            }
        }

        protected virtual void KeepAlive(bool enable)
        {
            DontDestroy = enable;
        }
        protected virtual void Awake()
        {
            if (_instance && _instance.GetInstanceID() != this.GetInstanceID())
            {
                Destroy(this);
            }
            if (!_instance)
            {
                // ép kiểu dữ liệu về MonoBehaviour rồi ép về dạng tổng quát
                _instance = (T)(MonoBehaviour)this;
            }
            if (DontDestroy)
            {
                DontDestroyOnLoad(this);
            }
        }
    }
}
