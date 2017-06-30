using System;
using UnityEngine;

public class Date
{
    private static int Seasons = 4;
    private static int Days = 365;
    private static int Hours = 24;
    private static int Minutes = 60;
    private static int Seconds = 60;

    /// <summary>
    /// seconds in year (3156000)
    /// </summary>
    public static int Year = 31536000; //Seconds in a year 14400
    public static int Season = Year/4;
    public static int Day = 86400;
    /// <summary>
    /// 3600s
    /// </summary>
    public static int Hour = 3600;
    public static int Minute = 60;

    public float time;
    public float deltaTime;
    public int year;
    public int season;
    public int day;
    public int hour;
    public int minute;
    private float seconds;

    public Date(float _time)
    {
        UpdateDate(_time);
    }

    public void AddTime(float _time)
    {
        deltaTime = _time;
        time += _time;

        seconds += _time;
        if (seconds > Seconds)
        {
            minute += Mathf.FloorToInt(seconds / Seconds);
            seconds = seconds % Seconds;

            if (minute > Minutes) //Setting the Add Time parts
            {
                hour += Mathf.FloorToInt(minute / Minutes);
                minute = minute % Minutes;

                if (hour > Hours)
                {
                    day += Mathf.FloorToInt(hour / Hours);
                    hour = hour % Hours;

                    season = Mathf.FloorToInt(day / (Days/4));

                    if (day > Days)
                    {
                        year += Mathf.FloorToInt(day / Days);
                        day = day % Days;

                    }
                }
            }
        }
        



    }

    internal void UpdateDate(float _time)
    {
        time = _time;
        deltaTime = 0;

        year = Mathf.FloorToInt(_time / Year);
        _time = _time % Year;
        season = Mathf.FloorToInt(_time / Season);
        day = Mathf.FloorToInt(_time / Day);
        _time = _time % Day;
        hour = Mathf.FloorToInt(_time / Hour);
        _time = _time % Hour;
        minute = Mathf.FloorToInt(_time / Minute);
        _time = _time % Minute;
        seconds = _time;
    }

    public string GetDate()
    {
        return year + " Year(s), " + (day + 1) + " Day(s)";
    }

    public string GetDateTime()
    {
        string dateTime = String.Format("{0} - {1}:{2}:{3}\nDay {4}, year {5}",
            GetSeason(),
            hour.ToString().PadLeft(2, '0'),
            minute.ToString().PadLeft(2, '0'),
            seconds.ToString("0").PadLeft(2, '0'),
            day + 1,
            year);
        return dateTime;
    }
    //public string GetDate(float _time)
    //{
    //    float year = Mathf.FloorToInt(_time / Year);
    //    _time = _time % Year;
    //    float season = Mathf.FloorToInt(_time / Season);
    //    float day = Mathf.FloorToInt(_time / Day);
    //    _time = _time % Day;
    //    float hour = Mathf.FloorToInt(_time / Hour);
    //    _time = _time % Hour;

    //    return year.ToString() + "/" + season.ToString() + "/" + day.ToString() + " " + hour.ToString() + " h";
    //}
    /// <summary>
    /// Returns a human readable version of duration of time
    /// </summary>
    /// <param name="time">Given in seconds</param>
    /// <returns></returns>
    public static string ReadTime(double time)
    {
        if (time < 90)
        {
            return time + " s";
        }
        else if (time < Date.Hour)
        {
            return (time / Date.Minute).ToString("0.0") + " minutes";
        }

        else if (time < Date.Day)
        {
            return (time / Date.Hour).ToString("0.00") + " hours";
        }
        else if (time < Date.Year)
        {
            return (time / Date.Day).ToString("0.0") + " days";
        }
        else
        {
            return (time / Date.Year).ToString("0.00") + " years";
        }
    }
    private string GetSeason()
    {
        string[] seasonNames = new string[4] { "Spring", "Summer", "Fall", "Winter" };
        return seasonNames[season];
    }

    public static Date operator -( Date date1, Date date2)
    {
        float diff = date1.time - date2.time;

        Date finalDate = new Date(diff);

        return finalDate;

    }

    public static bool operator >(Date date1, Date date2)
    {
        return (date1.time > date2.time) ? true:false;
    }

    public static bool operator <(Date date1, Date date2)
    {
        return (date1.time < date2.time) ? true : false;
    }

    public static bool operator >=(Date date1, Date date2)
    {
        return (date1.time >= date2.time) ? true : false;
    }
    public static bool operator <=(Date date1, Date date2)
    {
        return (date1.time <= date2.time) ? true : false;
    }
}
