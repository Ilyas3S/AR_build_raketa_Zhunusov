using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    public ParticleSystem fire; // ������ �� �����
    public ParticleSystem smog; // ������ �� ���
    void Start()
    {
        
    }
    void Update()
    {
        
    }
    public void SetParticlePlay(bool flag)
    {
        if (flag) // ���� ��, �� �������� ��������
        {
            fire.Play();
            smog.Play();
        }
        else // ���� ���, �� ��������� ��������
        {
            fire.Stop();
            smog.Stop();
        }
    }
}
