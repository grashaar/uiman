using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UnuGames.MVVM
{
    public class InteractableBinder : BinderBase
    {
        public List<Selectable> enableOnTrue = new List<Selectable>();
        public List<Selectable> disableOnTrue = new List<Selectable>();

        [HideInInspector]
        public BindingField valueField = new BindingField("bool");

        [HideInInspector]
        public BoolConverter valueConverter = new BoolConverter("bool");

        public override void Initialize(bool forceInit)
        {
            if (!CheckInitialize(forceInit))
                return;

            SubscribeOnChangedEvent(this.valueField, OnUpdateValue);
        }

        private void OnUpdateValue(object val)
        {
            var valChange = this.valueConverter.Convert(val, this);

            if (this.enableOnTrue != null && this.enableOnTrue.Count > 0)
            {
                for (var i = 0; i < this.enableOnTrue.Count; i++)
                {
                    if (this.enableOnTrue[i])
                        this.enableOnTrue[i].interactable = valChange;
                }
            }

            if (this.disableOnTrue != null && this.disableOnTrue.Count > 0)
            {
                for (var i = 0; i < this.disableOnTrue.Count; i++)
                {
                    if (this.disableOnTrue[i])
                        this.disableOnTrue[i].interactable = !valChange;
                }
            }
        }
    }
}