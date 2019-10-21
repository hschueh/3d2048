using System;
public class Constants
{
    #if UNITY_ANDROID
        static public string appId = "ca-app-pub-3940256099942544~3347511713";
        static public string bannerId = "ca-app-pub-3940256099942544/6300978111";
    #elif UNITY_IPHONE
        static public string appId = "ca-app-pub-3940256099942544~1458002511";
        static public string bannerId = "ca-app-pub-3940256099942544/2934735716";
    #else
        static public string appId = "unexpected_platform";
        static public string bannerId = "unexpected_platform";
    #endif
}