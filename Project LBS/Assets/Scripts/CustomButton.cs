using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
namespace Battle
{
    public class CustomButton : MonoBehaviour
    {
        public Image image = null;
        public Text text = null;
        public TextMeshProUGUI textMesh = null;
        public BookReference book;

        public Color normalColor = Color.white;
        public Color disabledColor = Color.gray;
        public Color highlightColor = Color.cyan;
        [SerializeField] bool interactable = true;
        public void SetInteractive(bool value)
        {
            interactable = value;
            if (value == false)
            {
                image.color = disabledColor;
            }
            else
            {
                image.color = normalColor;
            }
        }
        public bool GetInteractive { get => interactable; }

        public void Select()
        {
            if (GetInteractive)
                image.color = highlightColor;
            else
            {
                image.color = highlightColor * disabledColor;
            }
        }
        public void DeSelect()
        {
            if (GetInteractive)
                image.color = normalColor;
            else
                image.color = disabledColor;
        }
        public void SetValue(BookReference book)
        {
            this.book = book;
            if (text) text.text = book.Value;
            if (textMesh) textMesh.text = book.Value;
        }
    }
}