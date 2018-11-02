using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace PonyChallenge.TriggerActions
{
    public class FadeInTriggerAction : TriggerAction<VisualElement>
    {
        protected async override void Invoke(VisualElement sender)
        {
            sender.Opacity = 0.0;
            sender.IsVisible = true;
            await sender.FadeTo(1.0);
        }
    }
}
