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
	public partial class NavigateMazePage : ContentPage
	{
        PonyMazeViewModel vm => BindingContext as PonyMazeViewModel;
        PonyMazeViewModel prevVM = null;

        public NavigateMazePage ()
		{
			InitializeComponent ();
            prevVM = vm;
            if (vm != null)
            {
                vm.PropertyChanged += VM_PropertyChanged;
                buildMaze();
            }
		}

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            if (prevVM != null)
                prevVM.PropertyChanged -= VM_PropertyChanged;
            if (vm != null)
                vm.PropertyChanged += VM_PropertyChanged;
            prevVM = vm;
            buildMaze();
        }

        private void VM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (String.IsNullOrEmpty(e.PropertyName) || e.PropertyName == nameof(PonyMazeViewModel.LatestSnapshot))
            {
                buildMaze();
            }
        }


        void buildMaze()
        {
            TheMaze.Children.Clear();

            if (vm?.LatestSnapshot != null)
            {
                for (int y = 0; y < vm.Model.Height; y++)
                {
                    StackLayout row = new StackLayout();
                    row.HorizontalOptions = LayoutOptions.Fill;
                    row.VerticalOptions = LayoutOptions.FillAndExpand;
                    row.Spacing = 0;
                    row.Orientation = StackOrientation.Horizontal;
                    for (int x = 0; x < vm.Model.Width; x++)
                    {
                        MazeLocationView locationView = new MazeLocationView()
                        {
                            BindingContext = new MazeLocationViewModel(vm.Model.Positions.Locations[x, y]),
                        };
                        row.Children.Add(locationView);
                    }
                    TheMaze.Children.Add(row);
                }
            }
        }
	}
}