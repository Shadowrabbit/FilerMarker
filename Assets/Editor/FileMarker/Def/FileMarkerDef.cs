// ******************************************************************
//       /\ /|       @file       FileMarkerDef.cs
//       \ V/        @brief      文件标记定义
//       | "")       @author     Shadowrabbit, yue.wang04@mihoyo.com
//       /  |                    
//      /  \\        @Modified   2022-03-07 07:03:12
//    *(__\_\        @Copyright  Copyright (c)  2022, Shadowrabbit
// ******************************************************************

using UnityEngine;

namespace FileMarker
{
    public static class FileMarkerDef
    {
        public const string SwitchKey = "FileMarkerSwitch";
        public const string DataFilePath = "Assets/Editor/FileMarker/Assets/DataFile.asset"; //数据文件路径
        public const string DataFileFolder = "Assets/Editor/FileMarker/Assets/"; //数据文件目录
        public const string DefaultMark = "//"; //默认注释
        public static readonly Color BilibiliPink = new Color(0.984f, 0.447f, 0.6f, 1f);
        public static readonly Color BilibiliBlue = new Color(0.137f, 0.769f, 0.898f, 1f);
    }
}