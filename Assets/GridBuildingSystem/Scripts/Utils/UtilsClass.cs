using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
namespace CodeMonkey.Utils
{
    public static class UtilsClass
    {
        private static readonly Vector3 Vector3zero = Vector3.zero;
        private static readonly Vector3 Vector3one = Vector3.one;
        private static readonly Vector3 Vector3yDown = new Vector3(0, -1);
        public const int sortingOrderDefault = 5000;
        // Parse a float, return default if failed
        public static float Parse_Float(string txt, float _default)
        {
            float f;
            if (!float.TryParse(txt, out f))
            {
                f = _default;
            }
            return f;
        }
        // Parse a int, return default if failed
        public static int Parse_Int(string txt, int _default)
        {
            int i;
            if (!int.TryParse(txt, out i))
            {
                i = _default;
            }
            return i;
        }
        public static int Parse_Int(string txt)
        {
            return Parse_Int(txt, -1);
        }
        // Get Mouse Position in World with Z = 0f
        public static Vector3 GetMouseWorldPosition()
        {
            Vector3 vec = GetMouseWorldPositionWithZ(Input.mousePosition, Camera.main);
            vec.z = 0f;
            return vec;
        }
        public static Vector3 GetMouseWorldPositionWithZ()
        {
            return GetMouseWorldPositionWithZ(Input.mousePosition, Camera.main);
        }
        public static Vector3 GetMouseWorldPositionWithZ(Camera worldCamera)
        {
            return GetMouseWorldPositionWithZ(Input.mousePosition, worldCamera);
        }
        public static Vector3 GetMouseWorldPositionWithZ(Vector3 screenPosition, Camera worldCamera)
        {
            Vector3 worldPosition = worldCamera.ScreenToWorldPoint(screenPosition);
            return worldPosition;
        }
        public static Vector3 GetDirToMouse(Vector3 fromPosition)
        {
            Vector3 mouseWorldPosition = GetMouseWorldPosition();
            return (mouseWorldPosition - fromPosition).normalized;
        }
        // Is Mouse over a UI Element? Used for ignoring World clicks through UI
        public static bool IsPointerOverUI()
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return true;
            }
            PointerEventData pe = new PointerEventData(EventSystem.current);
            pe.position = Input.mousePosition;
            List<RaycastResult> hits = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pe, hits);
            return hits.Count > 0;
        }
        // Returns 00-FF, value 0->255
        public static string Dec_to_Hex(int value)
        {
            return value.ToString("X2");
        }
        // Returns 0-255
        public static int Hex_to_Dec(string hex)
        {
            return Convert.ToInt32(hex, 16);
        }
        // Returns a hex string based on a number between 0->1
        public static string Dec01_to_Hex(float value)
        {
            return Dec_to_Hex((int)Mathf.Round(value * 255f));
        }
        // Returns a float between 0->1
        public static float Hex_to_Dec01(string hex)
        {
            return Hex_to_Dec(hex) / 255f;
        }
        // Generate random normalized direction
        public static Vector3 GetRandomDir()
        {
            return new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f)).normalized;
        }
        // Generate random normalized direction
        public static Vector3 GetRandomDirXZ()
        {
            return new Vector3(UnityEngine.Random.Range(-1f, 1f), 0, UnityEngine.Random.Range(-1f, 1f)).normalized;
        }
        public static Vector3 GetVectorFromAngle(int angle)
        {
            // angle = 0 -> 360
            float angleRad = angle * (Mathf.PI / 180f);
            return new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
        }
        public static Vector3 GetVectorFromAngle(float angle)
        {
            // angle = 0 -> 360
            float angleRad = angle * (Mathf.PI / 180f);
            return new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
        }
        public static Vector3 GetVectorFromAngleInt(int angle)
        {
            // angle = 0 -> 360
            float angleRad = angle * (Mathf.PI / 180f);
            return new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
        }
        public static float GetAngleFromVectorFloat(Vector3 dir)
        {
            dir = dir.normalized;
            float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            if (n < 0) n += 360;
            return n;
        }
        public static float GetAngleFromVectorFloatXZ(Vector3 dir)
        {
            dir = dir.normalized;
            float n = Mathf.Atan2(dir.z, dir.x) * Mathf.Rad2Deg;
            if (n < 0) n += 360;
            return n;
        }
        public static int GetAngleFromVector(Vector3 dir)
        {
            dir = dir.normalized;
            float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            if (n < 0) n += 360;
            int angle = Mathf.RoundToInt(n);
            return angle;
        }
        public static int GetAngleFromVector180(Vector3 dir)
        {
            dir = dir.normalized;
            float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            int angle = Mathf.RoundToInt(n);

            return angle;
        }
        public static Vector3 ApplyRotationToVector(Vector3 vec, Vector3 vecRotation)
        {
            return ApplyRotationToVector(vec, GetAngleFromVectorFloat(vecRotation));
        }
        public static Vector3 ApplyRotationToVector(Vector3 vec, float angle)
        {
            return Quaternion.Euler(0, 0, angle) * vec;
        }
        public static Vector3 ApplyRotationToVectorXZ(Vector3 vec, float angle)
        {
            return Quaternion.Euler(0, angle, 0) * vec;
        }
        // Get UI Position from World Position
        public static Vector2 GetWorldUIPosition(Vector3 worldPosition, Transform parent, Camera uiCamera, Camera worldCamera)
        {
            Vector3 screenPosition = worldCamera.WorldToScreenPoint(worldPosition);
            Vector3 uiCameraWorldPosition = uiCamera.ScreenToWorldPoint(screenPosition);
            Vector3 localPos = parent.InverseTransformPoint(uiCameraWorldPosition);
            return new Vector2(localPos.x, localPos.y);
        }
        public static Vector3 GetWorldPositionFromUIZeroZ()
        {
            Vector3 vec = GetWorldPositionFromUI(Input.mousePosition, Camera.main);
            vec.z = 0f;
            return vec;
        }
        // Get World Position from UI Position
        public static Vector3 GetWorldPositionFromUI()
        {
            return GetWorldPositionFromUI(Input.mousePosition, Camera.main);
        }
        public static Vector3 GetWorldPositionFromUI(Camera worldCamera)
        {
            return GetWorldPositionFromUI(Input.mousePosition, worldCamera);
        }
        public static Vector3 GetWorldPositionFromUI(Vector3 screenPosition, Camera worldCamera)
        {
            Vector3 worldPosition = worldCamera.ScreenToWorldPoint(screenPosition);
            return worldPosition;
        }
        public static Vector3 GetWorldPositionFromUI_Perspective()
        {
            return GetWorldPositionFromUI_Perspective(Input.mousePosition, Camera.main);
        }
        public static Vector3 GetWorldPositionFromUI_Perspective(Camera worldCamera)
        {
            return GetWorldPositionFromUI_Perspective(Input.mousePosition, worldCamera);
        }
        public static Vector3 GetWorldPositionFromUI_Perspective(Vector3 screenPosition, Camera worldCamera)
        {
            Ray ray = worldCamera.ScreenPointToRay(screenPosition);
            Plane xy = new Plane(Vector3.forward, new Vector3(0, 0, 0f));
            float distance;
            xy.Raycast(ray, out distance);
            return ray.GetPoint(distance);
        }
        // Return random element from array
        public static T GetRandom<T>(T[] array)
        {
            return array[UnityEngine.Random.Range(0, array.Length)];
        }
        [System.Serializable]
        private class JsonDictionary
        {
            public List<string> keyList = new List<string>();
            public List<string> valueList = new List<string>();
        }
        // Take a Dictionary and return JSON string
        public static string SaveDictionaryJson<TKey, TValue>(Dictionary<TKey, TValue> dictionary)
        {
            JsonDictionary jsonDictionary = new JsonDictionary();
            foreach (TKey key in dictionary.Keys)
            {
                jsonDictionary.keyList.Add(JsonUtility.ToJson(key));
                jsonDictionary.valueList.Add(JsonUtility.ToJson(dictionary[key]));
            }
            string saveJson = JsonUtility.ToJson(jsonDictionary);
            return saveJson;
        }
        // Take a JSON string and return Dictionary<T1, T2>
        public static Dictionary<TKey, TValue> LoadDictionaryJson<TKey, TValue>(string saveJson)
        {
            JsonDictionary jsonDictionary = JsonUtility.FromJson<JsonDictionary>(saveJson);
            Dictionary<TKey, TValue> ret = new Dictionary<TKey, TValue>();
            for (int i = 0; i < jsonDictionary.keyList.Count; i++)
            {
                TKey key = JsonUtility.FromJson<TKey>(jsonDictionary.keyList[i]);
                TValue value = JsonUtility.FromJson<TValue>(jsonDictionary.valueList[i]);
                ret[key] = value;
            }
            return ret;
        }
        // Destroy all children of this parent
        public static void DestroyChildren(Transform parent)
        {
            foreach (Transform transform in parent)
                GameObject.Destroy(transform.gameObject);
        }
        // Set all parent and all children to this layer
        public static void SetAllChildrenLayer(Transform parent, int layer)
        {
            parent.gameObject.layer = layer;
            foreach (Transform trans in parent)
            {
                SetAllChildrenLayer(trans, layer);
            }
        }
        // Is this position inside the FOV? Top Down Perspective
        public static bool IsPositionInsideFov(Vector3 pos, Vector3 aimDir, Vector3 posTarget, float fov)
        {
            int aimAngle = UtilsClass.GetAngleFromVector180(aimDir);
            int angle = UtilsClass.GetAngleFromVector180(posTarget - pos);
            int angleDifference = (angle - aimAngle);
            if (angleDifference > 180) angleDifference -= 360;
            if (angleDifference < -180) angleDifference += 360;
            if (!(angleDifference < fov / 2f && angleDifference > -fov / 2f))
            {
                // Not inside fov
                return false;
            }
            else
            {
                // Inside fov
                return true;
            }
        }
        public static Vector3 GetRandomPositionWithinRectangle(float xMin, float xMax, float yMin, float yMax)
        {
            return new Vector3(UnityEngine.Random.Range(xMin, xMax), UnityEngine.Random.Range(yMin, yMax));
        }
        public static Vector3 GetRandomPositionWithinRectangle(Vector3 lowerLeft, Vector3 upperRight)
        {
            return new Vector3(UnityEngine.Random.Range(lowerLeft.x, upperRight.x), UnityEngine.Random.Range(lowerLeft.y, upperRight.y));
        }
        public static List<Vector3> GetPositionListAround(Vector3 position, float distance, int positionCount)
        {
            List<Vector3> ret = new List<Vector3>();
            for (int i = 0; i < positionCount; i++)
            {
                int angle = i * (360 / positionCount);
                Vector3 dir = UtilsClass.ApplyRotationToVector(new Vector3(0, 1), angle);
                Vector3 pos = position + dir * distance;
                ret.Add(pos);
            }
            return ret;
        }
        public static List<Vector3> GetPositionListAround(Vector3 position, float[] ringDistance, int[] ringPositionCount)
        {
            List<Vector3> ret = new List<Vector3>();
            for (int ring = 0; ring < ringPositionCount.Length; ring++)
            {
                List<Vector3> ringPositionList = GetPositionListAround(position, ringDistance[ring], ringPositionCount[ring]);
                ret.AddRange(ringPositionList);
            }
            return ret;
        }
        public static List<Vector3> GetPositionListAround(Vector3 position, float distance, int positionCount, Vector3 direction, int angleStart, int angleIncrease)
        {
            List<Vector3> ret = new List<Vector3>();
            for (int i = 0; i < positionCount; i++)
            {
                int angle = angleStart + angleIncrease * i;
                Vector3 dir = UtilsClass.ApplyRotationToVector(direction, angle);
                Vector3 pos = position + dir * distance;
                ret.Add(pos);
            }
            return ret;
        }
        public static List<Vector3> GetPositionListAlongDirection(Vector3 position, Vector3 direction, float distancePerPosition, int positionCount)
        {
            List<Vector3> ret = new List<Vector3>();
            for (int i = 0; i < positionCount; i++)
            {
                Vector3 pos = position + direction * (distancePerPosition * i);
                ret.Add(pos);
            }
            return ret;
        }
        public static List<Vector3> GetPositionListAlongAxis(Vector3 positionStart, Vector3 positionEnd, int positionCount)
        {
            Vector3 direction = (positionEnd - positionStart).normalized;
            float distancePerPosition = (positionEnd - positionStart).magnitude / positionCount;
            return GetPositionListAlongDirection(positionStart + direction * (distancePerPosition / 2f), direction, distancePerPosition, positionCount);
        }
        public static List<Vector3> GetPositionListWithinRect(Vector3 lowerLeft, Vector3 upperRight, int positionCount)
        {
            List<Vector3> ret = new List<Vector3>();
            float width = upperRight.x - lowerLeft.x;
            float height = upperRight.y - lowerLeft.y;
            float area = width * height;
            float areaPerPosition = area / positionCount;
            float positionSquareSize = Mathf.Sqrt(areaPerPosition);
            Vector3 rowLeft, rowRight;
            rowLeft = new Vector3(lowerLeft.x, lowerLeft.y);
            rowRight = new Vector3(upperRight.x, lowerLeft.y);
            int rowsTotal = Mathf.RoundToInt(height / positionSquareSize);
            float increaseY = height / rowsTotal;
            rowLeft.y += increaseY / 2f;
            rowRight.y += increaseY / 2f;
            int positionsPerRow = Mathf.RoundToInt(width / positionSquareSize);
            for (int i = 0; i < rowsTotal; i++)
            {
                ret.AddRange(GetPositionListAlongAxis(rowLeft, rowRight, positionsPerRow));
                rowLeft.y += increaseY;
                rowRight.y += increaseY;
            }
            int missingPositions = positionCount - ret.Count;
            Vector3 angleDir = (upperRight - lowerLeft) / missingPositions;
            for (int i = 0; i < missingPositions; i++)
            {
                ret.Add(lowerLeft + (angleDir / 2f) + angleDir * i);
            }
            while (ret.Count > positionCount)
            {
                ret.RemoveAt(UnityEngine.Random.Range(0, ret.Count));
            }
            return ret;
        }
        public static List<Vector2Int> GetPosXYListDiamond(int size)
        {
            List<Vector2Int> list = new List<Vector2Int>();
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size - x; y++)
                {
                    list.Add(new Vector2Int(x, y));
                    list.Add(new Vector2Int(-x, y));
                    list.Add(new Vector2Int(x, -y));
                    list.Add(new Vector2Int(-x, -y));
                }
            }
            return list;
        }
        public static List<Vector2Int> GetPosXYListOblong(int width, int dropXamount, int increaseDropXamount, Vector3 dir)
        {
            List<Vector2Int> list = GetPosXYListOblong(width, dropXamount, increaseDropXamount);
            list = RotatePosXYList(list, UtilsClass.GetAngleFromVector(dir));
            return list;
        }
        public static List<Vector2Int> GetPosXYListOblong(int width, int dropXamount, int increaseDropXamount)
        {
            List<Vector2Int> triangle = GetPosXYListTriangle(width, dropXamount, increaseDropXamount);
            List<Vector2Int> list = new List<Vector2Int>(triangle);
            foreach (Vector2Int posXY in triangle)
            {
                if (posXY.y == 0) continue;
                list.Add(new Vector2Int(posXY.x, -posXY.y));
            }
            foreach (Vector2Int posXY in new List<Vector2Int>(list))
            {
                if (posXY.x == 0) continue;
                list.Add(new Vector2Int(-posXY.x, posXY.y));
            }
            return list;
        }
        public static List<Vector2Int> GetPosXYListTriangle(int width, int dropXamount, int increaseDropXamount)
        {
            List<Vector2Int> list = new List<Vector2Int>();
            for (int i = 0; i > -999; i--)
            {
                for (int j = 0; j < width; j++)
                {
                    list.Add(new Vector2Int(j, i));
                }
                width -= dropXamount;
                dropXamount += increaseDropXamount;
                if (width <= 0) break;
            }
            return list;
        }
        public static List<Vector2Int> RotatePosXYList(List<Vector2Int> list, int angle)
        {
            List<Vector2Int> ret = new List<Vector2Int>();
            for (int i = 0; i < list.Count; i++)
            {
                Vector2Int posXY = list[i];
                Vector3 vec = UtilsClass.ApplyRotationToVector(new Vector3(posXY.x, posXY.y), angle);
                ret.Add(new Vector2Int(Mathf.RoundToInt(vec.x), Mathf.RoundToInt(vec.y)));
            }
            return ret;
        }
        public static Transform CloneTransform(Transform transform, string name = null)
        {
            Transform clone = GameObject.Instantiate(transform, transform.parent);
            if (name != null)
                clone.name = name;
            else
                clone.name = transform.name;
            return clone;
        }
        public static Transform CloneTransform(Transform transform, string name, Vector2 newAnchoredPosition, bool setActiveTrue = false)
        {
            Transform clone = CloneTransform(transform, name);
            clone.GetComponent<RectTransform>().anchoredPosition = newAnchoredPosition;
            if (setActiveTrue)
            {
                clone.gameObject.SetActive(true);
            }
            return clone;
        }
        public static Transform CloneTransform(Transform transform, Transform newParent, string name = null)
        {
            Transform clone = GameObject.Instantiate(transform, newParent);
            if (name != null)
                clone.name = name;
            else
                clone.name = transform.name;

            return clone;
        }
        public static Transform CloneTransform(Transform transform, Transform newParent, string name, Vector2 newAnchoredPosition, bool setActiveTrue = false)
        {
            Transform clone = CloneTransform(transform, newParent, name);
            clone.GetComponent<RectTransform>().anchoredPosition = newAnchoredPosition;
            if (setActiveTrue)
            {
                clone.gameObject.SetActive(true);
            }
            return clone;
        }
        public static Transform CloneTransformWorld(Transform transform, Transform newParent, string name, Vector3 newLocalPosition)
        {
            Transform clone = CloneTransform(transform, newParent, name);
            clone.localPosition = newLocalPosition;
            return clone;
        }
        public static T[] ArrayAdd<T>(T[] arr, T add)
        {
            T[] ret = new T[arr.Length + 1];
            for (int i = 0; i < arr.Length; i++)
            {
                ret[i] = arr[i];
            }
            ret[arr.Length] = add;
            return ret;
        }
        public static void ShuffleArray<T>(T[] arr, int iterations)
        {
            for (int i = 0; i < iterations; i++)
            {
                int rnd = UnityEngine.Random.Range(0, arr.Length);
                T tmp = arr[rnd];
                arr[rnd] = arr[0];
                arr[0] = tmp;
            }
        }
        public static void ShuffleArray<T>(T[] arr, int iterations, System.Random random)
        {
            for (int i = 0; i < iterations; i++)
            {
                int rnd = random.Next(0, arr.Length);
                T tmp = arr[rnd];
                arr[rnd] = arr[0];
                arr[0] = tmp;
            }
        }
        public static void ShuffleList<T>(List<T> list, int iterations)
        {
            for (int i = 0; i < iterations; i++)
            {
                int rnd = UnityEngine.Random.Range(0, list.Count);
                T tmp = list[rnd];
                list[rnd] = list[0];
                list[0] = tmp;
            }
        }
    }
}