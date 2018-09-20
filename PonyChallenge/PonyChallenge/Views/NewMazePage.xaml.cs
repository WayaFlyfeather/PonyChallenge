using PonyChallenge.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PonyChallenge.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class NewMazePage : ContentPage
	{
        PonyMazeViewModel vm => BindingContext as PonyMazeViewModel;
        PonyMazeViewModel prevVM = null;
        public NewMazePage ()
		{
			InitializeComponent ();
            prevVM = vm;
            if (vm != null)
                vm.PropertyChanged += VM_PropertyChanged;
		}

        private void VM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (String.IsNullOrEmpty(e.PropertyName) || e.PropertyName==nameof(PonyMazeViewModel.LatestSnapshot))
            {
                if (vm.LatestSnapshot != null)
                    App.Current.MainPage = new NavigateMazePage() { BindingContext = vm };
            }
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            if (prevVM != null)
                prevVM.PropertyChanged -= VM_PropertyChanged;
            if (vm!= null)
                vm.PropertyChanged += VM_PropertyChanged;
            prevVM = vm;
        }
    }
}