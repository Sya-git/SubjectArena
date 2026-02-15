using System;
using SubjectArena.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SubjectArena.Combat.UI
{
    public class CanvasHealthBar : MonoBehaviour
    {
        [SerializeField] private Slider slider;
        [SerializeField] private Slider subHealthSlider;
        [SerializeField] private float subHealthDelay;
        [SerializeField] private TMP_Text txtHealth;

        private Health _health;
        private float _subHealthTimer;
        
        public void Initialize(Health health)
        {
            health.Evt_OnHealthChanged += OnHealthChanged;
            subHealthSlider.maxValue = slider.maxValue = health.MaxHealth;
            subHealthSlider.value = slider.value = health.CurrentHealth;
            _health = health;
            RefreshHealthText();
        }

        private void OnHealthChanged(uint oldValue, uint newValue)
        {
            slider.value = newValue;
            RefreshHealthText();
        }

        private void RefreshHealthText()
        {
            txtHealth.text = $"HP: {_health.CurrentHealth}/{_health.MaxHealth}";
        }

        private void Update()
        {
            if (!Mathf.Approximately(0, slider.value - subHealthSlider.value))
            {
                _subHealthTimer += Time.deltaTime;
                if (_subHealthTimer >= 1f)
                {
                    subHealthSlider.value = subHealthSlider.value.Damp(slider.value, .5f, Time.deltaTime);
                }
            }
            else
            {
                _subHealthTimer = 0;
            }
        }
    }
}