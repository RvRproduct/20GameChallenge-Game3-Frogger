// Game and Code By RvRproduct (Roberto Valentino Reynoso)
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public struct LevelNames
    {
        public const string MainMenu = "MainMenu";
        public const string LevelOne = "LevelOne";
        public const string LevelTwo = "LevelTwo";
        public const string LevelThree = "LevelThree";
    }

    public static LevelManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadLevel(string level)
    {
        SceneManager.LoadScene(level);
    }
}
