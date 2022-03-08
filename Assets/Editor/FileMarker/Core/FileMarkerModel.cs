// ******************************************************************
//       /\ /|       @file       FileMarkerData.cs
//       \ V/        @brief      文件注释 模型层
//       | "")       @author     Shadowrabbit, yue.wang04@mihoyo.com
//       /  |                    
//      /  \\        @Modified   2022-03-07 11:45:26
//    *(__\_\        @Copyright  Copyright (c)  2022, Shadowrabbit
// ******************************************************************

using System.Collections.Generic;
using UnityEngine;

namespace FileMarker
{
    [CreateAssetMenu(fileName = "FileMarkerData", menuName = "Create FileMarkerData", order = 1)]
    public class FileMarkerModel : ScriptableObject
    {
        private readonly Dictionary<string, FileMarkerData> _dict = new Dictionary<string, FileMarkerData>(); //全部注释数据
        public List<FileMarkerData> fileMarkerDataList = new List<FileMarkerData>(); //注释数据列表

        private void Awake()
        {
            Fetch();
        }

        private void Reset()
        {
            _dict?.Clear();
            fileMarkerDataList?.Clear();
        }

        /// <summary>
        /// 更新注释
        /// </summary>
        /// <param name="assetName"></param>
        public FileMarkerData GetFileMarkData(string assetName)
        {
            TryFetch();
            return _dict.ContainsKey(assetName) ? _dict[assetName] : null;
        }

        /// <summary>
        /// 为某个文件添加注释
        /// </summary>
        /// <param name="assetName"></param>
        /// <param name="mark"></param>
        public void AddData(string assetName, string mark)
        {
            var fileMarkerData = new FileMarkerData(assetName, mark);
            fileMarkerDataList.Add(fileMarkerData);
            _dict.Add(assetName, fileMarkerData);
        }

        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="assetName"></param>
        /// <param name="mark"></param>
        public void UpdateData(string assetName, string mark)
        {
            TryFetch();
            if (_dict.ContainsKey(assetName))
            {
                _dict[assetName].SetData(assetName, mark);
            }
        }

        /// <summary>
        /// 数据里是否包含某个key
        /// </summary>
        /// <param name="assetName"></param>
        /// <returns></returns>
        public bool Contain(string assetName)
        {
            TryFetch();
            return _dict.ContainsKey(assetName);
        }

        /// <summary>
        /// 尝试同步
        /// </summary>
        private void TryFetch()
        {
            if (_dict.Count == fileMarkerDataList.Count)
            {
                return;
            }

            Fetch();
            Debug.Log($"FileMarker Fetch");
        }

        /// <summary>
        /// 同步数据
        /// </summary>
        private void Fetch()
        {
            _dict.Clear();
            for (var index = 0; index < fileMarkerDataList.Count; index++)
            {
                var fileMarkerData = fileMarkerDataList[index];
                if (!_dict.ContainsKey(fileMarkerData.assetName))
                {
                    _dict.Add(fileMarkerData.assetName, fileMarkerData);
                }
            }
        }
    }
}