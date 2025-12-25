using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class GameManaging : MonoBehaviour
{
    public GameData gameData;
    private Timer timer;

    // scene transition: constant player view/orientation
    [HideInInspector] public float rotX;
    [HideInInspector] public float rotY;
    [HideInInspector] public Vector3 playerToPlatformPos;
    [HideInInspector] public bool fromLastScene;

    // on death, load scene after 3 seconds
    private bool dead = false;
    private float timeCnt = 0;

    // load level when continue button pressed
    private bool loading = false;
    private int levelToLoad = 1;
    [HideInInspector] public bool justLoaded;
    // music
    private AudioSource music;


    void Awake()  // the game object does not get destroyed after loading another scene
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("GameManager");

        if (objs.Length > 1)
        {
            Destroy(this.gameObject);
        }
        fromLastScene = false;

        // get time
        if (SceneManager.GetActiveScene().name != "Main Menu")
        {
            timer = GameObject.Find("Time").GetComponent<Timer>();
        }


        // load file
        gameData = SaveSystem.Load();
        DontDestroyOnLoad(this.gameObject);
        if (SceneManager.GetActiveScene().name != "Main Menu")
        {
            gameData.level = StringToLevel(SceneManager.GetActiveScene().name);
        }

        music = GameObject.Find("Music").GetComponent<AudioSource>();
    }


    // a function generating level string to load scene
    private string LevelToString(int level)
    {
        return "Level " + level.ToString();
    }

    // a function generating level number from string
    private int StringToLevel(string levelName)
    {
        return int.Parse(levelName.Substring(6, levelName.Length - 6));
    }

    // when quiting the game, save
    void OnApplicationQuit()
    {
        SaveSystem.Save(gameData);
    }


    // message from player if dead
    public void PlayerKilled()
    {
        // add number of deaths
        gameData.numberOfDeaths++;
        dead = true;

        // disable all scripts on player
        MonoBehaviour[] scripts = GameObject.Find("Player").GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour script in scripts)
        {
            script.enabled = false;
        }

        // if inPast, disable all scripts on clone
        if (timer.inPast)
        {
            scripts = GameObject.Find("Player(Clone)").GetComponents<MonoBehaviour>();
            foreach (MonoBehaviour script in scripts)
            {
                script.enabled = false;
            }
        }
    }


    // message from elevator (checkpoint) to load the next level
    public void LoadNextLevel(Vector3 elevatorPos)
    {
        fromLastScene = true;

        // record player's rotation and position relating to the elevator
        GameObject player = GameObject.Find("Player");
        rotX = player.GetComponent<PlayerLook>().xRotation;
        rotY = player.GetComponent<PlayerLook>().yRotation;
        playerToPlatformPos = player.transform.position - elevatorPos;

        // increment the level currently on
        string currentLevel = SceneManager.GetActiveScene().name;
        gameData.level = StringToLevel(currentLevel) + 1;

        // unlock the level if not played before
        gameData.unlockedLevel = Mathf.Max(gameData.level, gameData.unlockedLevel);
        if (Application.CanStreamedLevelBeLoaded(LevelToString(gameData.level)))
        {
            SceneManager.LoadScene(LevelToString(gameData.level));
        }
        else
        {
            SceneManager.LoadScene("Main Menu");
            Cursor.lockState = CursorLockMode.None;
        }

        justLoaded = true;
    }


    // message from continue button (continue last level)
    public void Continue()
    {
        // load the last played level
        LoadLevel(gameData.level);
    }

    public void LoadLevel(int index)
    {
        loading = true;
        levelToLoad = index;
        timeCnt = 0;
    }

    private void Update()
    {
        if (dead)
        {
            timeCnt += Time.deltaTime;
            if (timeCnt >= 3)
            {
                // reload scene
                dead = false;
                timeCnt = 0;
                justLoaded = true;
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }

        if (loading)
        {
            timeCnt += Time.deltaTime;
            music = GameObject.Find("Music").GetComponent<AudioSource>();
            music.volume = Mathf.Lerp(music.volume, 0, timeCnt / 2);
            if (timeCnt >= 2)
            {
                // load level
                loading = false;
                timeCnt = 0;
                justLoaded = true;
                SceneManager.LoadScene(LevelToString(levelToLoad));
            }
        }
    }
}
