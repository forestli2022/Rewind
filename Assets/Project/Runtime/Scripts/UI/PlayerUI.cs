using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.Video;
using UnityEngine.Audio;

public class PlayerUI : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private PlayerAnimation pa;
    [SerializeField] private PlayerCombatState pcs;
    [SerializeField] private PistolBehaviour pb;
    [SerializeField] private GameObject pistol;
    [SerializeField] private PickUpCube puc;
    [SerializeField] private PlayerFinisher pf;
    private Rigidbody rb;
    private Timer timer;
    private PlayerControl playerControl;
    private bool firstFrameAfter;

    [Header("Centre")]
    [SerializeField] private Transform centre;
    [SerializeField] private Transform c1;
    [SerializeField] private Transform c2;
    [SerializeField] private Transform c3;
    [SerializeField] private Transform c4;
    [SerializeField] private GameObject centreCircle;
    [SerializeField] private float centreOriginalScale;
    [SerializeField] private float centreSeparateScale;
    [SerializeField] private float centreObjectOriginalScale;
    [SerializeField] private float centreObjectSeparateScale;
    [SerializeField] private float lerpSpeed;
    // hit mark
    [SerializeField] private ParticleSystem hitMark;

    [Header("Ability Cool Down")]
    [SerializeField] private Image CDFiller;
    [SerializeField] private TextMeshProUGUI CDNumber;
    [SerializeField] private ParticleSystem halo;
    [SerializeField] private ParticleSystem abilityReady;
    private bool abilityReadyPlayed = false;
    [Header("Health Bar")]
    [SerializeField] private Image healthBar;
    [SerializeField] private Image healthBarSlow;
    [SerializeField] private float damageReactingSpeed;
    [SerializeField] private Color healthyColour;
    [SerializeField] private Color unHealthyColour;
    [Header("Reloading")]
    [SerializeField] private Image reloadingSign;

    [Header("Bullet Count")]
    [SerializeField] private GameObject bulletCounter;
    [SerializeField] private TextMeshProUGUI currentBulletCount;
    [SerializeField] private Color bulletBlue;
    [SerializeField] private Color bulletRed;
    [SerializeField] private int counterNomralSize;
    [SerializeField] private int counterEnlargedSize;

    [Header("Time State")]
    [SerializeField] private TextMeshProUGUI timeStateText;
    [SerializeField] private Color rewindColour;
    [SerializeField] private Color pastColour;
    [SerializeField] private Color currentColour;

    [Header("PIT stableness")]
    [SerializeField] private TextMeshProUGUI PITStablenessText;
    private int stablenessScore;
    private List<int> pitList = new List<int>();
    [SerializeField] Color stableCol;
    [SerializeField] Color unstableCol;
    [SerializeField] Color chaoticCol;

    [Header("Terminal Detection")]
    [SerializeField] private LayerMask onlyTerminal;
    [SerializeField] private float terminalDetectionDistance;
    [SerializeField] TextMeshProUGUI interactionButtonText;

    [Header("Tutorial")]
    [SerializeField] RectTransform tutorialWindow;
    [SerializeField] TextMeshProUGUI tutorialText;
    private float tutorialTimeCnt;
    [SerializeField] private float tutorialShowTime;
    [SerializeField] private Transform windowEndPos;
    [SerializeField] private Transform windowOriginPos;
    private bool tutorialActivated;

    [Header("Death")]
    [SerializeField] private VideoPlayer deathVideo;
    [SerializeField] private GameObject presentedVideo; // at the start, video is initialized to not activated gameobject

    [Header("Rewind Timing")]
    [SerializeField] private TextMeshProUGUI rewindTime;

    [Header("Pause Menu")]
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private RectTransform content;
    [SerializeField] private Slider gameVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sensitivitySlider;

    [Header("Audio")]
    [SerializeField] private AudioMixer gameAudioMixer;
    [SerializeField] private AudioMixer musicAudioMixer;
    [SerializeField] private Slider gameAudioSlider;
    [SerializeField] private Slider musicAudioSlider;

    // clone modification
    private bool isClone;
    private PITInteractions pit;
    private ClonedPlayer cp;
    // Start is called before the first frame update
    void Start()
    {
        timer = GameObject.Find("Time").GetComponent<Timer>();
        rb = GetComponent<Rigidbody>();
        playerControl = GetComponent<PlayerInput>().playerControl;
        isClone = gameObject.tag == "Clone";
        if (isClone)
        {
            cp = GetComponent<ClonedPlayer>();
        }

        pauseMenu.SetActive(false);

        // audio
        gameAudioSlider.value = PlayerPrefs.GetFloat("GameVolume");
        musicAudioSlider.value = PlayerPrefs.GetFloat("MusicVolume");

        gameAudioMixer.SetFloat("Volume", Mathf.Log10(gameAudioSlider.value) * 20);
        musicAudioMixer.SetFloat("Volume", Mathf.Log10(musicAudioSlider.value) * 20);
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        isClone = gameObject.tag == "Clone";
        // pit for clone
        if (isClone)
        {
            pit = cp.pit;
        }


        AdjustCentre();
        AdjustCoolDown();
        AdjustReloadingSign();
        AdjustBulletCounter();
        TimeState();
        PITStableness();
        Interaction();
        Tutorial();
        RewindTime();
        PauseMenu();
    }

    private void AdjustCentre()
    {
        if (!pa.canShoot || !pistol.activeInHierarchy)
        {
            centre.gameObject.SetActive(false);
            centreCircle.SetActive(true);
        }
        else if (!pa.aiming)
        {
            centre.gameObject.SetActive(true);
            centreCircle.SetActive(false);
            c1.localScale = Vector3.Lerp(c1.localScale, new Vector3(centreSeparateScale, centreSeparateScale, centreSeparateScale), lerpSpeed * Time.fixedDeltaTime);
            c2.localScale = Vector3.Lerp(c2.localScale, new Vector3(centreSeparateScale, centreSeparateScale, centreSeparateScale), lerpSpeed * Time.fixedDeltaTime);
            c3.localScale = Vector3.Lerp(c3.localScale, new Vector3(centreSeparateScale, centreSeparateScale, centreSeparateScale), lerpSpeed * Time.fixedDeltaTime);
            c4.localScale = Vector3.Lerp(c4.localScale, new Vector3(centreSeparateScale, centreSeparateScale, centreSeparateScale), lerpSpeed * Time.fixedDeltaTime);
            centre.localScale = Vector3.Lerp(centre.localScale, new Vector3(centreObjectSeparateScale, centreObjectSeparateScale, centreObjectSeparateScale), lerpSpeed * Time.fixedDeltaTime);

        }
        else
        {
            centre.gameObject.SetActive(true);
            centreCircle.SetActive(false);
            c1.localScale = Vector3.Lerp(c1.localScale, new Vector3(centreOriginalScale, centreOriginalScale, centreOriginalScale), lerpSpeed * Time.fixedDeltaTime);
            c2.localScale = Vector3.Lerp(c2.localScale, new Vector3(centreOriginalScale, centreOriginalScale, centreOriginalScale), lerpSpeed * Time.fixedDeltaTime);
            c3.localScale = Vector3.Lerp(c3.localScale, new Vector3(centreOriginalScale, centreOriginalScale, centreOriginalScale), lerpSpeed * Time.fixedDeltaTime);
            c4.localScale = Vector3.Lerp(c4.localScale, new Vector3(centreOriginalScale, centreOriginalScale, centreOriginalScale), lerpSpeed * Time.fixedDeltaTime);
            centre.localScale = Vector3.Lerp(centre.localScale, new Vector3(centreObjectOriginalScale, centreObjectOriginalScale, centreObjectOriginalScale), lerpSpeed * Time.fixedDeltaTime);
        }
    }

    private void AdjustCoolDown()
    {
        CDNumber.text = (Mathf.Round(timer.cd * 10) / 10f).ToString();
        if (timer.inPast && !timer.rewinding && timer.cd == timer.coolDown)
        {
            CDFiller.fillAmount = Mathf.Lerp(CDFiller.fillAmount, 0, 10 * Time.fixedDeltaTime);
        }
        else
        {
            CDFiller.fillAmount = (timer.coolDown - timer.cd) / timer.coolDown;
        }

        // if ability activated
        if (timer.inPast)
        {
            halo.Play();
        }
        else
        {
            halo.Stop();
        }

        // when ability ready, play the ability ready particle effect
        if (timer.cd == 0 && !abilityReadyPlayed)
        {
            abilityReadyPlayed = true;
            abilityReady.Play();
        }
        else if (timer.cd != 0)
        {
            abilityReadyPlayed = false;
        }

        // health bar
        healthBar.fillAmount = pcs.health / pcs.maxHealth;
        healthBar.color = Color.Lerp(unHealthyColour, healthyColour, healthBar.fillAmount);

        // health bar slow
        healthBarSlow.fillAmount = Mathf.Lerp(healthBarSlow.fillAmount, healthBar.fillAmount, damageReactingSpeed * Time.fixedDeltaTime);
    }

    private void AdjustReloadingSign()
    {
        if (pistol.activeInHierarchy && pb.reloading)
        {
            reloadingSign.enabled = true;
            reloadingSign.fillAmount = pb.currentReloadTime / pb.reloadTime;
        }
        else
        {
            reloadingSign.enabled = false;
        }
    }

    private void AdjustBulletCounter()
    {
        // only if pistol active, show the bullet counter
        if (pistol.activeInHierarchy)
        {
            bulletCounter.SetActive(true);
        }
        else
        {
            bulletCounter.SetActive(false);
            return;
        }

        // adjust colour
        if (pb.bulletNum > 3)
        {
            currentBulletCount.color = bulletBlue;
        }
        else
        {
            currentBulletCount.color = bulletRed;
        }

        // adjust size
        if (playerControl.Player.Shoot.IsPressed() && pb.isShooting)
        {
            currentBulletCount.fontSize = counterEnlargedSize;
        }
        currentBulletCount.text = pb.bulletNum.ToString();
        currentBulletCount.fontSize = (int)Mathf.Lerp(currentBulletCount.fontSize, counterNomralSize, 10 * Time.fixedDeltaTime);
    }

    private void TimeState()
    {
        if (timer.inPast)
        {
            if (timer.rewinding)
            {
                timeStateText.text = "Rewinding";
                timeStateText.color = rewindColour;
            }
            else
            {
                timeStateText.text = "Past";
                timeStateText.color = pastColour;
            }
        }
        else
        {
            timeStateText.text = "Current";
            timeStateText.color = currentColour;
        }
    }

    private void PITStableness()
    {
        if (timer.rewinding && pitList.Count > 0)
        {
            stablenessScore = pitList[pitList.Count - 1];
            pitList.RemoveAt(pitList.Count - 1);
            firstFrameAfter = true;
        }
        else
        {
            stablenessScore = 0;
            if (rb.velocity.magnitude > 3)
            {
                stablenessScore += 1;
            }
            if (pb.isShooting)
            {
                stablenessScore += 1;
            }
            if (pb.ownPistol && !pa.canShoot)
            {
                stablenessScore += 1;
            }

            if (pitList.Count > timer.maximumRecordingTime / Time.fixedDeltaTime)
            {
                pitList.RemoveAt(0);
            }
            if (firstFrameAfter)
            {
                firstFrameAfter = false;
                pitList.Clear();
            }
            pitList.Add(stablenessScore);
        }


        // show stableness score
        if (stablenessScore == 0)
        {
            PITStablenessText.text = "Stable";
            PITStablenessText.color = stableCol;
        }
        else if (stablenessScore == 1)
        {
            PITStablenessText.text = "Unstable...";
            PITStablenessText.color = unstableCol;
        }
        else
        {
            PITStablenessText.text = "CHAOTIC!!!";
            PITStablenessText.color = chaoticCol;
        }
    }

    // interaction indications are set up here because it closely relates to what the UI shows and player's action to avoid any collisions
    private void Interaction()  // interactions has priority, picking up cubes first, then executing enemies, then activating terminal
    {
        if (timer.rewinding)
        {
            return;
        }
        string keyBinding = playerControl.Player.MultiFunction.bindings[0].path;
        string boundKey = keyBinding.Substring(keyBinding.Length - 1, 1).ToUpper();

        // picking up cube
        if (puc.canPickUp)
        {
            interactionButtonText.text = "[" + boundKey + "] Pick Up Cube";  // picking up cube takes quite a few lines of code, so it is in individual script;
            return;
        }
        // placing cube
        if (puc.hasCube)
        {
            interactionButtonText.text = "[" + boundKey + "] Place Cube";
            return;
        }


        // executing enemies
        if (pf.canExecute)
        {
            interactionButtonText.text = "[" + boundKey + "] Execute";
            return;
        }


        // terminal
        Collider[] terminals = Physics.OverlapSphere(transform.position, terminalDetectionDistance, onlyTerminal);
        if (terminals.Length != 0)
        {
            GameObject currentTerminal = terminals[0].gameObject;
            if (!currentTerminal.GetComponent<TimeControlTerminal>().IsActivated())
            {
                if (interactionButtonText.text != "[" + boundKey + "] Activate Terminal")
                {
                    interactionButtonText.text = "[" + boundKey + "] Activate Terminal";
                }

                // if key pressed, activate the terminal 
                if (Utils.keyPressed(playerControl.Player.MultiFunction, pit, isClone))
                {
                    currentTerminal.GetComponent<TimeControlTerminal>().ActivateTerminal(true);
                }
                return;
            }
        }
        interactionButtonText.text = "";
    }

    private void PauseMenu()
    {
        if (gameObject.tag != "Clone" && Input.GetKeyDown(KeyCode.Escape))
        {
            if (!pauseMenu.activeInHierarchy)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                Time.timeScale = 0;
                pauseMenu.SetActive(true);
            }
            else
            {
                Resume();
            }
        }
    }

    // show tutorial window when on specific activation area
    private void Tutorial()
    {
        if (tutorialActivated)
        {
            // lerp tutorial to desired location
            tutorialWindow.position = Vector3.Lerp(tutorialWindow.position, windowEndPos.position, 5 * Time.fixedDeltaTime);

            // increment time, if time greater than maximum limit, deactivate tutorial
            tutorialTimeCnt += Time.fixedDeltaTime;
            if (tutorialTimeCnt >= tutorialShowTime)
            {
                tutorialTimeCnt = 0;
                tutorialActivated = false;
            }
        }
        else
        {
            // lerp tutorial back
            tutorialWindow.position = Vector3.Lerp(tutorialWindow.position, windowOriginPos.position, 5 * Time.fixedDeltaTime);
        }
    }

    // record the the timing of rewind
    private void RewindTime()
    {
        if (timer.inPast)
        {
            if (!rewindTime.gameObject.activeInHierarchy)
            {
                rewindTime.gameObject.SetActive(true);
            }
            rewindTime.text = timer.rewindTime.ToString("F2");
        }
        else if (rewindTime.gameObject.activeInHierarchy)
        {
            rewindTime.gameObject.SetActive(false);
        }
    }

    // messages from other scripts
    public void BulletHitEnemy()
    {
        hitMark.Play();
    }

    public void Death()
    {
        presentedVideo.SetActive(true);
        deathVideo.Play();
        playerControl.Disable();
    }

    // message from tutorial activation area
    public void PlayTutorial(string value)
    {
        tutorialText.text = value;
        tutorialActivated = true;
        tutorialTimeCnt = 0;
        tutorialWindow.position = windowOriginPos.position;
    }

    // message from RESUME button
    public void Resume()
    {
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        pauseMenu.SetActive(false);
    }

    public void Restart()
    {
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        gameObject.SendMessage("ReduceHealth", 999);
    }

    public void MainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Main Menu");
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

    // reset button
    public void Reset()
    {
        gameVolumeSlider.value = 1;
        musicVolumeSlider.value = 1;
        sensitivitySlider.value = 100;
    }
}
