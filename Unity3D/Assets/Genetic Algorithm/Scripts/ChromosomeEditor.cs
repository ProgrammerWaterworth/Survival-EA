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
    Object source;
    bool genesDisplayed, geneInfoDisplayed = true;
    bool chromsomesDisplayed, chromosomeInfoDisplayed = true;
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
        SerializedProperty individual = dataObj.FindProperty("gameObject");

        EditorGUILayout.BeginVertical();
        scroll = EditorGUILayout.BeginScrollView(scroll);

        if (UnityEditor.EditorApplication.isPlaying)
            view = View.GeneList;

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
            OnGUI_ListView(geneList, individual);
        }

        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();

        dataObj.ApplyModifiedProperties();
    }

#if UNITY_EDITOR

#endif

    private void OnGUI_ListView(SerializedProperty genesList, SerializedProperty individual)
    {
        EditorGUILayout.BeginVertical();

        EditorGUILayout.LabelField(chromosomeData.name + " genes: ", EditorStyles.boldLabel);
        EditorGUILayout.Separator();

            EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(individual);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Separator();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Display Genes", EditorStyles.toolbarButton))
        {
            genesDisplayed = true;
            geneInfoDisplayed = false;
        }

        if (GUILayout.Button("Information", EditorStyles.toolbarButton))
        {
            genesDisplayed = false;
            geneInfoDisplayed = true;
        }

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space(10);
        if (geneInfoDisplayed)
        {
            EditorGUILayout.HelpBox("Game Object is the prefab that the genetic algorithm will be applied to.", MessageType.Info, true);
            EditorGUILayout.HelpBox("The Button - \"Get Current GameObject Genes\" - Will search the Game Object for numerical data variables (E.g. float, int) and set each one to be a gene." , MessageType.Info, true);
            EditorGUILayout.HelpBox("The Button - \"Apply Genes to GameObject\" - Applies the values of the genes in this Editor Window to the Game Object Prefab. (May require re-selecting prefab for changes to take place).", MessageType.Info, true);
            EditorGUILayout.HelpBox("Chromosome - A chromosome is comprised of a number of genes which affect the behaviour of the individual (Game Object)." +
                " Running the genetic algorithm once defining the fitness of the individual will cause the individual to adapt to it's environment over a series of generations by altering it's genes.", MessageType.Info, true);
            EditorGUILayout.HelpBox("Genes - A genes value is determine by linearly interpolating the weight between min and max values, not exceeding the range. If the Genes Min and Max range are not valid" +
                " the weight returned from the genetic algorithm will be directly applied as the value.", MessageType.Info, true);
            EditorGUILayout.HelpBox("Adding/Removing numerical data variables to an individuals (Game Object) components will invalidate being able to \"Apply Genes " +
                "to GameObject\", it will have to be refreshed by clicking \"Get Current GameObject Genes\". This will ensure all genes are found and altered as intended.", MessageType.Warning, true);
        }

        if (genesDisplayed)
        {
            for (int count = 0; count < genesList.arraySize; ++count)
            {
                SerializedProperty arrayElement = genesList.GetArrayElementAtIndex(count);
                SerializedProperty weight = arrayElement.FindPropertyRelative("weight");
                SerializedProperty min = arrayElement.FindPropertyRelative("minValue");
                SerializedProperty max = arrayElement.FindPropertyRelative("maxValue");
                SerializedProperty value = arrayElement.FindPropertyRelative("value");
                SerializedProperty geneName = arrayElement.FindPropertyRelative("geneName");

                EditorGUILayout.BeginHorizontal();
                if (geneName != null)
                {
                    EditorGUILayout.LabelField(geneName.stringValue, EditorStyles.boldLabel);
                }
                else
                    EditorGUILayout.LabelField("gene " + count.ToString(), EditorStyles.boldLabel);

                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                if (weight != null)
                {
                    EditorGUILayout.LabelField("Weight", GUILayout.MaxWidth(40));
                    EditorGUILayout.PropertyField(weight, GUIContent.none, true);
                    GUILayout.Space(10);
                }
                if (min != null)
                {
                    EditorGUILayout.LabelField("Min Value", GUILayout.MaxWidth(60));
                    EditorGUILayout.PropertyField(min, GUIContent.none, true);
                    GUILayout.Space(10);
                }
                if (max != null)
                {
                    EditorGUILayout.LabelField("Max Value", GUILayout.MaxWidth(60));
                    EditorGUILayout.PropertyField(max, GUIContent.none, true);
                    GUILayout.Space(10);
                }
                if (value != null)
                {
                    EditorGUILayout.LabelField("Value", GUILayout.MaxWidth(35));
                    EditorGUILayout.PropertyField(value, GUIContent.none, true);
                    GUILayout.Space(10);
                }
                EditorGUILayout.EndHorizontal();
                GUILayout.Space(10);
            }
        }
        EditorGUILayout.EndVertical();
        GUILayout.Space(20);

        if (genesDisplayed)
        {
            if (GUILayout.Button("Get Current GameObject Genes", GUILayout.Height(30)))
            {
                chromosomeData.GetCurrentGameObjectGenes();
            }
            if (!UnityEditor.EditorApplication.isPlaying)
            {
                if (GUILayout.Button("Apply Genes to GameObject", GUILayout.Height(30)))
                {
                    chromosomeData.ApplyGenesToGameObject();
                }
            }
        }
        if (!UnityEditor.EditorApplication.isPlaying)
        {
            if (GUILayout.Button("Set Genetic Algorithm to this Chromosome", GUILayout.Height(30)))
            {
                chromosomeData.UpdateGeneticAlgorithm();
            }

            if (GUILayout.Button("Return to Chromosome List", GUILayout.Height(30)))
            {
                view = View.ChromosomeList;
                chromosomeIndex = -1;
            }
        }
    }

    private void OnGUI_ChromosomeView(List<ChromosomeData> dataList)
    {
        EditorGUILayout.BeginVertical();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Display Chromosomes", EditorStyles.toolbarButton))
        {
            chromsomesDisplayed = true;
            chromosomeInfoDisplayed = false;
        }

        if (GUILayout.Button("Information", EditorStyles.toolbarButton))
        {
            chromsomesDisplayed = false;
            chromosomeInfoDisplayed = true;
        }

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space(10);

        if (chromosomeInfoDisplayed)
        {
            EditorGUILayout.HelpBox("New Chromosomes will be created in the \"Assets\\Genetic Algorithm\\Data\" Folder. Changing data name will update chromosome in Editor.", MessageType.Info, true);
            EditorGUILayout.HelpBox("Data that is not located in the: \"Assets\\Genetic Algorithm\\Data\" Folder will not be found.", MessageType.Warning, true);
        }
        if (chromsomesDisplayed)
        {
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
                    //chromosomeData.GetGameObjectProperties();

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

