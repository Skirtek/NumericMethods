using System.Threading.Tasks;
using NumericMethods.Resources;
using Prism.Commands;
using Prism.Services;

namespace NumericMethods.ViewModels
{
    public class MainMenuPageViewModel : BaseViewModel
    {
        public MainMenuPageViewModel(IPageDialogService pageDialogService) : base(pageDialogService)
        {
            AboutAppCommand = new DelegateCommand(async () => await AboutApp());
        }

        public DelegateCommand AboutAppCommand { get; set; }

        private async Task AboutApp()
        {
            await ShowAlert(AppResources.Menu_AboutApp, "Twórcami aplikacji są");
        }
    }
}
