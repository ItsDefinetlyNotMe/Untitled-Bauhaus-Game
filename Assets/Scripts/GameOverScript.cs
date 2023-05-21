using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverScript : MonoBehaviour
{
    //TODO rework whole game over mechanic

    [SerializeField] private GameObject gameOverScreen;
    private void Start()
    {
        HitablePlayer.onPlayerDeath += SetUp;
    }

    public void SetUp()
    {
        SceneManager.LoadScene("HUB");

        //gameOverScreen.SetActive(true);
    }

    public void Respawn()
    {
        //Spawn new Player with stats
        //make new Scene
    }

    public void Quit()
    {
        //save Progress
        Application.Quit();
    }

    private void OnDisable()
    {
        HitablePlayer.onPlayerDeath -= SetUp;
    }
}
