using Notung.Feuerzauber.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace Notung.Feuerzauber.Configuration
{
    /// <summary>
    /// Класс расширения для отслеживания изменения связанного значения 
    /// </summary>
    public static class BindingHelper
    {
        /// <summary>
        /// Свойство отслеживаемой привязки
        /// </summary>
        #region Property BoundValueProperty
        public static readonly DependencyProperty BoundValueProperty =
            DependencyProperty.RegisterAttached(
                "BoundValue", typeof(object), typeof(BindingHelper),
                new PropertyMetadata(null, OnBoundValueChanged));

        public static object GetBoundValue(DependencyObject obj)
        {
            return obj.GetValue(BoundValueProperty);
        }

        public static void SetBoundValue(DependencyObject obj, object value)
        {
            obj.SetValue(BoundValueProperty, value);
        }

        private static void OnBoundValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var handler = GetBoundValueChangedHandler(obj);
            handler?.Invoke(obj, EventArgs.Empty);
        }
        #endregion
        /// <summary>
        /// Свойсво для события обновления поля
        /// </summary>
        #region Property BoundValueChangedHandlerProperty
        public static readonly DependencyProperty BoundValueChangedHandlerProperty =
            DependencyProperty.RegisterAttached(
                "BoundValueChangedHandler", typeof(EventHandler), typeof(BindingHelper));

        public static EventHandler GetBoundValueChangedHandler(DependencyObject obj)
        {
            return (EventHandler)obj.GetValue(BoundValueChangedHandlerProperty);
        }

        public static void SetBoundValueChangedHandler(DependencyObject obj, EventHandler value)
        {
            obj.SetValue(BoundValueChangedHandlerProperty, value);
        }
        #endregion


        /// <summary>
        /// Свойство отслеживаемой привязки
        /// </summary>
        #region Property BoundValueProperty
        public static readonly DependencyProperty SourceModelItemProperty =
            DependencyProperty.RegisterAttached(
                "SourceModelItem", typeof(ModelItem), typeof(BindingHelper),
                new PropertyMetadata(null, OnSourceModelItemPropertyChanged));

        public static ModelItem GetSourceModelItem(DependencyObject obj)
        {
            return (ModelItem)obj.GetValue(SourceModelItemProperty);
        }

        public static void SetSourceModelItem(DependencyObject obj, ModelItem value)
        {
            obj.SetValue(SourceModelItemProperty, value);
        }

        private static void OnSourceModelItemPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {

            if (obj is FrameworkElement fe)
            {
                var propertyName = GetPropertyName(fe);
                if (string.IsNullOrWhiteSpace(propertyName))
                    return;

                PropertyDescriptor property = TypeDescriptor.GetProperties(fe)[propertyName];
                DependencyPropertyDescriptor dependencyProperty = DependencyPropertyDescriptor.FromProperty(property);

                if (e.OldValue is ModelItem ov && ov != null)
                {
                    UnsetNewBinding(fe, dependencyProperty);
                }
                if (e.NewValue is ModelItem nv)
                {

                    SetNewBinding(nv, fe, dependencyProperty);

                }
            }

                
           
        }

        private static void SetNewBinding(ModelItem nv, FrameworkElement fe, DependencyPropertyDescriptor dependencyProperty)
        {
            if (nv != null)
            {

              
                Binding binding = new Binding(nv.Path);
                binding.Source = nv.Source;
                binding.Mode = BindingMode.TwoWay;
                fe.SetBinding(dependencyProperty.DependencyProperty, binding);

            }
        }
        private static void UnsetNewBinding( FrameworkElement fe, DependencyPropertyDescriptor dependencyProperty)
        {

                BindingOperations.ClearBinding(fe, dependencyProperty.DependencyProperty);
   
        }
        #endregion

        /// <summary>
        /// Имя свойсва привязки конечного элимента
        /// </summary>
        #region Property BoundValueChangedHandlerProperty
        public static readonly DependencyProperty PropertyNameProperty =
            DependencyProperty.RegisterAttached(
                "PropertyName", typeof(string), typeof(BindingHelper), new PropertyMetadata(string.Empty, OnPropertyNameChanged));

        public static string GetPropertyName(DependencyObject obj)
        {
            return (string)obj.GetValue(PropertyNameProperty);
        }

        public static void SetPropertyName(DependencyObject obj, string value)
        {
            obj.SetValue(PropertyNameProperty, value);
        }


        private static void OnPropertyNameChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {

            if (obj is FrameworkElement fe)
            {
                var mi = GetSourceModelItem(fe);

                if (e.OldValue is string ov && !String.IsNullOrWhiteSpace(ov))
                {
                    PropertyDescriptor property = TypeDescriptor.GetProperties(fe)[ov];
                    DependencyPropertyDescriptor dependencyProperty = DependencyPropertyDescriptor.FromProperty(property);

                    UnsetNewBinding(fe, dependencyProperty);
                }
                if (e.NewValue is string nv && !String.IsNullOrWhiteSpace(nv))
                {
                    PropertyDescriptor property = TypeDescriptor.GetProperties(fe)[nv];
                    DependencyPropertyDescriptor dependencyProperty = DependencyPropertyDescriptor.FromProperty(property);

                    SetNewBinding(mi, fe, dependencyProperty);
                }
            }



        }
        #endregion

    }
}
