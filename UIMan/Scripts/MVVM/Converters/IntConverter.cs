﻿using System;

namespace UnuGames.MVVM
{
    [Serializable]
    public sealed class IntConverter : Converter<int, IntAdapter>
    {
        public IntConverter(string label) : base(label) { }

        protected override int ConvertWithoutAdapter(object value, UnityEngine.Object context)
            => IntAdapter.Convert(value, 0, context);

        protected override object ConvertWithoutAdapter(int value, UnityEngine.Object context)
            => IntAdapter.Convert(value);
    }
}