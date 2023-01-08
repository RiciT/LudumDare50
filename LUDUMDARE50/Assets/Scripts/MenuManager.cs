using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;

    public GameObject mainMenu;
    public GameObject tutorialMenu;
    public GameObject gameMenu;
    public GameObject optionsMenu;
    public GameObject endScreenMenu;
    public GameObject upgradesMenu;

    public Slider musicVolumeSlider;
    public Slider soundFxVolumeSlider;
    public Toggle postProcessingToggle;

    public TMP_Text tutorialText;
    public Button tutorialStartButton;

    public TMP_Text endScreenResultText;
    public TMP_Text endScreenInevitableText;

    private void Start()
    {
        Instance = this;

        mainMenu.SetActive(true);
        tutorialMenu.SetActive(false);
        gameMenu.SetActive(false);
        optionsMenu.SetActive(false);
        endScreenMenu.SetActive(false);
        upgradesMenu.SetActive(false);

        tutorialStartButton.interactable = false;

        SoundManager.Instance.haveToStartRoundMusic = false;
        SoundManager.Instance.haveToGameMusic = false;
        SoundManager.Instance.haveToLoadingMusic = false;
        SoundManager.Instance.haveToMenuMusic = true;

        ChangeMusicVolume();
        ChangeSoundFxVolume();
        TogglePostProcessing();
    }

    public void OpenOptions()
    {
        mainMenu.SetActive(false);
        optionsMenu.SetActive(true);
    }
    public void CloseOptions()
    {
        optionsMenu.SetActive(false);
        mainMenu.SetActive(true);
    }

    public void OpenUpgrades()
    {
        mainMenu.SetActive(false);
        upgradesMenu.SetActive(true);
    }

    public void OpenMainMenu()
    {
        upgradesMenu.SetActive(false);
        mainMenu.SetActive(true);
    }

    public void ChangeMusicVolume()
    {
        SoundManager.Instance.volume = musicVolumeSlider.value;
    }
    public void ChangeSoundFxVolume()
    {

    }
    public void TogglePostProcessing()
    {

    }

    public void OpenEndScreen(bool victory)
    {
        gameMenu.SetActive(false);
        endScreenMenu.SetActive(true);

        if (victory)
        {
            endScreenResultText.text = "You've won!";
            endScreenInevitableText.text = "But your death was inevitable...";
        }
        else
        {
            endScreenResultText.text = "Game Over";
            endScreenInevitableText.text = "Your death was inevitable...";
        }
    }
    public void PlayAgain()
    {
        endScreenMenu.SetActive(false);
        mainMenu.SetActive(true);
        tutorialStartButton.interactable = false;

        SoundManager.Instance.haveToMenuMusic = true;
        SoundManager.Instance.haveToLoadingMusic = false;
        SoundManager.Instance.haveToGameMusic = false;
        SoundManager.Instance.haveToStartRoundMusic = false;
    }

    public void OpenTutorial()
    {
        mainMenu.SetActive(false);
        tutorialMenu.SetActive(true);

        tutorialText.text = "Generating Map...";

        SoundManager.Instance.haveToMenuMusic = false;
        SoundManager.Instance.haveToStartRoundMusic = false;
        SoundManager.Instance.haveToGameMusic = false;
        SoundManager.Instance.haveToLoadingMusic = true;

        StartCoroutine(MapGenerator.Instance.CreateMap());
    }
    public void UpdateTutorial()
    {
        tutorialText.text = "Click to start the game...";
        tutorialStartButton.interactable = true;

        GameLogic.Instance.GenerateEnemies();
    }

    public void OpenGame()
    {
        tutorialMenu.SetActive(false);
        gameMenu.SetActive(true);

        SoundManager.Instance.haveToMenuMusic = false;
        SoundManager.Instance.haveToLoadingMusic = false;
        SoundManager.Instance.haveToGameMusic = false;
        SoundManager.Instance.haveToStartRoundMusic = true;

        PlayerController.Instance.SpawnPlayer();
        GameLogic.Instance.timeText.text = "Remaining Time: " + GameLogic.Instance.remainingTime;
        StartCoroutine(StartGame());
    }
    public IEnumerator StartGame()
    {
        yield return new WaitForSeconds(GameLogic.Instance.waitBetweenRound);

        StartCoroutine(GameLogic.Instance.DisplayRoundNumber());

        yield return new WaitForSeconds(GameLogic.Instance.waitBetweenRound);

        GameLogic.Instance.startGame = true;
        StartCoroutine(GameLogic.Instance.StartCountdown());
    }
}
