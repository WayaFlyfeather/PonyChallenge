using PonyChallenge.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using SkiaSharp;
using SkiaSharp.Views.Forms;

namespace PonyChallenge.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class NavigateMazePage : ContentPage
	{
        PonyMazeViewModel vm => BindingContext as PonyMazeViewModel;

        public NavigateMazePage ()
		{
			InitializeComponent ();
		}

        protected override void OnAppearing()
        {
            base.OnAppearing();
            vm.PropertyChanged += VM_PropertyChanged;
            vm.StartTick();
        }

        protected override void OnDisappearing()
        {
            vm.PropertyChanged -= VM_PropertyChanged;
            vm.StopTick();
            base.OnDisappearing();
        }

        private void VM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (String.IsNullOrEmpty(e.PropertyName) || e.PropertyName == nameof(PonyMazeViewModel.LatestSnapshot))
            {
                MazeCanvas.InvalidateSurface();
                if (vm.LatestSnapshot.State == Models.MazeState.Won)
                    OnGameWon();
                if (vm.LatestSnapshot.State == Models.MazeState.Lost)
                    OnGameLost();
            }
        }

        async void OnGameWon()
        {
            string wonMessage = String.Format("You helped {0} escape the maze from the clutches of Domo-Kun. You were so brave!", vm.SelectedPonyName);
            await DisplayAlert("Congratulations", wonMessage, "Play again");

            App.Current.MainPage = new NewMazePage() { BindingContext = new PonyMazeViewModel() };
        }

        async void OnGameLost()
        {
            string lostMessage = String.Format("Oh no, how unfortunate. {0} was captured by Domo-Kun and will now be tickled! Can you do better next time?", vm.SelectedPonyName);
            await DisplayAlert("You lost", lostMessage, "Play again");

            App.Current.MainPage = new NewMazePage() { BindingContext = new PonyMazeViewModel() };
        }

        void OnMazeCanvasPaintSurface(object sender, SKPaintSurfaceEventArgs args)
        {
            SKImageInfo info = args.Info;
            SKSurface surface = args.Surface;
            SKCanvas canvas = surface.Canvas;

            float WallWidth = 2f;

            canvas.Clear();

            if (vm?.LatestSnapshot == null)
                return;

            float locationUnit = Math.Min((info.Width - WallWidth * 2) / (float)vm.Width, (info.Height - WallWidth * 2) / (float)vm.Height);
            float halfLocationUnit = locationUnit / 2f;
            float quarterLocationUnit = locationUnit / 4f;

            float offsetX = (info.Width - (locationUnit * vm.Width)) / 2f;
            float offsetY = (info.Height - (locationUnit * vm.Height)) / 2f;

            using (SKPaint wallStroke = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                Color = Color.Black.ToSKColor(),
                StrokeWidth = WallWidth,
                StrokeCap = SKStrokeCap.Square
            })
            using (SKPaint ponyFill = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                Color = Color.Red.ToSKColor(),
            })
            using (SKPaint exitFill = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                Color = Color.Green.ToSKColor(),
            })
            using (SKPaint domokunFill = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                Color = Color.Brown.ToSKColor(),
            })
            {
                canvas.DrawRect(offsetX - WallWidth, offsetY - WallWidth, vm.Width * locationUnit + WallWidth * 2, vm.Height * locationUnit + WallWidth * 2, wallStroke);

                for (int y = 0; y < vm.Height; y++)
                {
                    for (int x = 0; x < vm.Width; x++)
                    {
                        if (vm.LatestSnapshot.Locations[x, y].NorthWall)
                            canvas.DrawLine(new SKPoint() { X = offsetX + (x * locationUnit), Y = offsetY + (y * locationUnit) }, new SKPoint() { X = offsetX + ((x + 1) * locationUnit), Y = offsetY + (y * locationUnit) }, wallStroke);
                        if (vm.LatestSnapshot.Locations[x, y].EastWall)
                            canvas.DrawLine(new SKPoint() { X = offsetX + ((x + 1) * locationUnit), Y = offsetY + (y * locationUnit) }, new SKPoint() { X = offsetX + ((x + 1) * locationUnit), Y = offsetY + ((y + 1) * locationUnit) }, wallStroke);
                        if (vm.LatestSnapshot.Locations[x, y].SouthWall)
                            canvas.DrawLine(new SKPoint() { X = offsetX + (x * locationUnit), Y = offsetY + ((y + 1) * locationUnit) }, new SKPoint() { X = offsetX + ((x + 1) * locationUnit), Y = offsetY + ((y + 1) * locationUnit) }, wallStroke);
                        if (vm.LatestSnapshot.Locations[x, y].WestWall)
                            canvas.DrawLine(new SKPoint() { X = offsetX + (x * locationUnit), Y = offsetY + (y * locationUnit) }, new SKPoint() { X = offsetX + (x * locationUnit), Y = offsetY + ((y + 1) * locationUnit) }, wallStroke);

                        if (vm.LatestSnapshot.Locations[x, y].IsExit)
                            canvas.DrawRect(offsetX + (x * locationUnit) + quarterLocationUnit, offsetY + (y * locationUnit) + quarterLocationUnit, halfLocationUnit, halfLocationUnit, exitFill);

                        if (vm.LatestSnapshot.Locations[x, y].ContainsPony)
                            canvas.DrawRect(offsetX + (x * locationUnit) + quarterLocationUnit, offsetY + (y * locationUnit) + quarterLocationUnit, halfLocationUnit, halfLocationUnit, ponyFill);

                        if (vm.LatestSnapshot.Locations[x, y].ContainsDomokun)
                            canvas.DrawRect(offsetX + (x * locationUnit) + quarterLocationUnit, offsetY + (y * locationUnit) + quarterLocationUnit, halfLocationUnit, halfLocationUnit, domokunFill);
                    }
                }
            }
        }
	}
}