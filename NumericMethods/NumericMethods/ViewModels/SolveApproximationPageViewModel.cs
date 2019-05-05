using System;
using System.Collections.Generic;
using NumericMethods.Interfaces;
using NumericMethods.Models;
using NumericMethods.Settings;
using Prism.Navigation;
using Prism.Services;

namespace NumericMethods.ViewModels
{
    public class SolveApproximationPageViewModel : BaseViewModel
    {
        private readonly ICommonFunctions _commonFunctions;

        public SolveApproximationPageViewModel(
            INavigationService navigationService,
            IPageDialogService pageDialogService,
            ICommonFunctions commonFunctions)
            : base(navigationService, pageDialogService)
        {
            _commonFunctions = commonFunctions;
            Prepare();
        }

        private List<Operation> _operations = new List<Operation>();

        private void Prepare()
        {
            _operations.Add(new Operation { Value = 1, Weight = 2 });
            _operations.Add(new Operation { Value = -4, Weight = 1 });
            _operations.Add(new Operation { Value = 4, Weight = 0 });

            var derivative = _commonFunctions.CalculateDerivative(_operations);
            Steffensen(derivative, 1);
        }

        private float Steffensen(List<Operation> op, float x0)
        {
            float f0, f1, g;

            f0 = _commonFunctions.FunctionResult(x0, op);
            uint iterations = 0;

            while (Math.Abs(f0) > AppSettings.Epsilon && iterations < 50000)
            {
                f1 = _commonFunctions.FunctionResult(x0 + f0, op);
                g = (f1 - f0) / f0;
                x0 = x0 - f0 / g;
                f0 = _commonFunctions.FunctionResult(x0, op);
                iterations++;
                if (iterations == 50000)
                {
                    break;
                }
            }

            return x0;

        }
    }
}
