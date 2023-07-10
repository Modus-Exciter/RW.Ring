using Notung.Feuerzauber.Configuration;
using Notung.Feuerzauber.Controls;
using Notung.Feuerzauber.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace Notung.Feuerzauber.Configuration
{
    /// <summary>
    /// Класс для обеспечения ContentControl функциями содержимого вкладки окна параметров.
    /// </summary>
    public static class ContentControlHelper
    {
        /// <summary>
        /// Свойства автоматической привязки параметров 
        /// </summary>
        public static readonly DependencyProperty SettingsControllerProperty =
       DependencyProperty.RegisterAttached("SettingsController", typeof(SettingsDialogViewModel), typeof(ContentControlHelper), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSettingsControllerChanged));

        public static SettingsDialogViewModel GetSettingsController(DependencyObject obj)
        {
            return obj.GetValue(SettingsControllerProperty) as SettingsDialogViewModel;
        }

        public static void SetSettingsController(DependencyObject obj, SettingsDialogViewModel value)
        {
            obj.SetValue(SettingsControllerProperty, value);
        }

        private static void OnSettingsControllerChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            ContentControl cc = obj as ContentControl;
            if (cc != null)
            {

                if (e.OldValue is SettingsDialogViewModel oldV && oldV != null)
                {   //Дейсвия для ного обьекта
                    oldV.PropertyChanged -= SettingsControllerPropertyChanged;
                    //Дейсвия для старого обьекта
                }

                if (e.NewValue is SettingsDialogViewModel newV && newV != null)
                {
                    //Дейсвия для ного обьекта
                    newV.PropertyChanged += SettingsControllerPropertyChanged;


                }

                void SettingsControllerPropertyChanged(object sender, PropertyChangedEventArgs ee)
                {
                    SettingsDialogViewModel s = sender as SettingsDialogViewModel;
                    if (s == null)
                        return;
                    switch (ee.PropertyName)
                    {
                        case nameof(s.СonfigurationPageSelected):
                            if (s.СonfigurationPageSelected == null)
                                return;
                            ConfigurationPageViewAttribute attribute = s.СonfigurationPageSelected.GetType().GetCustomAttribute<ConfigurationPageViewAttribute>();
                            FrameworkElement fe = null;
                            if (cc.Content is FrameworkElement ccc && ccc != null)
                            {
                                ccc.Loaded += ContentPresenterLoaded;
                            }

                            if (attribute != null) //Пользовательская форма
                            {
                                fe = Activator.CreateInstance(attribute.Type) as FrameworkElement;


                            }
                            else //Стандарная форма
                            {

                                fe = new SettingsDefaultPage();

                            }
                            fe.Loaded += ContentPresenterLoaded;
                            cc.Content = fe;
                            var content = cc.Content as Control;
                            content.DataContext = s.СonfigurationPageSelected.Sections;
                            break;
                    }
                }
                void ContentPresenterLoaded(object sender, RoutedEventArgs ee)
                {
                    GetBindingsRecursive(sender as DependencyObject, cc);
                }



            }
        }


        private static void GetBindingsRecursive(DependencyObject element, ContentControl cc)
        {
            if (element == null)
                return;

            int childCount = VisualTreeHelper.GetChildrenCount(element);
            for (int i = 0; i < childCount; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(element, i);
                if (child is FrameworkElement fe && fe !=null)
                {
                  
                    foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(fe))
                    {
                        DependencyPropertyDescriptor dp1 = DependencyPropertyDescriptor.FromProperty(property);

                      
                        if (dp1 != null && dp1.Name != "ItemsSource")
                        {
                           
                            var bindingExpression = BindingOperations.GetBindingExpression(child, dp1.DependencyProperty);
                            if (bindingExpression != null && !string.IsNullOrWhiteSpace(bindingExpression.ParentBinding.Path.Path))
                            {


                                if (bindingExpression.DataItem is DependencyObject) //Если обьект привязки связан с DependencyObject то игнорируем дальнейшую привязку
                                {
                                    continue;
                                }
                             
                                Binding binding1 = new Binding(bindingExpression.ParentBinding.Path.Path);
                              

                                if (bindingExpression.ParentBinding.Source != null)
                                {
                                    binding1.Source = bindingExpression.ParentBinding.Source;

                                }
                                  
                                binding1.Mode = BindingMode.TwoWay;
                                fe.SetBinding(BindingHelper.BoundValueProperty, binding1);

                                BindingHelper.SetBoundValueChangedHandler(fe, (s, e) => {
                                    GetSettingsValueChanged(cc)?.Execute(null);
                                });




                            }
                        
                        }
                      
                      
                        
                    }

                    GetBindingsRecursive(child, cc);
                }
            }
        }
        /// <summary>
        /// Команда вызывающася при изменени любой привязки в подченной форме 
        /// </summary>
        public static readonly DependencyProperty SettingsValueChangedProperty =
    DependencyProperty.RegisterAttached("SettingsValueChanged", typeof(ICommand), typeof(ContentControlHelper), new FrameworkPropertyMetadata(null));

        public static ICommand GetSettingsValueChanged(DependencyObject obj)
        {
            return obj.GetValue(SettingsValueChangedProperty) as ICommand;
        }

        public static void SetSettingsValueChanged(DependencyObject obj, ICommand value)
        {
            obj.SetValue(SettingsValueChangedProperty, value);
        }

    }
}
