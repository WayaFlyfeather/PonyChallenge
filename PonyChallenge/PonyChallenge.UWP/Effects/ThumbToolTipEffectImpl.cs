using PonyChallenge.Effects;
using PonyChallenge.UWP.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using Xamarin.Forms.Platform.UWP;

[assembly: Xamarin.Forms.ResolutionGroupName("JonRLevy")]
[assembly: Xamarin.Forms.ExportEffect(typeof(PonyChallenge.UWP.Effects.ThumbToolTipEffectImpl), "ThumbToolTipEffect")]
namespace PonyChallenge.UWP.Effects
{
    public class ThumbToolTipEffectImpl : PlatformEffect
    {
        bool previousIsThumbToolTipEnabled = true;
        IValueConverter previousThumbToolTipValueConverter = null;

        protected override void OnAttached()
        {
            ThumbToolTipEffect thumbToolTipEffect = (ThumbToolTipEffect)Element.Effects.First(e => e is ThumbToolTipEffect);

            switch (Control)
            {
                case FormsSlider formsSlider:
                    previousIsThumbToolTipEnabled = formsSlider.IsThumbToolTipEnabled;
                    formsSlider.IsThumbToolTipEnabled = !thumbToolTipEffect.Suppressed;
                    previousThumbToolTipValueConverter = formsSlider.ThumbToolTipValueConverter;
                    if (thumbToolTipEffect.ThumbToolTipValueConverter != null)
                    {
                        formsSlider.ThumbToolTipValueConverter = new WindowsToXFormsValueConverter(thumbToolTipEffect.ThumbToolTipValueConverter);
                    }
                    break;
            }
        }

        protected override void OnDetached()
        {
            switch (Control)
            {
                case FormsSlider formsSlider:
                    formsSlider.IsThumbToolTipEnabled = previousIsThumbToolTipEnabled;
                    formsSlider.ThumbToolTipValueConverter = previousThumbToolTipValueConverter;
                    break;
            }
        }
    }
}
