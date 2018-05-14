using UnityEngine;
using Zenject;


namespace MenuSystemWithZenject
{
    public class MenuManagerMonoBehaviour : MonoBehaviour
    {
        private StartMenu.Factory _startMenuFactory;


        [Inject]
        private void Init(StartMenu.Factory startMenuFactory)
        {
            _startMenuFactory = startMenuFactory;
            _startMenuFactory.Create().Open();
        }
    }
}
