using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace PonyChallenge.Effects
{
    public class ThumbToolTipEffect : RoutingEffect
    {
        public bool Suppressed { get; set; } = true;
        public IValueConverter ThumbToolTipValueConverter { get; set; } = null;

        public ThumbToolTipEffect() : base("JonRLevy.ThumbToolTipEffect")
        { }
    }
}
