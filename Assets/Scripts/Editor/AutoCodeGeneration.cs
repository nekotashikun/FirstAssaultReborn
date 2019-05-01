using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditorInternal;
using System.IO;
using System.Text;
using System.Collections.Generic;

[InitializeOnLoad]
public static class AutoCodeGeneration
{

    // an array that hold all tags
    private static string[] _tags;
    // a flag if the dataset has changed
    private static bool _tagsHasChanged = false;
    // an array that hold all layers
    private static KeyValuePair<string, int>[] _layers;
    // a flag if the dataset has changed
    private static bool _layersHasChanged = false;
    // an array that hold all axis
    private static string[] _axes;
    // a flag if the dataset has changed
    private static bool _axesHasChanged = false;
    // time when we start to count
    private static double _startTime = 0.0;
    // the time that should elapse between the change of tags and the File write
    // this is importend because changed are triggered as soon as you start typing and this can cause lag
    private static double _timeToWait = 5.0;

    static AutoCodeGeneration()
    {
        //subscripe to event
        EditorApplication.update += Update;
        // get tags
        _tags = InternalEditorUtility.tags;
        // get layers
        _layers = getLayers();
        // get axes
        _axes = getAxes();
        // write file
        WriteCodeFile();

    }

    private static KeyValuePair<string, int>[] getLayers()
    {
        List<KeyValuePair<string, int>> layerList = new List<KeyValuePair<string, int>>();
        KeyValuePair<string, int> layer;
        for (int i = 0; i < 32; i++)
        {
            layer = new KeyValuePair<string, int>(LayerMask.LayerToName(i), i);
            if (layer.Key.Length > 0)
            {
                layerList.Add(layer);
            }
        }
        return layerList.ToArray();
    }

    private static string[] getAxes()
    {
        SerializedObject database = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset"));

        List<string> axes = new List<string>();

        SerializedProperty axisArray = database.FindProperty("m_Axes");
        if (axisArray.arraySize == 0) { Debug.LogWarning("No Axes"); return null; }

        for (int i = 0; i < axisArray.arraySize; i++)
        {
            SerializedProperty axis = axisArray.GetArrayElementAtIndex(i);

            string name = axis.FindPropertyRelative("m_Name").stringValue;

            axes.Add(name);
        }

        return axes.ToArray();
    }

    private static void Update()
    {
        // returns if we are in play mode
        if (Application.isPlaying == true)
            return;

        Wait();

        // temp array that hold new tags
        string[] newTags = InternalEditorUtility.tags;
        // check if the lenght is not the same
        if (newTags.Length != _tags.Length)
        {
            _tags = newTags;
            _tagsHasChanged = true;
            _startTime = EditorApplication.timeSinceStartup;
            //return;
        }
        else
        {
            // loop thru all new tags and compare them to the old ones
            for (int i = 0; i < newTags.Length; i++)
            {
                if (string.Equals(newTags[i], _tags[i]) == false)
                {
                    _tags = newTags;
                    _tagsHasChanged = true;
                    _startTime = EditorApplication.timeSinceStartup;
                    //return;
                }
            }
        }

        KeyValuePair<string, int>[] newLayers = getLayers();

        if (newLayers.Length != _layers.Length)
        {
            _layers = newLayers;
            _layersHasChanged = true;
            _startTime = EditorApplication.timeSinceStartup;
            //return;
        }
        else
        {
            // loop thru all new layers and compare them to the old ones
            for (int i = 0; i < newLayers.Length; i++)
            {
                if (string.Equals(newLayers[i].Key, _layers[i].Key) == false)
                {
                    _layers = newLayers;
                    _layersHasChanged = true;
                    _startTime = EditorApplication.timeSinceStartup;
                    //return;
                }
            }
        }

        string[] newAxes = getAxes();

        if (newAxes.Length != _axes.Length)
        {
            _axes = newAxes;
            _axesHasChanged = true;
            _startTime = EditorApplication.timeSinceStartup;
            //return;
        }
        else
        {
            // loop thru all new axes and compare them to the old ones
            for (int i = 0; i < newAxes.Length; i++)
            {
                if (string.Equals(newAxes[i], _axes[i]) == false)
                {
                    _axes = newAxes;
                    _axesHasChanged = true;
                    _startTime = EditorApplication.timeSinceStartup;
                    //return;
                }
            }
        }
        return;
    }

