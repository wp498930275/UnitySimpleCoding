using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MathUtility {


#region 经纬度计算

    private static double EARTH_RADIUS = 6378137;//赤道半径(单位m)  

    /** 
     * 转化为弧度(rad) 
     * */
    private static double Rad(double d) {
        return d * Math.PI / 180.0;
    }

    /** 
     * 基于googleMap中的算法得到两经纬度之间的距离,计算精度与谷歌地图的距离精度差不多，相差范围在0.2米以下 
     * @param lon1 第一点的经度 
     * @param lat1 第一点的纬度 
     * @param lon2 第二点的经度 
     * @param lat3 第二点的纬度 
     * @return 返回的距离，单位m 
     * */
    public static double GetDistance(double lon1, double lat1, double lon2, double lat2) {
        double radLat1 = Rad(lat1);
        double radLat2 = Rad(lat2);
        double a = radLat1 - radLat2;
        double b = Rad(lon1) - Rad(lon2);
        double s = 2 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin(a / 2), 2) + Math.Cos(radLat1) * Math.Cos(radLat2) * Math.Pow(Math.Sin(b / 2), 2)));
        s = s * EARTH_RADIUS;
        s = Math.Round(s * 100) / 100;
        return s;
    }

    //获取当前位置经度1度的距离
    public static double GetDistancePerLongitude(double lon, double lat) {
        double newLon = lon + 1;
        return GetDistance(lon, lat, newLon, lat);
    }

    //获取当前位置纬度1度的距离
    public static double GetDistancePerLatitude(double lon, double lat) {
        double absLat = Math.Abs(lat);
        double newLat = (absLat > 45) ? absLat - 1 : absLat + 1;
        return GetDistance(lon, lat, lon, newLat);
    }

    #endregion

    /// <summary>
    /// AngleBetween - the angle between 2 vectors
    /// </summary>
    /// <returns>
    /// Returns the the angle in degrees between vector1 and vector2
    /// </returns>
    /// <param name="vector1"> The first Vector </param>
    /// <param name="vector2"> The second Vector </param>
    public static double AngleBetween(Vector2 vector1, Vector2 vector2) {
        double sin = vector1.x * vector2.y - vector1.y * vector2.x; 
        double cos = vector1.x * vector2.x + vector1.y * vector2.y;
        return Math.Atan2(sin, cos) * (180 / Math.PI);
    }

}
