using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private static int[] ENEMIES_COUNT = { 0, 0, 1, 2, 3, 5, 7, 10, 14, 15, 20, 21 };       // ���-�� ������ � �����
    private static int WINNING_REBELS_AMOUNT = 15;

    [SerializeField] Button AddWarriorButton;                   // ������ �������� �����
    [SerializeField] Button AddPrinterButton;                   // ������ �������� ������������

    [SerializeField] TextMeshProUGUI RebelsStatsAmountText;     // ����� ������ ����������
    [SerializeField] TextMeshProUGUI GameStatsAmountText;       // ����� ������ ����
    [SerializeField] TextMeshProUGUI PomidoroStatsText;         // ����� ������ ������

    private int fightsAlive;                                    // ����� ��������� ����
    [SerializeField] int LeafletsAmount;                        // ����� ��������
    [SerializeField] int WarriorsAmount;                        // ����� ������
    [SerializeField] int PrintersAmount;                        // ����� �������������

    [SerializeField] Timer LeafletsTimer;                       // ������ ������ ��������
    [SerializeField] Timer FightTimer;                          // ������ �� ������������
    [SerializeField] ButtonTimer WarriorTimer;                  // ������ ���������� �����
    [SerializeField] ButtonTimer PrinterTimer;                  // ������ ���������� ������������

    [SerializeField] GameObject ResultPanel;                    // ������ ���������� ����
    [SerializeField] Image ResultImage;                         // �������� ����������
    [SerializeField] TextMeshProUGUI ResultText;                // ����� ����������
    [SerializeField] TextMeshProUGUI StatsText;                 // ���������� ����
    [SerializeField] TextMeshProUGUI DescriptionText;           // ����� �������� ���������� ����

    [SerializeField] Sprite WinSprite;
    [SerializeField] Sprite LooseSprite;

    [SerializeField] AudioSource WinAudioSource;
    [SerializeField] AudioSource LostAudioSource;
    [SerializeField] AudioSource ThemeAudioSource;

    [SerializeField] TextMeshProUGUI PauseStatsText;            // ����� ������������� ����������

    private int usedLeaflets;                                   // ���-�� �������������� ��������

    private float warriorTimerIndex;                           // ������ �������� ������� �������� �����
    private int enemiesAmount;                                  // ���-�� ������
    private int detectivesAmount;                               // ���-�� ����������
    private int looses;                                         // ���-�� ������


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
        // ���� �������� ����������
        if (LeafletsTimer.TaskCompleted)
        {
            LeafletsAmount += PrintersAmount;
            UpdateGameStatsAmountText();
        }
        // ���� ��������� ������������
        if (FightTimer.TaskCompleted)
        {
            RecountStats();
            UpdateRebelsStatsAmountText();
            CountLooses();

            // ����� �����������, ���� ���������� ������ ������, ��� ���������� ������
            if (WarriorsAmount < 0)
            {
                Time.timeScale = 0f;

                ResultText.text = "���������";
                DescriptionText.text = "��������� ���� ���������.";
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
                    // ����� �����������, ���� � 10 ����� ��� ������ �� ����������� ������ ������� �� 15
                    Time.timeScale = 0f;

                    ResultText.text = "���������";
                    DescriptionText.text = "��������� ���� ���������.";
                    StatsText.text = PauseStatsText.text;
                    ResultImage.sprite = LooseSprite;

                    ThemeAudioSource.Stop();
                    LostAudioSource.Play();

                    ResultPanel.SetActive(true);
                }
                else
                {
                    // ����� ���������, ���� ��������� ���������� ���������� ����� WINNING_REBELS_AMOUNT � �������� 10 ����
                    Time.timeScale = 0f;

                    ResultText.text = "������";
                    DescriptionText.text = "�� ����������� �������!";
                    StatsText.text = PauseStatsText.text;
                    ResultImage.sprite = WinSprite;

                    ThemeAudioSource.Stop();
                    WinAudioSource.Play();

                    ResultPanel.SetActive(true);
                }
            }
            // � ����� ������ ������ ���� ������������

            UpdateWarriorTimerIndex();

            fightsAlive++;
            enemiesAmount = ENEMIES_COUNT[fightsAlive];
            UpdatePomidoroStatsAmountText();
            UpdateGameStatsAmountText();
        }
        // ���� ��������� �����������
        if (PrinterTimer.TaskCompleted)
        {
            PrintersAmount += RandomAgitate();
            AddPrinterButton.enabled = true;
            UpdateRebelsStatsAmountText();
            UpdatePauseStats();
        }
        // ���� ��������� ����
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
    /// ��������� �����, ������������ ���������� ��������� ������������ � ���������������� ��������
    /// </summary>
    private void UpdateGameStatsAmountText()
    {
        GameStatsAmountText.text =
            $"{fightsAlive}\n" +
            $"{LeafletsAmount}\n";
        UpdatePauseStats();
    }

    /// <summary>
    /// ��������� �����, ������������ ���������� ������ � �������������
    /// </summary>
    private void UpdateRebelsStatsAmountText()
    {
        RebelsStatsAmountText.text =
            $"{WarriorsAmount}\n" +
            $"{PrintersAmount}\n";
    }

    /// <summary>
    /// ��������� �����, ������������ ���������� ������ � ����������� ��� ��������� ������������
    /// </summary>
    private void UpdatePomidoroStatsAmountText()
    {
        PomidoroStatsText.text =
            $"{enemiesAmount}\n" +
            $"{detectivesAmount}\n";
    }

    /// <summary>
    /// ��������� ������� ����������� ����� ������
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
    /// ��������� ������� ����������� ����� �������������
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
    /// ������������ ������ ��� ������� ������
    /// </summary>
    /// <param name="fights">���������� ������������ ������������</param>
    /// <returns></returns>
    private void CountLooses()
    {
        if (WarriorsAmount < 0)
            looses += ENEMIES_COUNT[fightsAlive] + WarriorsAmount;
        else
            looses += ENEMIES_COUNT[fightsAlive];
    }

    /// <summary>
    /// ����������� 0, 1 ��� 2 ����� ���������
    /// </summary>
    /// <returns></returns>
    private int RandomAgitate()
    {
        System.Random r = new();
        double rNum = r.NextDouble();
        // ���� ����������� 1 ��������� ���������� 75%
        if (rNum <= 0.75f)
            return 1;
        // ���� �� ����������� �� ������ ��������� ���������� 20%
        else if (rNum <= 0.95f)
            return 0;
        // ���� ����������� 2 ���������� ���������� 5%
        return 2;
    }

    /// <summary>
    /// ���������� ���������� ����
    /// </summary>
    private void UpdatePauseStats()
    {
        PauseStatsText.text = $"{fightsAlive}\n" +
                    $"{usedLeaflets}\n" +
                    $"{WarriorsAmount + PrintersAmount}\n" +
                    $"{looses}";
    }

    /// <summary>
    /// �������� ���������� ����
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
