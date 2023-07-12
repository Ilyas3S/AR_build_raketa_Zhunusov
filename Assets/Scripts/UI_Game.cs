using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class UI_Game : MonoBehaviour
{
    [SerializeField] AudioClip[] audios; // массив звуков
    private AudioSource audioSource;
    
    [SerializeField] TMPro.TextMeshProUGUI timerTMP; // Объявление ссылки для текста дсекундомера
    [SerializeField] TMPro.TextMeshProUGUI maxTimeTMP; // Объявление ссылки для текста максимального времени
    [SerializeField] TMPro.TextMeshProUGUI textEndTMP; // Объявление ссылки для текста, появляющегося при победе и поражении

    GameObject textEndGo; // обьект для конечного текста, ссылка
    public GameObject buttonRestart; // кнопка рестарта, ссылка
    public GameObject buttonPusk; // кнопка пуска ракеты, ссылка
    public GameObject rocket; // ссылка на префаб, который станет родительским для всех элементов ракеты после нажатия кнопки "пуск"
    public Element cosmodrom; // ссылка на скрипт элемента "космодром"

    public Animator animatorRocket; // аниматор рактеты, ссылка

    private float timer = 0; // Обьявление секундомера
    public float maxTime = 120; // Объявление максимального времени
    public float speedRocket; // Скорость ракеты

    private bool isWin = false; // Состояние для победы
    private bool isFail = false; // Состояние для проигрыша
    private bool isAnimCosmodrom = false; // Состояние для анимации открытия космодрома
    private bool isFly = false; // Состояние для полета

    void Start()
    {
        textEndGo = textEndTMP.transform.parent.gameObject;
        textEndGo.SetActive(false);
        buttonRestart.SetActive(false);
        buttonPusk.SetActive(false);

        TimeSpan timeSpan = TimeSpan.FromSeconds(maxTime); // Создание объекта TimeSpan из секунд
        maxTimeTMP.text = "/ " + timeSpan.Minutes.ToString() + ":" + timeSpan.Seconds.ToString("D2"); // Задание текста исходя из расчетов

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
            TimeSpan timeSpan = TimeSpan.FromSeconds(timer); // Создание объекта TimeSpan из секунд
            timerTMP.text = timeSpan.Minutes.ToString() + ":" + timeSpan.Seconds.ToString("D2"); // Задание текста исходя из расчетов

            if (timer >= maxTime)
            {
                timer = 0;
                GameOver();
            }
        } else if (isAnimCosmodrom)
        {
            timer += Time.deltaTime;
            
            if (timer >= 2.12f / 0.6f) // Время пока открывается космодром
            {
                isAnimCosmodrom = false; // Убираем флажок
                timer = 0;
                isFly = true; // Ставим другой флажок
                rocket.GetComponent<Rocket>().SetParticlePlay(true); // Включаем партиклы на ракете
                AudioPlay((int)EnumAudio.NoiseRocket);
            }
        } else if (isFly)
        {
            UpdateFly();
            timer += Time.deltaTime;
            if (timer >= 10) // Время в течение которого будет лететь ракета
            {
                isFly = false;
            }
        }
    }

    void UpdateFly() // При состоянии полета
    {
        if (isFly)
        {
            // Задаем направлеие и скорость движения с возрастанием
            Vector3 motion = speedRocket * Vector3.up * timer / 2 * Time.deltaTime; 
            rocket.transform.Translate(motion); // Перемещаем ракеты на определенное расстояние
        }
    }
    void GameOver() // При поражении
    {
        AudioPlay((int)EnumAudio.Fail); // Включаем фанфары
        isFail = true;
        textEndTMP.text = "Поражение!"; // Меняем текст
        textEndGo.SetActive(true); // Активируем видимость панели с текстом

        buttonRestart.SetActive(true);
    }

    public void Win() // При победе
    {
        AudioPlay((int)EnumAudio.Win); // Включаем успокаивающую музыку
        isWin = true;
        textEndTMP.text = "Победа!"; // Меняем текст
        textEndGo.SetActive(true); // Активируем видимость панели с текстом

        buttonPusk.SetActive(true); // Делаем видимой кнопку пуск
    }

    public void Pusk()
    {
        buttonPusk.SetActive(false); // Убираем видимой кнопку пуск
        buttonRestart.SetActive(true); // Делаем видимой кнопку рестарт

        rocket = Instantiate(rocket, cosmodrom.transform); // Создаем обьект из префаба
        for (int i = 0; i < cosmodrom.partRocket.Length; i++) // Проходимся по всем частям в космодроме
        {
            if (cosmodrom.partRocket[i].number != 4) // Исключаем сам космодром
            {
                cosmodrom.partRocket[i].transform.parent = rocket.transform; // Изменяем родителя для частей на ракету
            }
        }

        animatorRocket.SetBool("isFly", true); // Включаем анимацию космодрома
        timer = 0;
        isAnimCosmodrom = true;
        AudioPlay((int)EnumAudio.StartCosmodrom); // Включаем звук космодрома
    }
    public void ChangeScene(int index) // Изменение сцены
    {
        SceneManager.LoadScene(index); // Загрузка сцены
    }

    // Метод, чтобы пользоваться звуками
    public void AudioPlay(int index)
    {
        audioSource.Stop();
        audioSource.PlayOneShot(audios[index]); // Запускаем звук по индексу из массива
    }
}

// Перечисление со всеми звуками для удобного вызова
public enum EnumAudio
{
    StartCosmodrom = 0, // Звук космодрома
    NoiseRocket = 1, // Звук взлета ракеты
    AddedPart = 2, // Звук при соединении частей
    Win = 3, // Звук победы
    Fail = 4 // Звук поражения
}