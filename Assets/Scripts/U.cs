using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;
using System;
using TMPro;
using DG.Tweening;


// UTILITIES 
// All is static & public



public static class U 
{
	// get the angle between two vectors (-180, 180)
	public static float AngleSigned(Vector3 v1, Vector3 v2)
    {
    	Vector3 n = Vector3.Cross(v2,v1);

	    return Mathf.Atan2(
	        Vector3.Dot(n, Vector3.Cross(v1, v2)),
	        Vector3.Dot(v1, v2)) * Mathf.Rad2Deg;
	}

	// convert hexadecimal to color
	public static Color HexToColor(string hex)
    {
        hex = hex.Replace ("0x", "");//in case the string is formatted 0xFFFFFF
        hex = hex.Replace ("#", "");//in case the string is formatted #FFFFFF
        byte a = 255;//assume fully visible unless specified in hex
        byte r = byte.Parse(hex.Substring(0,2), System.Globalization.NumberStyles.HexNumber);
        byte g = byte.Parse(hex.Substring(2,2), System.Globalization.NumberStyles.HexNumber);
        byte b = byte.Parse(hex.Substring(4,2), System.Globalization.NumberStyles.HexNumber);
        
        //Only use alpha if the string has enough characters
        if(hex.Length == 8){
            a = byte.Parse(hex.Substring(4,2), System.Globalization.NumberStyles.HexNumber);
        }
        return new Color32(r,g,b,a);
    }

    public static int ToInt(string value)
    {
        return Convert.ToInt32(value);
    }

    public static Color WithAlpha(this Color c, float alpha)
    {
        return new Color(c.r, c.g, c.b, alpha);
    }

    public static SpriteRenderer SetAlpha(this SpriteRenderer sprite, float alpha)
    {
        Color newColorWithAlpha = sprite.color;
        newColorWithAlpha.a = alpha;
        sprite.color = newColorWithAlpha;
        return sprite;
    }

    public static float GetAlpha(this SpriteRenderer sprite)
    {
        return sprite.color.a;
    }

    public static TextMesh SetAlpha(this TextMesh text, float alpha)
    {
        Color newColorWithAlpha = text.color;
        newColorWithAlpha.a = alpha;
        text.color = newColorWithAlpha;
        return text;
    }

     public static Image SetAlpha(this Image image, float alpha)
    {
        Color newColorWithAlpha = image.color;
        newColorWithAlpha.a = alpha;
        image.color = newColorWithAlpha;
        return image;
    }

    public static float GetAlpha(this Image image)
    {
        return image.color.a;
    }

    public static int RandomSign()
    {
        int  number = UnityEngine.Random.Range(-1, 2);
        
        while(number == 0)
        {
            number = UnityEngine.Random.Range(-1, 2);
        }

        return number;
    }

    public static int RealModulo(float val, float val2)
    {
        return Mathf.RoundToInt(val - val2 * Mathf.Floor(val / val2));
    }

    public static string TimeToString(float time)
    {
        int timeInSecondsInt = (int)time;  //We don't care about fractions of a second, so easy to drop them by just converting to an int
        float milliSeconds = (time - timeInSecondsInt)*100f;
        int milliSecondsInt = (int)milliSeconds;
        int minutes = timeInSecondsInt / 60;  //Get total minutes
        int seconds = timeInSecondsInt - (minutes * 60);  //Get seconds for display alongside minutes
        return minutes.ToString("D2") + ":" + seconds.ToString("D2") + ":" + milliSecondsInt.ToString("D2");  //Create the string representation, where both seconds and minutes are at minimum 2 digits
    }

    //calculates a number of points around the guide
    //source for the math part :  https://www.youtube.com/watch?v=bqtqltqcQhw
    public static Vector3[] CreatePointsAroundCenter(Vector3 center, float maxDistance, int amount)
    {

        Vector3[] points = new Vector3[amount];

        if (amount == 1)
        {
            points[0] = center;
            return points;
        }


        float modifier = 1f + 35f / amount;

        float pow = 0.5f;
        float turnFraction = 1.618f; //gloden ratio

        for (int i = 0; i < amount; i++)
        {
            float dst = Mathf.Pow((float)i / (amount - 1f), pow) * maxDistance;
            float angle = 2 * Mathf.PI * turnFraction * i;

            float x = dst * Mathf.Cos(angle);
            float z = dst * Mathf.Sin(angle);


            Vector3 pos = center + x * Vector3.right + z * Vector3.forward;

            points[i] = pos;
        }

        return points;
    }

    
    public static Vector2 WorldToUIPos(Vector3 worldPos, Camera camera, CanvasScaler canvasScaler)
    {

        if (canvasScaler.matchWidthOrHeight != 0) Debug.LogWarning("Canvas scaler matchWidthOrHeight value isnt 0, worldToUIPos will not work properly");
        Vector2 res = canvasScaler.referenceResolution;
        //Vector2 ratio = new Vector2(res.x / canvasScaler.matchWidthOrHeight * Screen.width, res.y / Screen.height);

        //because it's in matchWidth fully !!
        float ratioFloat = res.x / Screen.width;
        Vector2 resPos;


        resPos = camera.WorldToScreenPoint(worldPos);
        resPos *= ratioFloat;

        return resPos;
    }

}
