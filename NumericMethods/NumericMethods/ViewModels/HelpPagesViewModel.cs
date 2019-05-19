using Prism.Navigation;
using Prism.Services;

namespace NumericMethods.ViewModels
{
    public class HelpPagesViewModel : BaseViewModel
    {
        public HelpPagesViewModel(
            INavigationService navigationService,
            IPageDialogService pageDialogService)
            : base(navigationService, pageDialogService)
        {
        }
    }
}
