# MenuSystemWithZenject

This is a Unity Menu Framework inspired by \
**MenuSystem** \
https://github.com/YousicianGit/UnityMenuSystem \
Check their talk at Unity Europe 2017 at \
https://www.youtube.com/watch?v=wbmjturGbAQ \
and **Zenject** \
https://github.com/modesttree/Zenject

I started this because my project is written with Zenject and I want the MenuSystem to work with Zenject as well. \
So inspired by the idea of MenuSystem, I did some modification to it and I think the result is quite pleasing.

How to use: \
1. You need to create a Zenject Project Context in Resource folder, all the menus will be a child in the hierarchy of this Project Context in runtime. 
```
Assets -> Create -> Zenject -> Project Context
```
2. Add GlobalInstaller.cs to Project Context's Installer list. \
GlobalInstaller.cs is where the global binding of Zenject happens.

3. Create a new script that derives from Menu<> i.e.
```C#
using Zenject;
using MenuSystemWithZenject;

public class StartMenu : Menu<StartMenu>
{
    private GameMenu.Factory _gameFactory;

    [Inject]
    private void Init(GameMenu.Factory gameFactory)
    {
        _gameFactory = gameFactory;
    }

    public void OnStartClick()
    {
        _gameFactory.Create().Open();
    }
}
```
The above code declares a StartMenu and open the GameMenu when start button is clicked.

4. All the menus are individual canvases and need to be a prefab, add all the prefabs to GlobalInstaller on the Project Context.

5. And you are good to go.

