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
using System.Diagnostics;
using System.Reflection;
using System.IO;
using PonyChallenge.Converters;

namespace PonyChallenge.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class NavigateMazePage : ContentPage
	{
        PonyMazeViewModel vm => BindingContext as PonyMazeViewModel;

        SKBitmap domokunBMP = null;
        SKBitmap ponyBMP = null;

        public NavigateMazePage ()
		{
            InitializeComponent();
		}

        protected override void OnAppearing()
        {
            base.OnAppearing();
            vm.PropertyChanged += VM_PropertyChanged;
            domokunBMP = loadBitmap("domokun.png");
            loadPonyBMP();
            vm.StartTick();
        }

        void loadPonyBMP()
        {
            ponyBMP?.Dispose();
            switch (vm.SelectedPonyName)
            {
                case "Applejack":
                    ponyBMP = loadBitmap("applejack.png"); break;
                case "Pinkie Pie":
                    ponyBMP = loadBitmap("pinkie_pie.png"); break;
                case "Spike":
                    ponyBMP = loadBitmap("spike.png"); break;
                case "Rarity":
                default:
                    ponyBMP = loadBitmap("rarity.png"); break;
            }
        }

        SKBitmap loadBitmap(string filename)
        {
            string resourceID = "PonyChallenge.Assets." + filename;
            Assembly assembly = this.GetType().GetTypeInfo().Assembly;

            SKBitmap bmp = null;
            using (Stream stream = assembly.GetManifestResourceStream(resourceID))
            {
                bmp = SKBitmap.Decode(stream);
            }

            return bmp;
        }

        protected override void OnDisappearing()
        {
            vm.PropertyChanged -= VM_PropertyChanged;
            vm.StopTick();

            domokunBMP.Dispose();
            domokunBMP = null;

            ponyBMP.Dispose();
            ponyBMP = null;

            base.OnDisappearing();
        }

        private void VM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (String.IsNullOrEmpty(e.PropertyName) || e.PropertyName == nameof(PonyMazeViewModel.HasSnapshot))
            {
                MazeCanvas.InvalidateSurface();
                CreatureCanvas.InvalidateSurface();
            }

            if (String.IsNullOrEmpty(e.PropertyName) || e.PropertyName == nameof(PonyMazeViewModel.LatestSnapshot))
            {
                CreatureCanvas.InvalidateSurface();
                if (vm.HasSnapshot && vm.LatestSnapshot.State == Models.MazeState.Won)
                    OnGameWon();
                if (vm.HasSnapshot && vm.LatestSnapshot.State == Models.MazeState.Lost)
                    OnGameLost();
            }

            if (String.IsNullOrEmpty(e.PropertyName) || e.PropertyName == nameof(PonyMazeViewModel.SelectedPonyName))
            {
                loadPonyBMP();
            }

            if (String.IsNullOrEmpty(e.PropertyName) || e.PropertyName == nameof(PonyMazeViewModel.UnreportedConnectionError) && vm.UnreportedConnectionError == true)
            {
                reportConnectionError();                
            }
        }

        async void OnGameWon()
        {
            string wonMessage = String.Format("You helped {0} escape the maze from the clutches of Domo-Kun. You were so brave!", vm.SelectedPonyName);
            await DisplayAlert("Congratulations", wonMessage, "Play again");

            vm.ResetMaze();
        }

        async void OnGameLost()
        {
            string lostMessage = String.Format("Oh no, how unfortunate. {0} was captured by Domo-Kun and will now be tickled! Can you do better next time?", vm.SelectedPonyName);
            await DisplayAlert("Game Over", lostMessage, "Play again");

            vm.ResetMaze();
        }

        async void reportConnectionError()
        {
            string connErrorMsg = String.Format("Uh-oh, it seems {0} has lost the connection to The Little Pony Hub... Maybe, if you try again later, you can save {0}?", vm.SelectedPonyName);
            await DisplayAlert("Problem", connErrorMsg , "OK");

            vm.UnreportedConnectionError = false;
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
            float eighthLocationUnit = locationUnit / 8f;

            float offsetX = (info.Width - (locationUnit * vm.Width)) / 2f;
            float offsetY = (info.Height - (locationUnit * vm.Height)) / 2f;

            SKColor ponyColor;
            SKColor ponyWallColor;
            switch (vm.SelectedPonyName)
            {
                case "Pinkie Pie":
                    ponyColor = PonyToPageBackgroundColorConverter.PinkiePiePageBackgroundColor.ToSKColor();
                    ponyWallColor = PonyToWallColorConverter.PinkiePieWallColor.ToSKColor();
                    break;
                case "Applejack":
                    ponyColor = PonyToPageBackgroundColorConverter.ApplejackPageBackgroundColor.ToSKColor();
                    ponyWallColor = PonyToWallColorConverter.ApplejackWallColor.ToSKColor();
                    break;
                case "Spike":
                    ponyColor = PonyToPageBackgroundColorConverter.SpikePageBackgroundColor.ToSKColor();
                    ponyWallColor = PonyToWallColorConverter.SpikeWallColor.ToSKColor();
                    break; 
                case "Rarity":
                default:
                    ponyColor = PonyToPageBackgroundColorConverter.RarityPageBackgroundColor.ToSKColor();
                    ponyWallColor = PonyToWallColorConverter.RarityWallColor.ToSKColor();
                    break;
            }

            using (SKPaint wallStroke = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                Color = ponyWallColor,
                StrokeWidth = WallWidth,
                StrokeCap = SKStrokeCap.Square
            })
            using (SKPaint exitFill = new SKPaint
            {
                Style=SKPaintStyle.Fill,
                Color=ponyColor
            })
            {
                //double outer rectangle
                canvas.DrawRect(offsetX, offsetY, vm.Width * locationUnit, vm.Height * locationUnit, wallStroke);
                canvas.DrawRect(offsetX - WallWidth, offsetY - WallWidth, vm.Width * locationUnit + WallWidth * 2, vm.Height * locationUnit + WallWidth * 2, wallStroke);

                for (int y = 0; y < vm.Height; y++)
                {
                    for (int x = 0; x < vm.Width; x++)
                    {
                        //It's enough to only draw walls for two sides (North or South and East or West), as the outer walls are already drawn.

                        if (x != vm.Width - 1 && vm.LatestSnapshot.Locations[x, y].EastWall)
                            canvas.DrawLine(new SKPoint() { X = offsetX + ((x + 1) * locationUnit), Y = offsetY + (y * locationUnit) }, new SKPoint() { X = offsetX + ((x + 1) * locationUnit), Y = offsetY + ((y + 1) * locationUnit) }, wallStroke);
                        if (y != vm.Height - 1 && vm.LatestSnapshot.Locations[x, y].SouthWall)
                            canvas.DrawLine(new SKPoint() { X = offsetX + (x * locationUnit), Y = offsetY + ((y + 1) * locationUnit) }, new SKPoint() { X = offsetX + ((x + 1) * locationUnit), Y = offsetY + ((y + 1) * locationUnit) }, wallStroke);
                    }
                }

                //Draw the exit
                canvas.DrawRect(offsetX + (vm.LatestSnapshot.Exit.X * locationUnit) + quarterLocationUnit, offsetY + (vm.LatestSnapshot.Exit.Y * locationUnit) + quarterLocationUnit, halfLocationUnit, halfLocationUnit, exitFill);
            }           
        }

        void OnCreatureCanvasPaintSurface(object sender, SKPaintSurfaceEventArgs args)
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
            float eighthLocationUnit = locationUnit / 8f;

            float offsetX = (info.Width - (locationUnit * vm.Width)) / 2f;
            float offsetY = (info.Height - (locationUnit * vm.Height)) / 2f;

            //                canvas.DrawRect(offsetX + (vm.LatestSnapshot.PonyPlacement.X * locationUnit) + quarterLocationUnit, offsetY + (vm.LatestSnapshot.PonyPlacement.Y * locationUnit) + quarterLocationUnit, halfLocationUnit, halfLocationUnit, ponyFill);

            //                canvas.DrawRect(offsetX + (vm.LatestSnapshot.DomokunPlacement.X * locationUnit) + quarterLocationUnit, offsetY + (vm.LatestSnapshot.DomokunPlacement.Y * locationUnit) + quarterLocationUnit, halfLocationUnit, halfLocationUnit, domokunFill);
            canvas.DrawBitmap(ponyBMP, new SKRect { Left = offsetX + (vm.LatestSnapshot.PonyPlacement.X * locationUnit) + eighthLocationUnit, Top = offsetY + (vm.LatestSnapshot.PonyPlacement.Y * locationUnit) + eighthLocationUnit, Right = offsetX + ((vm.LatestSnapshot.PonyPlacement.X + 1) * locationUnit) - eighthLocationUnit, Bottom = offsetY + ((vm.LatestSnapshot.PonyPlacement.Y + 1) * locationUnit) - eighthLocationUnit }, null);
            canvas.DrawBitmap(domokunBMP, new SKRect { Left = offsetX + (vm.LatestSnapshot.DomokunPlacement.X * locationUnit) + eighthLocationUnit, Top = offsetY + (vm.LatestSnapshot.DomokunPlacement.Y * locationUnit) + eighthLocationUnit, Right = offsetX + ((vm.LatestSnapshot.DomokunPlacement.X + 1) * locationUnit) - eighthLocationUnit, Bottom = offsetY + ((vm.LatestSnapshot.DomokunPlacement.Y + 1) * locationUnit) - eighthLocationUnit }, null);
        }

        private async void QuitButton_Clicked(object sender, EventArgs e)
        {
            vm.MakeRepeatingAutoMoves = false;
            string lostMessage = String.Format("Are you really, really sure you want to quit this maze? Then {0} will be left all alone in the cold & dark maze, chased by the scary Domo-Kun!", vm.SelectedPonyName);

            if (await DisplayAlert("Quit", lostMessage, "Quit", "Continue"))
                vm.ResetMaze();

        }
    }
}