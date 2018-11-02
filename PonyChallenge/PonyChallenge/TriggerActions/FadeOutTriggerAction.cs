using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace PonyChallenge.TriggerActions
{
    public class FadeOutTriggerAction : TriggerAction<VisualElement>
    {
        protected async override void Invoke(VisualElement sender)
        {
            await sender.FadeTo(0.0);
            sender.IsVisible = false;
        }
    }
}
