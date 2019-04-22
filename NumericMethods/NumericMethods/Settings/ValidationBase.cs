using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using Prism.Mvvm;
using Xamarin.Forms.Internals;

namespace NumericMethods.Settings
{
    public class ValidationBase : BindableBase, INotifyDataErrorInfo
    {
        public Dictionary<string, List<string>> Errors { get; set; } = new Dictionary<string, List<string>>();

        public ValidationBase()
        {
            ErrorsChanged += ValidationBase_ErrorsChanged;
        }

        #region INotifyDataErrorInfo Members

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public IEnumerable GetErrors(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                return Errors.SelectMany(err => err.Value.ToList()).ToList();
            }

            if (Errors.ContainsKey(propertyName) && (Errors[propertyName].Any()))
            {
                return Errors[propertyName].ToList();
            }

            return new List<string>();

        }
        public bool HasErrors
        {
            get
            {
                TestPropertiesForRequired();
                return Errors.Any(propErrors => propErrors.Value.Any());
            }
        }
        #endregion

        public IList<string> ErrorsList
        {
            get => GetErrors(string.Empty).Cast<string>().ToList();
        }

        protected virtual void ValidateProperty(object value, [CallerMemberName] string propertyName = null)
        {
            var validationContext = new ValidationContext(this, null)
            {
                MemberName = propertyName
            };

            var validationResults = new List<ValidationResult>();
            Validator.TryValidateProperty(value, validationContext, validationResults);

            RemoveErrorsByPropertyName(propertyName);

            HandleValidationResults(validationResults);
        }

        private void ValidationBase_ErrorsChanged(object sender, DataErrorsChangedEventArgs e)
        {
            RaisePropertyChanged(nameof(HasErrors));
            RaisePropertyChanged(nameof(Errors));
            RaisePropertyChanged(nameof(ErrorsList));
        }

        private void RemoveErrorsByPropertyName(string propertyName)
        {
            if (Errors.ContainsKey(propertyName))
            {
                Errors.Remove(propertyName);
            }

            RaiseErrorsChanged(propertyName);
        }

        private void HandleValidationResults(IEnumerable<ValidationResult> validationResults)
        {
            var resultsByPropertyName = from results in validationResults
                                        from memberNames in results.MemberNames
                                        group results by memberNames into groups
                                        select groups;

            foreach (var property in resultsByPropertyName)
            {
                Errors.Add(property.Key, property.Select(r => r.ErrorMessage).ToList());
                RaiseErrorsChanged(property.Key);
            }
        }

        private void RaiseErrorsChanged(string propertyName)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        private void TestPropertiesForRequired()
        {
            GetType().GetProperties()
                .Where(prop => prop.PropertyType == typeof(string) && prop.GetValue(this, null) == null)
                .ForEach(x => x.SetValue(this, string.Empty, null));
        }
    }
}
