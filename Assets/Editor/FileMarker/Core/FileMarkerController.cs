// ******************************************************************
//       /\ /|       @file       FileMarkerController.cs
//       \ V/        @brief      文件注释 VM层
//       | "")       @author     Shadowrabbit, yue.wang04@mihoyo.com
//       /  |                    
//      /  \\        @Modified   2022-03-07 02:03:46
//    *(__\_\        @Copyright  Copyright (c)  2022, Shadowrabbit
// ******************************************************************

using System.IO;
using UnityEditor;
using UnityEngine;

namespace FileMarker
{
    [InitializeOnLoad]
    public class FileMarkerController : Editor
    {
        private static readonly GUIStyle HierarchyStyle = new GUIStyle();
        private static readonly GUIStyle ProjectMarkedStyle = new GUIStyle();
        private static readonly GUIStyle ProjectWaitMarkStyle = new GUIStyle();
        private static FileMarkerModel _fileMarkerModel;

        static FileMarkerController()
        {
            CheckDataFile();
            HierarchyStyle.alignment = TextAnchor.MiddleRight;
            HierarchyStyle.normal.textColor = FileMarkerDef.BilibiliPink;
            HierarchyStyle.fontSize = 12;
            ProjectMarkedStyle.fontSize = 12;
            ProjectMarkedStyle.normal.textColor = FileMarkerDef.BilibiliPink;
            ProjectWaitMarkStyle.fontSize = 12;
            ProjectWaitMarkStyle.normal.textColor = FileMarkerDef.BilibiliBlue;
            EditorApplication.projectWindowItemOnGUI += ProjectWindowItemOnGUI;
            EditorApplication.hierarchyWindowItemOnGUI += HierarchyWindowItemOnGUI;
        }

        ~FileMarkerController()
        {
            EditorApplication.projectWindowItemOnGUI -= ProjectWindowItemOnGUI;
            EditorApplication.hierarchyWindowItemOnGUI -= HierarchyWindowItemOnGUI;
        }

        /// <summary>
        /// 数据文件校验
        /// </summary>
        private static void CheckDataFile()
        {
            _fileMarkerModel = AssetDatabase.LoadAssetAtPath<FileMarkerModel>(FileMarkerDef.DataFilePath);
            //设置文件存在
            if (_fileMarkerModel != null)
            {
                return;
            }

            Debug.Log("FileMarker 数据文件不存在");
            //检查保存路径
            if (!Directory.Exists(FileMarkerDef.DataFileFolder))
                Directory.CreateDirectory(FileMarkerDef.DataFileFolder);
            //创建资源实例
            _fileMarkerModel = CreateInstance<FileMarkerModel>();
            //创建资源
            AssetDatabase.CreateAsset(_fileMarkerModel, FileMarkerDef.DataFilePath);
            AssetDatabase.Refresh();
            Debug.Log("FileMarker 数据文件已创建");
        }

        private static void HierarchyWindowItemOnGUI(int instanceId, Rect selectionRect)
        {
            if (_fileMarkerModel == null)
            {
                return;
            }

            var obj = EditorUtility.InstanceIDToObject(instanceId) as GameObject;
            if (obj == null)
            {
                return;
            }

            var fileMarkData = _fileMarkerModel.GetFileMarkData(obj.name);
            if (fileMarkData == null)
            {
                return;
            }

            var rect = new Rect(selectionRect.x, selectionRect.y, selectionRect.width, selectionRect.height);
            EditorGUI.LabelField(rect, fileMarkData.mark, HierarchyStyle);
        }

        private static void ProjectWindowItemOnGUI(string guid, Rect selectionRect)
        {
            if (_fileMarkerModel == null)
            {
                return;
            }

            var tempMark = FileMarkerDef.DefaultMark;
            var assetPath = AssetDatabase.GUIDToAssetPath(guid);
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(assetPath);
            var fileMarkData = _fileMarkerModel.GetFileMarkData(fileNameWithoutExtension);
            //当前已有这个key的数据 替换掉默认注释
            if (fileMarkData != null && !fileMarkData.mark.Trim().Equals(""))
            {
                tempMark = fileMarkData.mark;
            }

            var rect = CalcTextFieldRect(selectionRect, tempMark);
            GUI.changed = false;
            tempMark = EditorGUI.TextField(rect, tempMark,
                tempMark.Equals(FileMarkerDef.DefaultMark) ? ProjectWaitMarkStyle : ProjectMarkedStyle);
            //显示层在内存创建临时数据 只有文本被编辑才会序列化 减少存储量
            if (!GUI.changed)
            {
                return;
            }

            if (_fileMarkerModel.Contain(fileNameWithoutExtension))
            {
                _fileMarkerModel.UpdateData(fileNameWithoutExtension, tempMark);
            }
            else
            {
                _fileMarkerModel.AddData(fileNameWithoutExtension, tempMark);
            }

            EditorUtility.SetDirty(_fileMarkerModel);
            AssetDatabase.SaveAssets();
        }

        /// <summary>
        /// 计算编辑文本位置和尺寸
        /// </summary>
        /// <param name="itemRect"></param>
        /// <param name="mark"></param>
        /// <returns></returns>
        private static Rect CalcTextFieldRect(Rect itemRect, string mark)
        {
            var markContent = new GUIContent(mark);
            var fieldSize = ProjectMarkedStyle.CalcSize(markContent);
            var isTreeView = itemRect.height <= 21f;
            //列表项的尺寸与描述在同一行 最多支持显示一半宽度、图标尺寸不在同一行 不超框即可
            var fixWidth = Mathf.Min(fieldSize.x + 4, isTreeView ? itemRect.width / 2 : itemRect.width);
            //project下以列表显示的情况
            if (isTreeView)
            {
                return new Rect
                {
                    x = itemRect.xMax - fixWidth,
                    y = itemRect.yMax - fieldSize.y,
                    width = fixWidth,
                    height = itemRect.height
                };
            }

            return new Rect
            {
                x = itemRect.x,
                y = itemRect.yMax,
                width = fixWidth,
                height = itemRect.height
            };
        }
    }
}