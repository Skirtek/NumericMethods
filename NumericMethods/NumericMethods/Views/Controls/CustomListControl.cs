using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using Xamarin.Forms;

namespace NumericMethods.Views.Controls
{
    public class CustomListControl : StackLayout
    {
        public static readonly BindableProperty ItemsSourceProperty =
           BindableProperty.Create(
               nameof(ItemsSource),
               typeof(IEnumerable<object>),
               typeof(CustomListControl));

        public IEnumerable<object> ItemsSource
        {
            get => (IEnumerable<object>)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public static readonly BindableProperty ItemTemplateProperty =
            BindableProperty.Create(
                nameof(ItemTemplate),
                typeof(DataTemplate),
                typeof(CustomListControl));

        public DataTemplate ItemTemplate
        {
            get => (DataTemplate)GetValue(ItemTemplateProperty);
            set => SetValue(ItemTemplateProperty, value);
        }

        protected override void OnPropertyChanged(string propertyName = null)
        {
            if (propertyName == ItemsSourceProperty.PropertyName)
            {
                if (ItemsSource != null && ItemsSource is INotifyCollectionChanged collection)
                {
                    BuildStack();
                    collection.CollectionChanged += ItemsSourceCollectionChanged;
                }
            }
            base.OnPropertyChanged(propertyName);
        }

        private void ItemsSourceCollectionChanged(object s, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    var index = e.NewStartingIndex;
                    foreach (var newItem in e.NewItems)
                    {
                        Children.Insert(index++, CreateCellView(newItem));
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    Children.RemoveAt(e.OldStartingIndex);
                    break;

                case NotifyCollectionChangedAction.Reset:
                    BuildStack();
                    break;
            }
        }

        private void BuildStack()
        {
            Children.Clear();

            foreach (var item in ItemsSource)
            {
                Children.Add(CreateCellView(item));
            }
        }

        private View CreateCellView(object item)
        {
            var view = (View)ItemTemplate.CreateContent();
            var bindableObject = (BindableObject)view;

            if (bindableObject != null)
            {
                bindableObject.BindingContext = item;
            }

            return view;
        }
    }
}
