using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class MainMenuCanvas : MonoBehaviour
{
    [SerializeField] private RectTransform pageHolder;
    [SerializeField] private float pageSpeed;
    [HideInInspector] public int currentPage = 0;  // 0: main menu, 1: level, 2: option

    // audio
    [SerializeField] private AudioMixer gameAudioMixer;
    [SerializeField] private AudioMixer musicAudioMixer;
    [SerializeField] private Slider gameAudioSlider;
    [SerializeField] private Slider musicAudioSlider;

    private AudioSource[] audioSources;

    // level loading
    [SerializeField] private Transform levelcontent;  // assign transform to access its children

    // Start is called before the first frame update
    void Start()
    {
        gameAudioSlider.value = PlayerPrefs.GetFloat("GameVolume");
        musicAudioSlider.value = PlayerPrefs.GetFloat("MusicVolume");

        gameAudioMixer.SetFloat("Volume", Mathf.Log10(gameAudioSlider.value) * 20);
        musicAudioMixer.SetFloat("Volume", Mathf.Log10(musicAudioSlider.value) * 20);

        audioSources = GetComponents<AudioSource>();

        // lock some of the levels
        UpdateLevel();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentPage == 0)
        {
            pageHolder.localPosition = new Vector3(Mathf.Lerp(pageHolder.localPosition.x, 0, pageSpeed * Time.deltaTime), 0, 0);
        }
        else if (currentPage == 1)
        {
            pageHolder.localPosition = new Vector3(Mathf.Lerp(pageHolder.localPosition.x, -1200, pageSpeed * Time.deltaTime), 0, 0);
        }
        else if (currentPage == 2)
        {
            pageHolder.localPosition = new Vector3(Mathf.Lerp(pageHolder.localPosition.x, 1200, pageSpeed * Time.deltaTime), 0, 0);
        }
    }

    // update information of levels, used later
    private void UpdateLevel()
    {
        GameManaging gameManaging = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManaging>();
        foreach (Transform transform in levelcontent)
        {
            if (transform.gameObject.name[transform.gameObject.name.Length - 1] - '0' > gameManaging.gameData.unlockedLevel)
            {
                // set text to locked and set button to not interactable
                transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().text = "LOCKED";
                transform.GetComponent<Button>().interactable = false;
            }
            else
            {
                // unlock level
                transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().text = transform.gameObject.name;
                transform.GetComponent<Button>().interactable = true;
            }
        }
    }

    // message from buttons
    public void ButtonHover()
    {
        if (!audioSources[0].isPlaying)
        {
            audioSources[0].Play();
        }
    }

    public void ButtonPressed()
    {
        audioSources[2].Play();
    }

    public void Back()
    {
        audioSources[1].Play();
    }

    public void Continue()
    {
        audioSources[3].Play();
        GameObject.FindGameObjectWithTag("GameManager").SendMessage("Continue");
    }

    public void Level()
    {
        currentPage = 1;
    }

    public void Option()
    {
        currentPage = 2;
    }

    public void Menu()
    {
        currentPage = 0;
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void Load(int index)
    {
        GameObject.FindGameObjectWithTag("GameManager").SendMessage("LoadLevel", index);
    }

    public void GameVolume(Slider slider)
    {
        gameAudioMixer.SetFloat("Volume", Mathf.Log10(slider.value) * 20);  // due to human hearing, according to the formula on wikipedia
        PlayerPrefs.SetFloat("GameVolume", slider.value);
        PlayerPrefs.Save();
    }

    public void MusicVolume(Slider slider)
    {
        musicAudioMixer.SetFloat("Volume", Mathf.Log10(slider.value) * 20);  // due to human hearing, according to the formula on wikipedia
        PlayerPrefs.SetFloat("MusicVolume", slider.value);
        PlayerPrefs.Save();
    }

    public void Cheat()
    {
        GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManaging>().gameData.unlockedLevel = levelcontent.childCount;
        UpdateLevel();
    }

    public void Clear()
    {
        GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManaging>().gameData.unlockedLevel = 1;
        GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManaging>().gameData.level = 1;
        UpdateLevel();
    }
}
