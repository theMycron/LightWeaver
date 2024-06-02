using UnityEngine.SceneManagement;
using UnityEngine;
using TMPro;

public class NavigatebetweenScenes : MonoBehaviour
{

    private AudioManager audioManager;
    public AudioClip clickSound;

    [SerializeField] private Canvas mainMenu;
    [SerializeField] private Canvas settings;
    [SerializeField] private Canvas levelSelect;
    [SerializeField] private Canvas credits;
    [SerializeField] private TextMeshProUGUI startButtonText;

    private Canvas currentMenu;
    private LevelManager levelManager;

    public enum Menus
    {
        MainMenu = 1,
        Settings = 2,
        LevelSelect = 3,
        Credits = 4
    }

    private void Start() {
        currentMenu = mainMenu;
        levelManager = FindAnyObjectByType<LevelManager>();
        audioManager = FindAnyObjectByType<AudioManager>();
        //clickSound = GetComponent<AudioSource>();

        // if this is a new save, show "New Game" instead of "Continue"
        if (levelManager != null && levelManager.gameSave == null)
        {
            startButtonText.text = "New Game";
        }
    }

    public void OnButtonClick()
    {
        audioManager.PlaySFX(clickSound);
    }

    public void goToScene(string sceneName) {

        SceneManager.LoadScene(sceneName);
    }

    public void SwitchMenu(int menuNumber)
    {
        Menus menu = (Menus)menuNumber;
        Debug.Log("Menu is : " + menu.ToString());
        currentMenu.gameObject.SetActive(false);
        switch (menu)
        {
            case Menus.MainMenu:
                currentMenu = mainMenu;
                break;
            case Menus.Settings:
                currentMenu = settings;
                break;
            case Menus.LevelSelect:
                currentMenu = levelSelect;
                break;
            case Menus.Credits:
                currentMenu = credits;
                break;
        }
        Debug.Log("Current Menu is : " + currentMenu.ToString());
        currentMenu.gameObject.SetActive(true);
    }

    public void ContinueGame()
    {
        levelManager.ContinueGame();
    }

    public void exitGame() {

        Application.Quit();
    }
}
