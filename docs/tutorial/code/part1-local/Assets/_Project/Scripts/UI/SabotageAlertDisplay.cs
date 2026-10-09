using System.Collections.Generic;
using System.Text;
using DumplingKitchen.Sabotage;
using TMPro;
using UnityEngine;

namespace DumplingKitchen.UI
{
    /// <summary>
    /// A world-space "what's broken" board for the chef (or a wrist panel, like the
    /// 2022 VrWristUI prototype). Listens to the static sabotage events.
    /// </summary>
    public sealed class SabotageAlertDisplay : MonoBehaviour
    {
        [SerializeField] private TMP_Text alertText;
        [SerializeField] private AudioSource alertSound;

        private readonly List<SabotageTarget> _broken = new();
        private readonly StringBuilder _builder = new();

        private void OnEnable()
        {
            SabotageTarget.AnySabotaged += HandleSabotaged;
            SabotageTarget.AnyRestored += HandleRestored;
            Refresh();
        }

        private void OnDisable()
        {
            SabotageTarget.AnySabotaged -= HandleSabotaged;
            SabotageTarget.AnyRestored -= HandleRestored;
        }

        private void HandleSabotaged(SabotageTarget target)
        {
            if (!_broken.Contains(target))
                _broken.Add(target);

            if (alertSound != null)
                alertSound.Play();

            Refresh();
        }

        private void HandleRestored(SabotageTarget target)
        {
            _broken.Remove(target);
            Refresh();
        }

        private void Refresh()
        {
            _builder.Clear();
            foreach (SabotageTarget target in _broken)
            {
                if (target != null)
                    _builder.AppendLine($"<color=#E5533D>!</color> {target.DisplayName}");
            }

            alertText.text = _builder.ToString();
        }
    }
}
