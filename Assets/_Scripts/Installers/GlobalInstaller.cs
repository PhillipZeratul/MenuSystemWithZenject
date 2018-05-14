using UnityEngine;
using Zenject;
using MenuSystemWithZenject;


public class GlobalInstaller : MonoInstaller<GlobalInstaller>
{
    public GameObject startMenu;
    public GameObject gameMenu;


    // TODO:~ Maybe I'm abusing ProjectContext, move something to SceneContext perhaps.
    public override void InstallBindings()
    {
        Debug.Log("GlobalInstaller Global Binding.");

        // Bind MenuSystemWithZenject
        Container.Bind<MenuManager>().AsSingle();

        Container.Bind<GameObject>().FromInstance(startMenu).WhenInjectedInto<StartMenu.CustomMenuFactory>();
        Container.BindFactory<StartMenu, StartMenu.Factory>().FromFactory<StartMenu.CustomMenuFactory>();

        Container.Bind<GameObject>().FromInstance(gameMenu).WhenInjectedInto<GameMenu.CustomMenuFactory>();
        Container.BindFactory<GameMenu, GameMenu.Factory>().FromFactory<GameMenu.CustomMenuFactory>();
    }
}