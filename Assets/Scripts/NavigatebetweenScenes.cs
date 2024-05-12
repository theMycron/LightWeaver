using UnityEngine.SceneManagement;
using UnityEngine;

public class NavigatebetweenScenes : MonoBehaviour
{

    [SerializeField] AudioSource clickSound;
    public AudioClip sound;

    private void Start() {

        //clickSound = GetComponent<AudioSource>();
        clickSound.clip = sound;
    }

    public void OnButtonClick()
    {
        clickSound.clip = sound;
        clickSound.Play();
    }

    public void goToScene(string sceneName) {


        SceneManager.LoadScene(sceneName);
    }

   

    public void exitGame() {

        Application.Quit();
    }
}
