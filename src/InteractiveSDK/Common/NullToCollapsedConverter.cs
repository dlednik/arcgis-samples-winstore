using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace InteractiveSDK.Common
{
    /// <summary>
    /// Value converter that translates true to <see cref="Visibility.Visible"/> and false to
    /// <see cref="Visibility.Collapsed"/>.
    /// </summary>
	public sealed class NullToCollapsedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
			if(value is string)
				return string.IsNullOrEmpty(value as string) ? Visibility.Collapsed : Visibility.Visible;
            return (value != null) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
			throw new NotImplementedException();
        }
    }
}
