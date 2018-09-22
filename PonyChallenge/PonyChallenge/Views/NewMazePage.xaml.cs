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
        public NewMazePage ()
		{
			InitializeComponent ();
		}

        private void VM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (String.IsNullOrEmpty(e.PropertyName) || e.PropertyName==nameof(PonyMazeViewModel.LatestSnapshot))
            {
                if (vm.LatestSnapshot != null)
                    App.Current.MainPage = new NavigateMazePage() { BindingContext = vm };
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (vm != null)
                vm.PropertyChanged += VM_PropertyChanged;
        }

        protected override void OnDisappearing()
        {
            if (vm != null)
                vm.PropertyChanged -= VM_PropertyChanged;

            base.OnDisappearing();
        }
    }
}