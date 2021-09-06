using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Battle
{
    public class UI_Visuals : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI turnText = null;

        [System.Serializable]
        public class CubeMenu
        {
            [Header("Cube Values")]
            public Transform rotationObject = null;
            public Transform positionPoint = null;
            public float defaultRate = 0.01f;
            public float rotationRate = 0.01f;
            public int index = 0;

            [Header("UI Values")]
            public TextMeshProUGUI textMesh = null;
            public string[] indexNames = new string[4];
        }
        [Header("Rotation Menu")]
        [SerializeField] CubeMenu cubeMenu = new CubeMenu();

        [System.Serializable]
        public class ListMenu
        {
            [Header("List Values")]
            public RectTransform listRect = null;
            public GameObject listPrefab = null;
            public List<CustomButton> listObjects = new List<CustomButton>();
            public BookReference[] bookReferences;
            [HideInInspector] public int index = 0;
            public int min = 0;
            public int max = 0;

        }
        [Header("List Menu")]
        public ListMenu listMenu = new ListMenu();

        public void UpdateTurnText(TurnSystem.Turns turn)
        {
            turnText.text = turn.ToString();
        }

        public void SetCubeActive(bool v)
        {
            cubeMenu.rotationObject.gameObject.SetActive(v);
        }

        #region Cube Animation
        public void CubeEnter()
        {
            UpdateCubeText(cubeMenu.index);
        }
        public void CubeExit()
        {
        }
        public void CubeAnimation(Vector2 dir)
        {
            cubeMenu.index += (int)dir.y;
            if (rotationCalls.Count == 0)
            {
                cubeMenu.rotationRate = cubeMenu.defaultRate;
                rotationCalls.Enqueue(dir);
                StartCoroutine(RotateDir());
            }
            else
            {
                rotationCalls.Enqueue(dir);
                cubeMenu.rotationRate /= 2;
            }
        }
        public void UpdateCubeText(int index)
        {
            cubeMenu.textMesh.text = cubeMenu.indexNames[Mathf.Clamp(index, 0, cubeMenu.indexNames.Length - 1)];
        }
        readonly Queue<Vector2> rotationCalls = new Queue<Vector2>();
        IEnumerator RotateDir()
        {
            while (rotationCalls.Count > 0)
            {
                Vector2 dir = rotationCalls.Peek();
                dir = new Vector2(-dir.y, dir.x);
                Vector3 vec = cubeMenu.rotationObject.transform.InverseTransformDirection(dir); // Rotate around x axis.
                float amountToRotate = 1f;
                // No matter how the object is angled, it'll always rotate around the x axis.
                Quaternion newRotation = cubeMenu.rotationObject.transform.rotation * Quaternion.AngleAxis(90, vec);
                for (int i = 0; i < 90; i++)
                {
                    cubeMenu.rotationObject.transform.rotation *= Quaternion.AngleAxis(amountToRotate, vec);
                    yield return new WaitForSeconds(cubeMenu.rotationRate);
                }
                cubeMenu.rotationObject.transform.rotation = newRotation;
                rotationCalls.Dequeue();
                if (rotationCalls.Count == 0)
                    cubeMenu.rotationRate = cubeMenu.defaultRate;
            }
        }
        #endregion

        #region List Animation
        public void ListEnter(int yCubeLeave, int yListLeave)
        {
            if (yCubeLeave == 0) AbilityListEnter(BattleManager.inst.IndexPlayer.rank.books.ToArray(), yListLeave);
            else if (yCubeLeave == 2) InventoryListEnter(BattleManager.inst.IndexPlayer);
        }
        void AbilityListEnter(BookReference[] books, int yVal)
        {
            listMenu.min = 0;
            listMenu.bookReferences = books;//Add Books
            for (int i = 0; i < listMenu.listRect.childCount; i++)
            {
                listMenu.listObjects.Add(listMenu.listRect.GetChild(i).GetComponent<CustomButton>());//Add Buttons
            }
            if (listMenu.listObjects.Count > 0)
            {
                listMenu.max = listMenu.listObjects.Count - 1;
                listMenu.listObjects[0].Select();
                AbilityListAnimate(yVal);
            }
            listMenu.listRect.gameObject.SetActive(true);
        }
        void InventoryListEnter(Entity_Player player)
        {
            listMenu.bookReferences = null;
            for (int i = 0; i < listMenu.listRect.childCount; i++)
            {
                listMenu.listObjects.Add(listMenu.listRect.GetChild(i).GetComponent<CustomButton>());//Add Buttons
            }
            listMenu.listRect.gameObject.SetActive(true);
        }

        public void ListExit()
        {
            listMenu.listObjects = new List<CustomButton>();
            listMenu.bookReferences = null;
            listMenu.listRect.gameObject.SetActive(false);
        }
        public void ListAnimate(int yLeave, int yVal)
        {
            if (yLeave == 0) AbilityListAnimate(yVal);
            else if (yLeave == 2) InventoryListAnimate(yVal);
        }
        void AbilityListAnimate(int val)
        {
            if (listMenu.listObjects.Count < 1)
                return;
            listMenu.listObjects[listMenu.index].DeSelect();


            if (val < listMenu.min)
            {
                listMenu.min = val;
                listMenu.max = listMenu.min + listMenu.listObjects.Count - 1;
            }
            if (val > listMenu.max)
            {
                listMenu.max = val;
                listMenu.min = listMenu.max - (listMenu.listObjects.Count - 1);
            }
            for (int i = 0; i < listMenu.listObjects.Count; i++)
            {
                listMenu.listObjects[i].SetValue(listMenu.bookReferences[listMenu.min + i]);
                listMenu.listObjects[i].SetInteractive(BattleManager.inst.UsableBook(BattleManager.inst.IndexPlayer, listMenu.bookReferences[listMenu.min + i]));

            }
            listMenu.index = val - listMenu.min;
            listMenu.listObjects[listMenu.index].Select();
        }
        void InventoryListAnimate(int val)
        {
            if (listMenu.listObjects.Count < 1)
                return;
            Debug.Log("Inventory: " + val);
        }

        #endregion

        #region Targeting
        IEnumerator ie = null;
        public void TargetEnter(Ability_Book book)
        {
            //Set PotencialTargets
            targets = BattleManager.inst.GetFaction
                (
                    (book.TargetType.HasFlag(TargetType.Support) ?
                    Entity_Preset.EntityFaction.Player :
                    Entity_Preset.EntityFaction.Enemy)
                );
            ie = FlashTargets(book);
            StartCoroutine(ie);
        }
        public void TargetExit()
        {
            if (ie != null)
                StopCoroutine(ie);
        }
        public void TargetAnimate(int val)
        {
            targetIndex = val;
        }

        Entity_Character[] targets = null;
        int targetIndex = 0;
        float flashTime = 0.1f;

        IEnumerator FlashTargets(Ability_Book book)
        {
            while (true)
            {
                if (!book.TargetType.HasFlag(TargetType.Random))
                {
                    targets[targetIndex % targets.Length].Flash(Color.gray, flashTime);
                }
                else
                {
                    foreach (var item in targets)
                    {
                        item.Flash(Color.gray, flashTime);
                    }
                }
                yield return new WaitForSeconds(flashTime * 2);
            }
        }
        #endregion
    }
}