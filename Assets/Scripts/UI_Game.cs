using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class UI_Game : MonoBehaviour
{
    [SerializeField] AudioClip[] audios; // ������ ������
    private AudioSource audioSource;
    
    [SerializeField] TMPro.TextMeshProUGUI timerTMP; // ���������� ������ ��� ������ ������������
    [SerializeField] TMPro.TextMeshProUGUI maxTimeTMP; // ���������� ������ ��� ������ ������������� �������
    [SerializeField] TMPro.TextMeshProUGUI textEndTMP; // ���������� ������ ��� ������, ������������� ��� ������ � ���������

    GameObject textEndGo; // ������ ��� ��������� ������, ������
    public GameObject buttonRestart; // ������ ��������, ������
    public GameObject buttonPusk; // ������ ����� ������, ������
    public GameObject rocket; // ������ �� ������, ������� ������ ������������ ��� ���� ��������� ������ ����� ������� ������ "����"
    public Element cosmodrom; // ������ �� ������ �������� "���������"

    public Animator animatorRocket; // �������� �������, ������

    private float timer = 0; // ���������� �����������
    public float maxTime = 120; // ���������� ������������� �������
    public float speedRocket; // �������� ������

    private bool isWin = false; // ��������� ��� ������
    private bool isFail = false; // ��������� ��� ���������
    private bool isAnimCosmodrom = false; // ��������� ��� �������� �������� ����������
    private bool isFly = false; // ��������� ��� ������

    void Start()
    {
        textEndGo = textEndTMP.transform.parent.gameObject;
        textEndGo.SetActive(false);
        buttonRestart.SetActive(false);
        buttonPusk.SetActive(false);

        TimeSpan timeSpan = TimeSpan.FromSeconds(maxTime); // �������� ������� TimeSpan �� ������
        maxTimeTMP.text = "/ " + timeSpan.Minutes.ToString() + ":" + timeSpan.Seconds.ToString("D2"); // ������� ������ ������ �� ��������

        audioSource = GetComponent<AudioSource>();
    }
    
    void Update()
    {
        UpdateTimer();
    }

    void UpdateTimer()
    {
        if (!isWin && !isFail)
        {
            timer += Time.deltaTime;
            TimeSpan timeSpan = TimeSpan.FromSeconds(timer); // �������� ������� TimeSpan �� ������
            timerTMP.text = timeSpan.Minutes.ToString() + ":" + timeSpan.Seconds.ToString("D2"); // ������� ������ ������ �� ��������

            if (timer >= maxTime)
            {
                timer = 0;
                GameOver();
            }
        } else if (isAnimCosmodrom)
        {
            timer += Time.deltaTime;
            
            if (timer >= 2.12f / 0.6f) // ����� ���� ����������� ���������
            {
                isAnimCosmodrom = false; // ������� ������
                timer = 0;
                isFly = true; // ������ ������ ������
                rocket.GetComponent<Rocket>().SetParticlePlay(true); // �������� �������� �� ������
                AudioPlay((int)EnumAudio.NoiseRocket);
            }
        } else if (isFly)
        {
            UpdateFly();
            timer += Time.deltaTime;
            if (timer >= 10) // ����� � ������� �������� ����� ������ ������
            {
                isFly = false;
            }
        }
    }

    void UpdateFly() // ��� ��������� ������
    {
        if (isFly)
        {
            // ������ ���������� � �������� �������� � ������������
            Vector3 motion = speedRocket * Vector3.up * timer / 2 * Time.deltaTime; 
            rocket.transform.Translate(motion); // ���������� ������ �� ������������ ����������
        }
    }
    void GameOver() // ��� ���������
    {
        AudioPlay((int)EnumAudio.Fail); // �������� �������
        isFail = true;
        textEndTMP.text = "���������!"; // ������ �����
        textEndGo.SetActive(true); // ���������� ��������� ������ � �������

        buttonRestart.SetActive(true);
    }

    public void Win() // ��� ������
    {
        AudioPlay((int)EnumAudio.Win); // �������� ������������� ������
        isWin = true;
        textEndTMP.text = "������!"; // ������ �����
        textEndGo.SetActive(true); // ���������� ��������� ������ � �������

        buttonPusk.SetActive(true); // ������ ������� ������ ����
    }

    public void Pusk()
    {
        buttonPusk.SetActive(false); // ������� ������� ������ ����
        buttonRestart.SetActive(true); // ������ ������� ������ �������

        rocket = Instantiate(rocket, cosmodrom.transform); // ������� ������ �� �������
        for (int i = 0; i < cosmodrom.partRocket.Length; i++) // ���������� �� ���� ������ � ����������
        {
            if (cosmodrom.partRocket[i].number != 4) // ��������� ��� ���������
            {
                cosmodrom.partRocket[i].transform.parent = rocket.transform; // �������� �������� ��� ������ �� ������
            }
        }

        animatorRocket.SetBool("isFly", true); // �������� �������� ����������
        timer = 0;
        isAnimCosmodrom = true;
        AudioPlay((int)EnumAudio.StartCosmodrom); // �������� ���� ����������
    }
    public void ChangeScene(int index) // ��������� �����
    {
        SceneManager.LoadScene(index); // �������� �����
    }

    // �����, ����� ������������ �������
    public void AudioPlay(int index)
    {
        audioSource.Stop();
        audioSource.PlayOneShot(audios[index]); // ��������� ���� �� ������� �� �������
    }
}

// ������������ �� ����� ������� ��� �������� ������
public enum EnumAudio
{
    StartCosmodrom = 0, // ���� ����������
    NoiseRocket = 1, // ���� ������ ������
    AddedPart = 2, // ���� ��� ���������� ������
    Win = 3, // ���� ������
    Fail = 4 // ���� ���������
}