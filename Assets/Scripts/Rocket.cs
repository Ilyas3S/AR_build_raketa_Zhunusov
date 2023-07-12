using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    public ParticleSystem fire; // ссылка на огонь
    public ParticleSystem smog; // ссылка на дым
    void Start()
    {
        
    }
    void Update()
    {
        
    }
    public void SetParticlePlay(bool flag)
    {
        if (flag) // Если да, то включаем партиклы
        {
            fire.Play();
            smog.Play();
        }
        else // если нет, то отключаем партиклы
        {
            fire.Stop();
            smog.Stop();
        }
    }
}
