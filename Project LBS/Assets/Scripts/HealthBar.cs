using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Battle
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] Entity_Character character;
        public TextMeshProUGUI text = null;
        public Slider slider = null;
        // Start is called before the first frame update
        void Start()
        {
            UpdateHealth("", 0);
            character.Stats.OnHealthChanged += UpdateHealth;
        }
        public void UpdateHealth(string type, int deltaVal)
        {
            float val = ((float)character.Stats.HP / character.MaxStats.HP);
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
