using System;
using Xamarin.Forms;
using System.Globalization;
using System.Diagnostics;

namespace CWITC.Clients.UI
{
    /// <summary>
    /// Is favorite text converter.
    /// </summary>
    class IsFavoriteTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return string.Empty;
            
            return (bool)value ? "Unfavorite" : "Favorite";

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Is favorite detail text converter.
    /// </summary>
    class IsFavoriteDetailTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return string.Empty;
            
            return (bool)value ? "Remove from Favorites" : "Add to Favorites";

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

	class EventTitleAbbrDisplayConverter : IValueConverter
	{

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			try
			{
				if (!(value is string))
					return string.Empty;

                return value.ToString().Substring(0, 1);
			}
			catch (Exception ex)
			{
				Debug.WriteLine("Unable to convert: " + ex);
			}

			return string.Empty;
		}


		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}

