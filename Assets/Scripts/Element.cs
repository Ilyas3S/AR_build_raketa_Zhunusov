using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Element : MonoBehaviour
{
    private UI_Game ui; // ссылка на ui
    public GameObject puskGo; // ссылка на объект, который станет родительским для всех элементов после нажатия кнопки "пуск"
    // номер элемента
    public int number;
    // ожидаемый номер для присоединения
    public int [] waitNumber;
    // проверка пустой ли элемент
    public bool isEmpty = false;
    // Сдвиг по Y для задания части ракеты
    public float ElementSdvigY;
    // Массив объектов, обозначающих части ракеты
    public PartRocket[] partRocket;
    private void Start()
    {
        // Вносим первый элемент в массив из детей
        partRocket = new PartRocket[] { new PartRocket(transform.GetChild(0),ElementSdvigY, number) };
        // Добываем ссылку на Ui потом и кровью через сцену
        ui = GameObject.Find("UI").GetComponent<UI_Game>(); 
    }
    // обрабатываем солкновение
    private void OnTriggerEnter(Collider other)
    {
        // получаем скрипт столкнувшийся с нами
        Element element = other.GetComponent<Element>();
        // проверяем есть ли у объекта такой скрипт и мы не пустые
        if (element != null && !element.isEmpty)
        {
            Debug.Log(element.number + " " + number);
            // Сравниваем первое ожидаемое число и число у элемента
            if (waitNumber.Length != 0 && waitNumber[0] == element.number)
            {
                ui.AudioPlay((int)EnumAudio.AddedPart);
                while (element.partRocket.Length > 0) // Проверяем пуст ли другой элемент и повторяем, если нет
                {
                    AddPart(element.partRocket[0]); // Присоединяем часть ракеты другого элемента к этому элементу
                    element.partRocket = SdvigListLeft(element.partRocket); // Убираем эту часть ракеты из списка у другого элемента
                }
                if (element.waitNumber.Length > 0) // Смотрим количество ожидаемых чисел у другого
                {
                    int prevCount = waitNumber.Length;
                    Array.Resize(ref waitNumber, prevCount + element.waitNumber.Length); // Изменяем размер нашего массива до соответствующего
                    for (int i = 0; i < element.waitNumber.Length; i++)
                    {
                        waitNumber[prevCount + i] = element.waitNumber[i]; // Копируем элементы другого массива в наш
                    }
                }
                waitNumber = SdvigListLeft(waitNumber); // Сдвигаем массив влево
                element.Empty(); // Делаем другой элемент до конца пустым
            }
        }
        if (number == 4 && partRocket.Length == 4) // если все собрано на космодроме
        {
            ui.Win();
        }
    }

    // Добавление части ракеты к этому элемнту
    private void AddPart(PartRocket part)
    {
        Array.Resize(ref partRocket, partRocket.Length + 1); // Увеличение размера массива
        partRocket[partRocket.Length - 1] = part; // Вставка нового элемента в конец массива
        part.transform.parent = transform; // прикрепляем соприкоснувшийся элемент к нашему transform
        UpdatePartsPosition(); // Обновляем позиции частей ракеты
    }

    // Обновление позиции частей ракеты в этом элементе
    private void UpdatePartsPosition()
    {
        if (partRocket.Length < 2) return; // Уходим, если частей мало

        for (int i = 1; i < partRocket.Length; i++) // Прогоняем части ракеты этого элемента, начиная со второго
        {
            float currentSdvigY = partRocket[0].transform.localPosition.y; // Сдвиг для элемента i. Задаем для начала текущую локальную позицию первого элемента
            for (int j = 0; j < i; j++) // Опять прогоняем части ракеты
            {
                if (partRocket[j].number > partRocket[i].number) // Смотрим, чтобы номер части был выше, чем у i
                {
                    currentSdvigY += partRocket[j].sdvigY; // Прибавляем сдвиг этой части к сдвигу, который будет i
                }
            }
            partRocket[i].transform.localPosition = new Vector3(0, currentSdvigY, 0); // Присваеваем сдвиг
            partRocket[i].transform.localRotation = Quaternion.Euler(-90, 0, 0); // Приводим к нормальному вращению
        }
    }

    public void Empty()
    {
        
        waitNumber = new int[0]; // Обнуляем массив, чтоб не мешал
        isEmpty = true;
    }

    // метод для сдвига массива влево
    private T [] SdvigListLeft<T>(T [] list)
    {
        //Прогоняем все элементы массива
        for (int i = 0; i < list.Length; i++)
        {
            if (i + 1 < list.Length) // Если следующий элемент существует
                list[i] = list[i + 1]; // Сдвинуть его влево
        }
        // Уменьшить размер массива на 1
        Array.Resize(ref list, list.Length - 1);
        return list;
    }
}

public class PartRocket
{
    // номер части
    public int number;
    // transform части ракеты
    public Transform transform;
    // Сдвиг по Y относительно элемента +1
    public float sdvigY;

    // Конструктор с transform и сдвигом
    public PartRocket(Transform _transform, float _sdvigY, int _number)
    {
        transform = _transform;
        sdvigY = _sdvigY;
        number = _number;
    }

}
