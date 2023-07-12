using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Element : MonoBehaviour
{
    private UI_Game ui; // ������ �� ui
    public GameObject puskGo; // ������ �� ������, ������� ������ ������������ ��� ���� ��������� ����� ������� ������ "����"
    // ����� ��������
    public int number;
    // ��������� ����� ��� �������������
    public int [] waitNumber;
    // �������� ������ �� �������
    public bool isEmpty = false;
    // ����� �� Y ��� ������� ����� ������
    public float ElementSdvigY;
    // ������ ��������, ������������ ����� ������
    public PartRocket[] partRocket;
    private void Start()
    {
        // ������ ������ ������� � ������ �� �����
        partRocket = new PartRocket[] { new PartRocket(transform.GetChild(0),ElementSdvigY, number) };
        // �������� ������ �� Ui ����� � ������ ����� �����
        ui = GameObject.Find("UI").GetComponent<UI_Game>(); 
    }
    // ������������ �����������
    private void OnTriggerEnter(Collider other)
    {
        // �������� ������ ������������� � ����
        Element element = other.GetComponent<Element>();
        // ��������� ���� �� � ������� ����� ������ � �� �� ������
        if (element != null && !element.isEmpty)
        {
            Debug.Log(element.number + " " + number);
            // ���������� ������ ��������� ����� � ����� � ��������
            if (waitNumber.Length != 0 && waitNumber[0] == element.number)
            {
                ui.AudioPlay((int)EnumAudio.AddedPart);
                while (element.partRocket.Length > 0) // ��������� ���� �� ������ ������� � ���������, ���� ���
                {
                    AddPart(element.partRocket[0]); // ������������ ����� ������ ������� �������� � ����� ��������
                    element.partRocket = SdvigListLeft(element.partRocket); // ������� ��� ����� ������ �� ������ � ������� ��������
                }
                if (element.waitNumber.Length > 0) // ������� ���������� ��������� ����� � �������
                {
                    int prevCount = waitNumber.Length;
                    Array.Resize(ref waitNumber, prevCount + element.waitNumber.Length); // �������� ������ ������ ������� �� ����������������
                    for (int i = 0; i < element.waitNumber.Length; i++)
                    {
                        waitNumber[prevCount + i] = element.waitNumber[i]; // �������� �������� ������� ������� � ���
                    }
                }
                waitNumber = SdvigListLeft(waitNumber); // �������� ������ �����
                element.Empty(); // ������ ������ ������� �� ����� ������
            }
        }
        if (number == 4 && partRocket.Length == 4) // ���� ��� ������� �� ����������
        {
            ui.Win();
        }
    }

    // ���������� ����� ������ � ����� �������
    private void AddPart(PartRocket part)
    {
        Array.Resize(ref partRocket, partRocket.Length + 1); // ���������� ������� �������
        partRocket[partRocket.Length - 1] = part; // ������� ������ �������� � ����� �������
        part.transform.parent = transform; // ����������� ���������������� ������� � ������ transform
        UpdatePartsPosition(); // ��������� ������� ������ ������
    }

    // ���������� ������� ������ ������ � ���� ��������
    private void UpdatePartsPosition()
    {
        if (partRocket.Length < 2) return; // ������, ���� ������ ����

        for (int i = 1; i < partRocket.Length; i++) // ��������� ����� ������ ����� ��������, ������� �� �������
        {
            float currentSdvigY = partRocket[0].transform.localPosition.y; // ����� ��� �������� i. ������ ��� ������ ������� ��������� ������� ������� ��������
            for (int j = 0; j < i; j++) // ����� ��������� ����� ������
            {
                if (partRocket[j].number > partRocket[i].number) // �������, ����� ����� ����� ��� ����, ��� � i
                {
                    currentSdvigY += partRocket[j].sdvigY; // ���������� ����� ���� ����� � ������, ������� ����� i
                }
            }
            partRocket[i].transform.localPosition = new Vector3(0, currentSdvigY, 0); // ����������� �����
            partRocket[i].transform.localRotation = Quaternion.Euler(-90, 0, 0); // �������� � ����������� ��������
        }
    }

    public void Empty()
    {
        
        waitNumber = new int[0]; // �������� ������, ���� �� �����
        isEmpty = true;
    }

    // ����� ��� ������ ������� �����
    private T [] SdvigListLeft<T>(T [] list)
    {
        //��������� ��� �������� �������
        for (int i = 0; i < list.Length; i++)
        {
            if (i + 1 < list.Length) // ���� ��������� ������� ����������
                list[i] = list[i + 1]; // �������� ��� �����
        }
        // ��������� ������ ������� �� 1
        Array.Resize(ref list, list.Length - 1);
        return list;
    }
}

public class PartRocket
{
    // ����� �����
    public int number;
    // transform ����� ������
    public Transform transform;
    // ����� �� Y ������������ �������� +1
    public float sdvigY;

    // ����������� � transform � �������
    public PartRocket(Transform _transform, float _sdvigY, int _number)
    {
        transform = _transform;
        sdvigY = _sdvigY;
        number = _number;
    }

}
