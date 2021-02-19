using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

public class ChromosomeEditor : EditorWindow
{
    private enum View { GeneList, Gene, ChromosomeList }

    private Vector2 scroll = new Vector2();
    private int currentIndex = -1;
    private int chromosomeIndex = -1;
    private View view;

    private ChromosomeData chromosomeData;

    public const string PathToDataAssets = "Assets/Genetic Algorithm/Data";

    [MenuItem("Genetic Algorithm/Show Chromosome Editor")]
    public static void ShowChromosomeEditor()
    {
        GetWindow(typeof(ChromosomeEditor));
    }

#if UNITY_EDITOR

    /// <summary>
    /// Adds newly found assets in folder to list
    /// Returns how many found (not how many added)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="path"></param>
    /// <param name="assetsFound">Adds to this list if it is not already there</param>
    /// <returns></returns>
    public static void TryGetUnityObjectsOfTypeFromPath<T>(string path, List<T> assetsFound) where T : UnityEngine.Object
    {
        string[] filePaths = System.IO.Directory.GetFiles(path);

        //Debug.Log(filePaths.Length);

        if (filePaths != null && filePaths.Length > 0)
        {
            for (int i = 0; i < filePaths.Length; i++)
            {
                UnityEngine.Object obj = UnityEditor.AssetDatabase.LoadAssetAtPath(filePaths[i], typeof(T));
                if (obj is T asset)
                {
                    if (!assetsFound.Contains(asset))
                    {
                        assetsFound.Add(asset);
                    }
                }
            }
        }
    }

    public static ChromosomeData CreateNewChromosome()
    {
        int attempts = 2;
        int maxAttempts = 100;
        string PathToAsset = "Assets/Genetic Algorithm/Data/ChromosomeData.asset";
        ChromosomeData newData = AssetDatabase.LoadAssetAtPath<ChromosomeData>(PathToAsset);
        while (attempts < maxAttempts)
        {          
            if (newData == null)
            {
                Debug.Log("data set to default!");
                newData = CreateInstance<ChromosomeData>();
                AssetDatabase.CreateAsset(newData, PathToAsset);
                Debug.Log(newData);
                return newData;
            }

            PathToAsset = "Assets/Genetic Algorithm/Data/ChromosomeData" + attempts.ToString() + ".asset";
            newData = AssetDatabase.LoadAssetAtPath<ChromosomeData>(PathToAsset);
            attempts++;
        }
        Debug.Log("<color=orange>Attempts to create assets exceeded: </color>" + maxAttempts);
        return newData;
    }

    public static ChromosomeData SetDefaultChromsome()
    {
        string _pathToAsset = "Assets/Genetic Algorithm/Data/ChromosomeData.asset";

        ChromosomeData newData = AssetDatabase.LoadAssetAtPath<ChromosomeData>(_pathToAsset);
        if (newData == null)
        {
            Debug.Log("data set to default!");
            newData = CreateInstance<ChromosomeData>();
            AssetDatabase.CreateAsset(newData, _pathToAsset);
            Debug.Log(newData);
        }

        return newData;
    }
#endif

    void OnGUI()
    {
        //Try set data to default
        if (chromosomeData == null)
            chromosomeData = SetDefaultChromsome();

        // if returned data is null, do not continue.
        if (chromosomeData == null)
        {
            Debug.LogWarning(this + "'s data == " + chromosomeData);
            return;
        }
        SerializedObject dataObj = new SerializedObject(chromosomeData);

        SerializedProperty geneList = dataObj.FindProperty("genes");

        EditorGUILayout.BeginVertical();
        scroll = EditorGUILayout.BeginScrollView(scroll);

        if (view == View.Gene && currentIndex != -1)
        {
            OnGUI_GeneView(geneList, currentIndex);
        }
        else if (view == View.ChromosomeList)
        {
            //Declare a list to store all found story data
            List<ChromosomeData> allData = new List<ChromosomeData>();

            //All StoryData that can be found in PathToDataAssets
            TryGetUnityObjectsOfTypeFromPath(PathToDataAssets, allData);

            OnGUI_ChromosomeView(allData);
        }
        else
        {
            OnGUI_ListView(geneList);
        }

        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();

        dataObj.ApplyModifiedProperties();
    }

#if UNITY_EDITOR

#endif

    private void OnGUI_ListView(SerializedProperty genesList)
    {
        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField(chromosomeData.name + " genes: ", EditorStyles.boldLabel);
        EditorGUILayout.Separator();

        for (int count = 0; count < genesList.arraySize; ++count)
        {
            SerializedProperty arrayElement = genesList.GetArrayElementAtIndex(count);
            SerializedProperty choiceList = arrayElement.FindPropertyRelative("weight");
            SerializedProperty text = arrayElement.FindPropertyRelative("minValue");
            SerializedProperty id = arrayElement.FindPropertyRelative("maxValue");

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(id.floatValue.ToString());
            /*
            if (GUILayout.Button("Edit"))
            {
                _view = View.Gene;
                _currentIndex = count;
                break;
            }

            if (GUILayout.Button("Delete"))
            {
                genesList.DeleteArrayElementAtIndex(count);
                break;
            }
            */
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(text);
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndVertical();

        if (GUILayout.Button("Return to Chromosome List", GUILayout.Height(50)))
        {
            view = View.ChromosomeList;
            chromosomeIndex = -1;
        }
    }

    private void OnGUI_ChromosomeView(List<ChromosomeData> dataList)
    {
        EditorGUILayout.BeginVertical();

        if (dataList.Count == 0)
        {
            AssetDatabase.CreateAsset(CreateInstance<ChromosomeData>(), PathToDataAssets);
        }

        EditorGUILayout.LabelField("List of Chromosomes", EditorStyles.boldLabel);
        EditorGUILayout.Separator();

        for (int count = 0; count < dataList.Count; ++count)
        {
            string text = dataList[count].name;
            ChromosomeData nextData = dataList[count];

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(text.ToString());

            if (GUILayout.Button("Edit Chromosome"))
            {
                chromosomeIndex = count;
                nextData = dataList[count];
                chromosomeData = nextData;
                view = View.GeneList;
                break;
            }

            EditorGUILayout.EndHorizontal();
        }

        if (GUILayout.Button("Add New Chromosome"))
        {
            CreateNewChromosome();
        }
        EditorGUILayout.EndVertical();
    }

    private void OnGUI_GeneView(SerializedProperty geneList, int index)
    {
        SerializedProperty arrayElement = geneList.GetArrayElementAtIndex(index);
        SerializedProperty weight = arrayElement.FindPropertyRelative("weight");
        SerializedProperty name = arrayElement.FindPropertyRelative("name");
        SerializedProperty min = arrayElement.FindPropertyRelative("minValue");
        SerializedProperty max = arrayElement.FindPropertyRelative("maxValue");

        EditorGUILayout.BeginVertical();

        EditorGUILayout.LabelField(chromosomeData.name + " gene ID: " + name.ToString(), EditorStyles.boldLabel);
        EditorGUILayout.Separator();
        //min.stringValue = EditorGUILayout.TextArea(min.stringValue, GUILayout.Height(200));

        EditorGUILayout.EndVertical();

        if (GUILayout.Button("Return to Gene List", GUILayout.Height(50)))
        {
            view = View.GeneList;
            currentIndex = -1;
        }
    }
}

