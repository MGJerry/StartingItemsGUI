using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace StartingItemsGUI
{
    public class Util : MonoBehaviour
    {
        static public List<float> GetDifficultyParabola(float easyMultiplier, float normalMultiplier, float hardMultiplier, float eclipseMultiplier)
        {
            float max = Mathf.Infinity;
            float min = -Mathf.Infinity;
            float a = 0;
            float b = 0;
            float c = 0;

            List<Vector2> unsortedCoordinates = new List<Vector2>()
            {
                    new Vector2(RoR2.DifficultyCatalog.GetDifficultyDef(RoR2.DifficultyIndex.Easy).scalingValue, easyMultiplier),
                    new Vector2(RoR2.DifficultyCatalog.GetDifficultyDef(RoR2.DifficultyIndex.Normal).scalingValue, normalMultiplier),
                    new Vector2(RoR2.DifficultyCatalog.GetDifficultyDef(RoR2.DifficultyIndex.Hard).scalingValue, hardMultiplier),
                    new Vector2(RoR2.DifficultyCatalog.GetDifficultyDef(RoR2.DifficultyIndex.Eclipse1).scalingValue, eclipseMultiplier),
                    // These vaues should probably be adjusted some day.
                    new Vector2(RoR2.DifficultyCatalog.GetDifficultyDef(RoR2.DifficultyIndex.Eclipse2).scalingValue, eclipseMultiplier + 1),
                    new Vector2(RoR2.DifficultyCatalog.GetDifficultyDef(RoR2.DifficultyIndex.Eclipse3).scalingValue, eclipseMultiplier + 2),
                    new Vector2(RoR2.DifficultyCatalog.GetDifficultyDef(RoR2.DifficultyIndex.Eclipse4).scalingValue, eclipseMultiplier + 3),
                    new Vector2(RoR2.DifficultyCatalog.GetDifficultyDef(RoR2.DifficultyIndex.Eclipse5).scalingValue, eclipseMultiplier + 4),
                    new Vector2(RoR2.DifficultyCatalog.GetDifficultyDef(RoR2.DifficultyIndex.Eclipse6).scalingValue, eclipseMultiplier + 5),
                    new Vector2(RoR2.DifficultyCatalog.GetDifficultyDef(RoR2.DifficultyIndex.Eclipse7).scalingValue, eclipseMultiplier + 6),
                    new Vector2(RoR2.DifficultyCatalog.GetDifficultyDef(RoR2.DifficultyIndex.Eclipse8).scalingValue, eclipseMultiplier + 7)
            };

            var dictCoordinates = new Dictionary<float, List<float>>();
            foreach (var coordinate in unsortedCoordinates)
            {
                if (!dictCoordinates.ContainsKey(coordinate.x))
                {
                    dictCoordinates.Add(coordinate.x, new List<float>());
                }
                dictCoordinates[coordinate.x].Add(coordinate.y);
            }

            List<float> xValues = dictCoordinates.Keys.ToList();
            // I wonder if sorting is actually necessary.
            xValues.Sort();
            var coordinates = new List<Vector2>();
            foreach (var xValue in xValues)
            {
                // Same as above. Is sorting necessary?
                dictCoordinates[xValue].Sort();
                foreach (var yValue in dictCoordinates[xValue])
                {
                    coordinates.Add(new Vector2(xValue, yValue));
                }
            }

            Vector2 A = coordinates[0];
            Vector2 B = coordinates[1];
            Vector2 C = coordinates[2];

            // I am not touching this with a 10-foot pole.
            if ((A.x != B.x || A.y == B.y ) && (B.x != C.x || B.y == C.y) && (C.x != A.x || C.y == A.y))
            {
                if (A.y != B.y && B.y != C.y && A.x != B.x && B.x != C.x && C.x != A.x)
                {
                    var AB = new float[] { Mathf.Pow(A.x, 2) - Mathf.Pow(B.x, 2), A.x - B.x, A.y - B.y };
                    var BC = new float[] { Mathf.Pow(B.x, 2) - Mathf.Pow(C.x, 2), B.x - C.x, B.y - C.y };
                    a = (BC[1] * AB[2] + AB[1] * BC[2]) / (BC[1] * AB[0] + AB[1] * BC[0]);
                    b = (AB[2] - AB[0] * a) / AB[1];
                    c = A.y - a * Mathf.Pow(A.x, 2) - b * A.x;
                }
                else if (A.y == B.y && B.y == C.y)
                {
                    c = A.y;
                }
                else
                {
                    Vector2 D;
                    Vector2 E;
                    if (A.y == B.y)
                    {
                        D = B;
                        E = C;
                    }
                    else
                    {
                        D = B;
                        E = A; 
                    }

                    var DE = new float[] { D.x - E.x, D.y - E.y };
                    b = DE[1] / DE[0];
                    c = D.y - D.x * b;
                    if (D.y > E.y)
                    {
                        max = D.y;
                    }
                    else
                    {
                        min = D.y;
                    }
                }
            }

            return new() { a, b, c, max, min };
        }

        static public string TrimString(string givenString) {
            if (givenString.Length > 0) {
                return givenString.Substring(0, givenString.Length - 1);
            }
            return "";
        }

        static public string MultilineToSingleLine(string givenFallback, string givenPath) {
            if (givenPath != "null") {
            if (File.Exists(givenPath)) {
                    StreamReader reader = new StreamReader(givenPath);
                    string line = "";
                    while (reader.Peek() >= 0) {
                        string newString = reader.ReadLine().Replace(Data.splitChar, Data.dictChar);
                        line += newString + Data.splitChar;
                    }
                    reader.Close();
                    line = TrimString(line);
                    return line;
                }
        }
            return givenFallback;
        }

        static public string ListToString(List<int> givenList) {
            string newString = "";
            foreach (int item in givenList) {
                newString += item.ToString() + Data.splitChar;
            }
            newString = TrimString(newString);
            return newString;
        }

        static public string DictToString(List<Dictionary<int, int>> givenList) {
            string newString = "";
            foreach (Dictionary<int, int> item in givenList) {
                foreach (int key in item.Keys) {
                    newString += key.ToString() + Data.dictChar + item[key].ToString() + Data.splitChar;
                }
                newString = TrimString(newString);
                newString += Data.profileChar;
            }
            newString = TrimString(newString);
            return newString;
        }

        static public string GetConfig(Dictionary<string, string> config, List<string> keys) {
            foreach (string key in keys) {
                if (config.ContainsKey(key)) {
                    return config[key];
                }
            }
            return "";
        }

        static string MapHierarchy(Transform givenTransform, int givenDepth, string givenTree) {
            string workingString = "";
            for (int spaceNumber = 0; spaceNumber < givenDepth * 4; spaceNumber++) {
                workingString += " ";
            }
            workingString += givenTransform.gameObject.name + "\n";
            givenTree += workingString;
            for (int childIndex = 0; childIndex < givenTransform.childCount; childIndex++) {
                givenTree = MapHierarchy(givenTransform.GetChild(childIndex), givenDepth + 1, givenTree);
            }
            return givenTree;
        }

        static public bool GetObjectFromHierarchy(ref Transform desiredObject, List<string> hierarchy, int hierarchyIndex, Transform parent) {
            bool childFound = false;
            if (hierarchyIndex == 0) {
                GameObject[] rootObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
                for (int rootIndex = 0; rootIndex < rootObjects.Length; rootIndex++) {
                    if (rootObjects[rootIndex].name == hierarchy[hierarchyIndex]) {
                        parent = rootObjects[rootIndex].transform;
                        childFound = true;
                    }
                }
            } else {
                for (int childIndex = 0; childIndex < parent.childCount; childIndex++) {
                    if (parent.GetChild(childIndex).name == hierarchy[hierarchyIndex]) {
                        parent = parent.GetChild(childIndex);
                        childFound = true;
                    }
                }
            }
            if (childFound) {
                if (hierarchyIndex == hierarchy.Count - 1) {
                    desiredObject = parent;
                    return true;
                } else {
                    return GetObjectFromHierarchy(ref desiredObject, hierarchy, hierarchyIndex + 1, parent);
                }
            }
            return false;
        }

        public static void MakeDirectoryExist(string location)
        {
            Log.LogInfo("Checking if directory exists: " + location);

            if (!System.IO.Directory.Exists(location))
            {
                System.IO.Directory.CreateDirectory(location);
            }
        }
    }
}
