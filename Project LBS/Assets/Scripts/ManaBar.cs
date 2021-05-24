using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Battle
{
    public class ManaBar : MonoBehaviour
    {
        [SerializeField] Entity_Character character;
        public TextMeshProUGUI text = null;
        public Slider slider = null;
        // Start is called before the first frame update
        void Start()
        {
            UpdateMana("", 0);
            character.stats.OnManaChanged += UpdateMana;
        }
        public void UpdateMana(string type, int deltaVal)
        {
            float val = 0;
            if (character.stats.MP > 0)
            {
                val = ((float)character.stats.MP / character.maxStats.MP);
            }
            if (slider)
            {
                slider.value = val;
            }
            if (text)
            {
                text.text = val.ToString("P", System.Globalization.CultureInfo.InvariantCulture);
            }
        }
    }
}
