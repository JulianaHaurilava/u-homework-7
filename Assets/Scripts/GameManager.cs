using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private static int[] ENEMIES_COUNT = { 0, 0, 1, 2, 3, 5, 7, 10, 14, 15, 20, 21 };       // кол-во врагов в раунд
    private static int WINNING_REBELS_AMOUNT = 15;

    [SerializeField] Button AddWarriorButton;                   // кнопка добавить воина
    [SerializeField] Button AddPrinterButton;                   // кнопка добавить полиграфиста

    [SerializeField] TextMeshProUGUI RebelsStatsAmountText;     // текст статов повстанцев
    [SerializeField] TextMeshProUGUI GameStatsAmountText;       // текст статов игры
    [SerializeField] TextMeshProUGUI PomidoroStatsText;         // текст статов врагов

    private int fightsAlive;                                    // число пережитых битв
    [SerializeField] int LeafletsAmount;                        // число листовок
    [SerializeField] int WarriorsAmount;                        // число воинов
    [SerializeField] int PrintersAmount;                        // число полиграфистов

    [SerializeField] Timer LeafletsTimer;                       // таймер печати листовок
    [SerializeField] Timer FightTimer;                          // таймер до столкновения
    [SerializeField] ButtonTimer WarriorTimer;                  // таймер подготовки воина
    [SerializeField] ButtonTimer PrinterTimer;                  // таймер подготовки полиграфиста

    [SerializeField] GameObject ResultPanel;                    // панель завершения игры
    [SerializeField] Image ResultImage;                         // картинка результата
    [SerializeField] TextMeshProUGUI ResultText;                // текст результата
    [SerializeField] TextMeshProUGUI StatsText;                 // статистика игры
    [SerializeField] TextMeshProUGUI DescriptionText;           // текст описания результата игры

    [SerializeField] Sprite WinSprite;
    [SerializeField] Sprite LooseSprite;

    [SerializeField] AudioSource WinAudioSource;
    [SerializeField] AudioSource LostAudioSource;
    [SerializeField] AudioSource ThemeAudioSource;

    [SerializeField] TextMeshProUGUI PauseStatsText;            // текст промежуточной статистики

    private int usedLeaflets;                                   // кол-во использованных листовок

    private float warriorTimerIndex;                           // индекс скорости таймера агитации воина
    private int enemiesAmount;                                  // кол-во врагов
    private int detectivesAmount;                               // кол-во детективов
    private int looses;                                         // кол-во потерь


    void Start()
    {
        looses = 0;
        usedLeaflets = 0;
        fightsAlive = 0;
        enemiesAmount = ENEMIES_COUNT[fightsAlive];
        detectivesAmount = 1;

        UpdateGameStatsAmountText();
        UpdateRebelsStatsAmountText();
        Time.timeScale = 1f;
    }

    void Update()
    {
        // если листовки напечатаны
        if (LeafletsTimer.TaskCompleted)
        {
            LeafletsAmount += PrintersAmount;
            UpdateGameStatsAmountText();
        }
        // если произошло столкновение
        if (FightTimer.TaskCompleted)
        {
            RecountStats();
            UpdateRebelsStatsAmountText();
            CountLooses();

            // игрок проигрывает, если количество врагов больше, чем количество воинов
            if (WarriorsAmount < 0)
            {
                Time.timeScale = 0f;

                ResultText.text = "Поражение";
                DescriptionText.text = "Восстание было подавлено.";
                StatsText.text = PauseStatsText.text;
                ResultImage.sprite = LooseSprite;

                ThemeAudioSource.Stop();
                LostAudioSource.Play();

                ResultPanel.SetActive(true);

            }

            if (fightsAlive == 10)
            {
                if (WarriorsAmount < WINNING_REBELS_AMOUNT)
                {
                    // игрок проигрывает, если к 10 битве его войско не превосходит войско Лимонов на 15
                    Time.timeScale = 0f;

                    ResultText.text = "Поражение";
                    DescriptionText.text = "Восстание было подавлено.";
                    StatsText.text = PauseStatsText.text;
                    ResultImage.sprite = LooseSprite;

                    ThemeAudioSource.Stop();
                    LostAudioSource.Play();

                    ResultPanel.SetActive(true);
                }
                else
                {
                    // игрок побеждает, если суммарное количество повстанцев равно WINNING_REBELS_AMOUNT и пережито 10 битв
                    Time.timeScale = 0f;

                    ResultText.text = "Победа";
                    DescriptionText.text = "Да здравствует свобода!";
                    StatsText.text = PauseStatsText.text;
                    ResultImage.sprite = WinSprite;

                    ThemeAudioSource.Stop();
                    WinAudioSource.Play();

                    ResultPanel.SetActive(true);
                }
            }
            // в любом другом случае игра продолжается

            UpdateWarriorTimerIndex();

            fightsAlive++;
            enemiesAmount = ENEMIES_COUNT[fightsAlive];
            UpdatePomidoroStatsAmountText();
            UpdateGameStatsAmountText();
        }
        // если привлечен полиграфист
        if (PrinterTimer.TaskCompleted)
        {
            PrintersAmount += RandomAgitate();
            AddPrinterButton.enabled = true;
            UpdateRebelsStatsAmountText();
            UpdatePauseStats();
        }
        // если привлечен воин
        if (WarriorTimer.TaskCompleted)
        {
            WarriorsAmount += RandomAgitate();
            AddWarriorButton.enabled = true;
            UpdateRebelsStatsAmountText();
            UpdatePauseStats();

            UpdateWarriorTimerIndex();
        }
    }

    /// <summary>
    /// Обновляет текст, отображающий количество пережитых столкновений и распространенных листовок
    /// </summary>
    private void UpdateGameStatsAmountText()
    {
        GameStatsAmountText.text =
            $"{fightsAlive}\n" +
            $"{LeafletsAmount}\n";
        UpdatePauseStats();
    }

    /// <summary>
    /// Обновляет текст, отображающий количество воинов и полиграфистов
    /// </summary>
    private void UpdateRebelsStatsAmountText()
    {
        RebelsStatsAmountText.text =
            $"{WarriorsAmount}\n" +
            $"{PrintersAmount}\n";
    }

    /// <summary>
    /// Обновляет тектс, отображающий количество врагов и полицейских при следующем столкновении
    /// </summary>
    private void UpdatePomidoroStatsAmountText()
    {
        PomidoroStatsText.text =
            $"{enemiesAmount}\n" +
            $"{detectivesAmount}\n";
    }

    /// <summary>
    /// Запускает процесс привлечения новых воинов
    /// </summary>
    public void AgitateWarrior()
    {
        if (!WarriorTimer.Works && LeafletsAmount > 0)
        {
            usedLeaflets++;
            LeafletsAmount--;
            UpdateGameStatsAmountText();
            WarriorTimer.Works = true;
            AddWarriorButton.enabled = false;
        }
    }

    /// <summary>
    /// Запускает процесс привлечения новых полиграфистов
    /// </summary>
    public void AgitatePrinter()
    {
        if (!PrinterTimer.Works && LeafletsAmount > 0)
        {
            usedLeaflets++;
            LeafletsAmount--;
            UpdateGameStatsAmountText();
            PrinterTimer.Works = true;
            AddPrinterButton.enabled = false;
        }
    }

    /// <summary>
    /// Подсчитывает потери для каждого раунда
    /// </summary>
    /// <param name="fights">Количество перенесенных столкновений</param>
    /// <returns></returns>
    private void CountLooses()
    {
        if (WarriorsAmount < 0)
            looses += ENEMIES_COUNT[fightsAlive] + WarriorsAmount;
        else
            looses += ENEMIES_COUNT[fightsAlive];
    }

    /// <summary>
    /// Привлечение 0, 1 или 2 новых повстанца
    /// </summary>
    /// <returns></returns>
    private int RandomAgitate()
    {
        System.Random r = new();
        double rNum = r.NextDouble();
        // шанс привлечения 1 повстанца составляет 75%
        if (rNum <= 0.75f)
            return 1;
        // шанс не привлечения ни одного повстанца составляет 20%
        else if (rNum <= 0.95f)
            return 0;
        // шанс привлечения 2 повстанцев составляет 5%
        return 2;
    }

    /// <summary>
    /// Обновление статистики игры
    /// </summary>
    private void UpdatePauseStats()
    {
        PauseStatsText.text = $"{fightsAlive}\n" +
                    $"{usedLeaflets}\n" +
                    $"{WarriorsAmount + PrintersAmount}\n" +
                    $"{looses}";
    }

    /// <summary>
    /// Пересчет статистики игры
    /// </summary>
    private void RecountStats()
    {
        WarriorsAmount -= enemiesAmount;

        if (LeafletsAmount != 0)
        {
            LeafletsAmount -= detectivesAmount * 2;
            if (LeafletsAmount < 0)
            {
                LeafletsAmount = 0;
            }

            detectivesAmount += 2;
        }
    }

    private void UpdateWarriorTimerIndex()
    {
        if (WarriorsAmount != 0)
        {
            warriorTimerIndex = (float)(1 + Math.Pow(WarriorsAmount - 1, 0.5f) / 2f);
        }
        else
        {
            warriorTimerIndex = 1;
        }
        WarriorTimer.UpdateSpeedIndex(warriorTimerIndex);
        
    }
}
