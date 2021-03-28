using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimeManager : MonoBehaviour
{
    public Player player;

    public GameObject sun;

    public TextMeshProUGUI timetext;

    public float dayCycleSpeed;

    private int day;

    private float time;

    private const float seconds= 1;
    private const float minutes = 60 * seconds;
    private const float hours = 60 * minutes;
    private const float days = 24 * hours;

    private const float DEGREES_PER_SECOND = 360 / days;

    private float _degreeRotation;

    private float fixedDeltaTime;

    public void ChangeTimeScale(float a)
    {
        Time.timeScale = a;
        //Time.fixedDeltaTime = this.fixedDeltaTime * Time.timeScale;
    }

    void Awake() 
    {
        this.fixedDeltaTime = Time.fixedDeltaTime;
    }

    void Start() 
    {
        day = 1;
        time = days / 4;
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime * dayCycleSpeed;
        sun.transform.rotation = Quaternion.Euler((time / days) * 360, 0, 0);

        timetext.text = "Day " + day.ToString() + string.Format(", {0:00}:{1:00}", time / hours, Mathf.Floor(time / 60) % 60);

        if (time >= days)
        {
            day++;
            time = 0;
            player.PayRent();
        }
    }
}
