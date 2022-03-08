// ******************************************************************
//       /\ /|       @file       FileMarkerData.cs
//       \ V/        @brief      文件注释数据结构
//       | "")       @author     Shadowrabbit, yue.wang04@mihoyo.com
//       /  |                    
//      /  \\        @Modified   2022-03-07 08:48:50
//    *(__\_\        @Copyright  Copyright (c)  2022, Shadowrabbit
// ******************************************************************

using System;

namespace FileMarker
{
    [Serializable]
    public class FileMarkerData
    {
        public string assetName; //资源名 不含后缀
        public string mark; //注释

        public FileMarkerData(string assetName, string mark)
        {
            this.assetName = assetName;
            this.mark = mark;
        }

        /// <summary>
        /// 设置数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void SetData(string key, string value)
        {
            assetName = key;
            mark = value;
        }
    }
}