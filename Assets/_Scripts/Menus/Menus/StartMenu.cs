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