    private static void Wait()
    {
        // if nothing has changed return
        if (_tagsHasChanged == false && _layersHasChanged == false && _axesHasChanged == false)
            return;

        // if the time delta between now and the last change, is greater than the time we schould wait Than write the file
        if (EditorApplication.timeSinceStartup - _startTime > _timeToWait)
        {
            WriteCodeFile();
            _tagsHasChanged = false;
            _layersHasChanged = false;
            _axesHasChanged = false;
        }
    }


    // writes a file to the project folder
    private static void WriteCodeFile()
    {
        string folderPath = string.Concat("Scripts", Path.DirectorySeparatorChar, "AutoGenerated", Path.DirectorySeparatorChar);
        //		if(!AssetDatabase.IsValidFolder(folderPath)){
        //			AssetDatabase.CreateFolder("Assets","Scripts");
        //			AssetDatabase.CreateFolder("Assets/Scripts","AutoGenerated");
        //		}
        // the path we want to write to

        string path = string.Concat(Application.dataPath, Path.DirectorySeparatorChar, folderPath, "AutoTagsLayers.cs");

        if (File.Exists(path) == true) { File.Delete(path); }

        try
        {
            // opens the file if it allready exists, creates it otherwise
            using (FileStream stream = File.Open(path, FileMode.OpenOrCreate, FileAccess.Write))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    StringBuilder builder = new StringBuilder();
                    builder.AppendLine("// ----- AUTO GENERATED CODE ----- //");
                    builder.AppendLine("namespace UnityEngine{");
                    builder.AppendLine("\tpublic static class Tags{");
                    foreach (string tag in _tags)
                    {
                        builder.AppendLine(string.Format("\t\tpublic static readonly string {0} = \"{1}\";", tag.Replace(" ", ""), tag));
                    }

                    builder.AppendLine("\t}");

                    builder.AppendLine("\tpublic static class Layers{");
                    foreach (KeyValuePair<string, int> layer in _layers)
                    {
                        builder.AppendLine(string.Format("\t\tpublic static readonly int {0} = {1};", layer.Key.Replace(" ", ""), layer.Value));
                    }

                    builder.AppendLine("\t}");
                    builder.AppendLine("}");
                    writer.Write(builder.ToString());
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogException(e);

            // if we have an error, it is certainly that the file is screwed up. Delete to be save
            if (File.Exists(path) == true)
                File.Delete(path);
        }

        // the path we want to write to
        path = string.Concat(Application.dataPath, Path.DirectorySeparatorChar, folderPath, "InputUtils.cs");

        if (File.Exists(path) == true) { File.Delete(path); }

        try
        {
            // opens the file if it allready exists, creates it otherwise
            using (FileStream stream = File.Open(path, FileMode.OpenOrCreate, FileAccess.Write))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    StringBuilder builder = new StringBuilder();
                    builder.AppendLine("// ----- AUTO GENERATED CODE ----- //");
                    builder.AppendLine("using System.Collections.Generic;");
                    builder.AppendLine("namespace UnityEngine\n{");
                    builder.AppendLine("\tpublic static class InputUtils\n\t{");
                    
                    builder.AppendLine("\t\tpublic enum Axes");
                    builder.AppendLine("\t\t{");
                    foreach (string axis in _axes)
                    {
                        builder.AppendLine(string.Format("\t\t\t{0},", axis.Replace(" ", "")));
                    }
                    builder.AppendLine("\t\t}\n");
                    
                    foreach (string axis in _axes)
                    {
                        builder.AppendLine(string.Format("\t\tpublic static readonly string {0}Name = \"{1}\";", axis.Replace(" ", ""), axis));
                        builder.AppendLine(string.Format("\t\tpublic static InputValue {0} = new InputValue(\"{1}\");", axis.Replace(" ", ""), axis));
                    }

                    builder.AppendLine("\t\tpublic static Dictionary<Axes, InputValue> inputs = new Dictionary<Axes, InputValue>");
                    builder.AppendLine("\t\t{");
                    foreach (string axis in _axes)
                    {
                        builder.AppendLine(string.Format("\t\t\t{{ Axes.{0}, {1} }},", axis.Replace(" ", ""), axis.Replace(" ", "")));
                    }
                    builder.AppendLine("\t\t};");

                    builder.AppendLine("\t}");
                    builder.AppendLine("}");
                    writer.Write(builder.ToString());
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogException(e);

            // if we have an error, it is certainly that the file is screwed up. Delete to be save
            if (File.Exists(path) == true)
                File.Delete(path);
        }

        AssetDatabase.Refresh();
    }
}
