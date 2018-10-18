using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace PonyChallenge.Effects
{
    public class ThumbToolTipEffect : RoutingEffect
    {
        public bool Suppressed { get; set; } = false;
        public string StringFormat { get; set; } = null;
        public IValueConverter ThumbToolTipValueConverter { get; set; } = null;

        public ThumbToolTipEffect() : base("JonRLevy.ThumbToolTipEffect")
        { }
    }
}
