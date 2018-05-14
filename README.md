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

How to use:
1. You need to create a Zenject Project Context in Resource folder, all the menus will be a child in the hierarchy of this Project Context at runtime. 
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

Some use tips:
1. You need to use the ```Factory.Create()``` to find the instance of the menu so it get injected by Zenject when created.
2. All the menus inhereit from Menu has OnBackPress() method that will destroy/disable it self when called based on the setting of this menu.
3. ```OnMenuEnabled``` Action in MenuManager.cs is called when Menu is enabled, you can override ```OnEnable()``` method to do some notification stuff.
4. You can override ```OpenAnimation()``` and ```CloseAnimation()``` to add animation when Menu is opened or closed.
5. ```public void GoToMenu(Menu instance, bool shouldCloseAlwaysOnTopMenu = false)``` will let you jump to ```Menu instance```, close all the menus in between.
6. ```AlwaysKeepOnTop``` is a tag that will make the Menu on top of all the Menus without it, ```OnBackPressed()``` on other Menus will not close Menus with ```AlwaysKeepOnTop``` set to true, you can close it specifically call this Menu's ```OnBackPressed()```. Good to use for overlay menus.
