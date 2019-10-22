using System;
public class Constants
{
    #if UNITY_ANDROID
        static public string appId = "ca-app-pub-8550386526282187~8134368059";
        static public string bannerId = "ca-app-pub-8550386526282187/6821286384";
    #elif UNITY_IPHONE
        static public string appId = "ca-app-pub-8550386526282187~9505507089";
        static public string bannerId = "ca-app-pub-8550386526282187/5251046234";
    #else
        static public string appId = "unexpected_platform";
        static public string bannerId = "unexpected_platform";
    #endif
}