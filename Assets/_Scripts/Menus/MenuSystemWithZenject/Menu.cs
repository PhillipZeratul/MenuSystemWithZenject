using UnityEngine;
using Zenject;
using System.Collections;


namespace MenuSystemWithZenject
{
    public class Menu<T> : Menu where T : Menu<T>
    {
        public static T Instance { get; private set; }
        [HideInInspector]
        public bool isReady = true;
        private MenuManager _menuManager;

        private WaitForEndOfFrame waitForEndOfFrame;


        [Inject]
        private void Init(MenuManager menuManager)
        {
            _menuManager = menuManager;
        }

        protected void Awake()
        {
            Instance = (T)this;
            _menuManager.OnMenuEnabled += MenuEnabled;
        }

        protected virtual void OnEnable()
        {
            _menuManager.MenuEnabled(Instance);
        }

        protected void OnDestroy()
        {
            //_menuManager.OnMenuEnabled -= MenuEnabled;
            Instance = null;
        }

        public void Open()
        {
            if (Instance == null)
            {
                Debug.LogError("Instance is null, please call Factory.Create() first!");
                return;
            }

            Debug.Log("Openning: " + this.name);

            Instance.gameObject.SetActive(true);
            _menuManager.OpenMenu(Instance);
            StartCoroutine(OpenAnimation());
        }

        public void Close()
        {
            if (Instance)
            {
                if (gameObject.activeSelf)
                {
                    StartCoroutine(CloseWithAnimation());
                }
                else
                {
                    CloseNow();
                }
            }
        }

        private IEnumerator CloseWithAnimation()
        {
            yield return StartCoroutine(CloseAnimation());
            CloseNow();
        }

        private void CloseNow()
        {
            _menuManager.CloseMenu(Instance);
        }

        public override void OnBackPressed()
        {
            Close();
        }

        public virtual void MenuEnabled(Menu instance) {}

        protected virtual IEnumerator OpenAnimation() { yield return waitForEndOfFrame; }           

        protected virtual IEnumerator CloseAnimation() { yield return waitForEndOfFrame; }

        public override void OnMenuEnabled(Menu instance) {}

        public void GoToMenu(Menu instance, bool shouldCloseAlwaysOnTopMenu = false)
        {
            _menuManager.CloseMenuUntil(instance, shouldCloseAlwaysOnTopMenu);
        }

        public class Factory : Factory<T> {}

        public class CustomMenuFactory : IFactory<T>, IValidatable
        {
            DiContainer _container;
            GameObject _prefab;

            public CustomMenuFactory(DiContainer container, GameObject prefab)
            {
                _container = container;
                _prefab = prefab;
            }

            public T Create()
            {
                // If exists an Instance, return Instance.
                if (Instance == null)
                {
                    Debug.LogFormat("In CustormMenuFactory, return New prefab : {0}", _prefab.name);
                    return _container.InstantiatePrefab(_prefab).GetComponent<T>();
                }
                else
                {
                    Debug.LogFormat("In CustormMenuFactory, return existing Instance : {0}", Instance.name);
                    return Instance;
                }
            }

            // @TODO : Not sure if this is correct.
            public void Validate()
            {
                GameObject instance = _container.InstantiatePrefab(_prefab);
                instance.GetComponent<T>();
            }
        }
    }

    public abstract class Menu : MonoBehaviour
    {
        [Tooltip("Destroy the Game Object when menu is closed (reduces memory usage)")]
        public bool DestroyWhenClosed = true;

        [Tooltip("Disable menus that are under this one in the stack")]
        public bool DisableMenusUnderneath = true;

        [Tooltip("Always keep this menu on top, may need to disable raycast")]
        public bool AlwaysKeepOnTop = false;

        public abstract void OnBackPressed();

        public abstract void OnMenuEnabled(Menu instance);
    }
}
