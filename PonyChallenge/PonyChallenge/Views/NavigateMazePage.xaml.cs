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

        public NavigateMazePage ()
		{
			InitializeComponent ();
            if (vm != null)
                buildMaze();
		}

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            if (vm != null)
                buildMaze();
        }

        void buildMaze()
        {
            for (int y=0; y<vm.Model.Height; y++)
            {
                StackLayout row = new StackLayout();
                row.HorizontalOptions = LayoutOptions.Fill;
                row.VerticalOptions = LayoutOptions.FillAndExpand;
                row.Spacing = 0;
                row.Orientation = StackOrientation.Horizontal;
                for (int x=0; x < vm.Model.Width; x++)
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