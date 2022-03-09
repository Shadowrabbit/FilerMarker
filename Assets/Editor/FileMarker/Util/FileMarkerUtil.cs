// ******************************************************************
//       /\ /|       @file       FileMarkerUtil.cs
//       \ V/        @brief      文件注释工具类
//       | "")       @author     Shadowrabbit, yue.wang04@mihoyo.com
//       /  |                    
//      /  \\        @Modified   2022-03-08 04:13:52
//    *(__\_\        @Copyright  Copyright (c)  2022, Shadowrabbit
// ******************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace FileMarker
{
    public static class FileMarkerUtil
    {
        public static bool IsEnable
        {
            get => EditorPrefs.GetBool(FileMarkerDef.SwitchKey);
            private set
            {
                EditorPrefs.SetBool(FileMarkerDef.SwitchKey, value);
                EditorApplication.RepaintProjectWindow();
                EditorApplication.RepaintHierarchyWindow();
            }
        }

        /// <summary>
        /// 设置启用
        /// </summary>
        [MenuItem("FileMarker/Enable")]
        private static void SetEnable()
        {
            IsEnable = true;
        }

        /// <summary>
        /// 设置禁用
        /// </summary>
        [MenuItem("FileMarker/Disable")]
        private static void SetDisable()
        {
            IsEnable = false;
        }

        /// <summary>
        /// 启用按钮状态
        /// </summary>
        /// <returns></returns>
        [MenuItem("FileMarker/Enable", true)]
        private static bool EnableCheck()
        {
            return !IsEnable;
        }

        /// <summary>
        /// 禁用按钮状态
        /// </summary>
        /// <returns></returns>
        [MenuItem("FileMarker/Disable", true)]
        private static bool DisableCheck()
        {
            return IsEnable;
        }

        [MenuItem("FileMarker/CleanData")]
        private static void Clean()
        {
            //递归遍历Assets下全部非meta文件
            var allAssetPathList = GetAssetPathsAndFolders("Assets");
            var hashSet = new HashSet<string>();
            foreach (var assetName in allAssetPathList.Select(Path.GetFileNameWithoutExtension)
                .Where(assetName => !hashSet.Contains(assetName)))
            {
                hashSet.Add(assetName);
            }

            var fileMarkerModel = AssetDatabase.LoadAssetAtPath<FileMarkerModel>(FileMarkerDef.DataFilePath);
            //设置文件存在
            if (fileMarkerModel == null || fileMarkerModel.fileMarkerDataList == null)
            {
                Debug.LogError("FileMarker 数据文件不存在 无法清理");
                return;
            }

            var fileMarkerDataList = fileMarkerModel.fileMarkerDataList;
            for (var i = fileMarkerDataList.Count - 1; i >= 0; i--)
            {
                var key = fileMarkerDataList[i].assetName;
                if (!hashSet.Contains(key))
                {
                    fileMarkerDataList.RemoveAt(i);
                }
            }

            Debug.Log("FileMarker 废弃数据清理完毕");
        }

        /// <summary>
        /// 获取某个目录下全部子目录和文件路径
        /// </summary>
        /// <param name="modelFolder"></param>
        /// <returns></returns>
        private static List<string> GetAssetPathsAndFolders(string modelFolder)
        {
            //全部目录
            var allFolders = GetAllFolders(modelFolder);
            var assetPathList = new List<string>();
            foreach (var filePaths in allFolders.Select(Directory.GetFiles))
            {
                assetPathList.AddRange(filePaths.Where(t => !t.EndsWith(".meta")));
            }

            assetPathList.AddRange(allFolders);
            return assetPathList;
        }

        /// <summary>
        /// 获取全部目录名称列表
        /// </summary>
        /// <param name="modelFolder"></param>
        /// <returns></returns>
        private static List<string> GetAllFolders(string modelFolder)
        {
            var allDirs = new List<string>
            {
                GetLocalFolderOrPath(modelFolder)
            };
            var subDirList = GetSubFolders(modelFolder);
            allDirs.AddRange(subDirList);
            return allDirs;
        }

        /// <summary>
        /// 递归获取所有子文件夹目录 不包含自身
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        private static IEnumerable<string> GetSubFolders(string folder)
        {
            var subFolders = new List<string>();
            if (string.IsNullOrEmpty(folder))
            {
                return subFolders;
            }

            var folderArray = Directory.GetDirectories(folder);
            if (folderArray.Length <= 0)
            {
                return subFolders;
            }

            subFolders.AddRange(folderArray.Select(GetLocalFolderOrPath));
            foreach (var t in folderArray)
            {
                var list = GetSubFolders(t);
                subFolders.AddRange(list);
            }

            return subFolders;
        }

        /// <summary>
        /// 获取本地目录
        /// </summary>
        /// <param name="fullFolder"></param>
        /// <returns></returns>
        private static string GetLocalFolderOrPath(string fullFolder) // 路径会含有反斜杠 所以全部替换掉
        {
            if (string.IsNullOrEmpty(fullFolder))
            {
                return string.Empty;
            }

            fullFolder = fullFolder.Replace("\\", "/");
            var index = fullFolder.IndexOf("Assets/", StringComparison.CurrentCultureIgnoreCase);
            return index < 0 ? fullFolder : fullFolder.Substring(index);
        }
    }
}