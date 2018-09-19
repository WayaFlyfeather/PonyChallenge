using PonyChallenge.Services;
using PonyChallenge.ViewModels;
using PonyChallenge.Views;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace PonyChallenge
{
    public partial class App : Application
    {
        readonly public IPonyMazeService PonyMazeService = new HTTPPonyMazeService();

        public App()
        {
            InitializeComponent();

            MainPage = new NewMazePage() { BindingContext = new PonyMazeViewModel() };
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
