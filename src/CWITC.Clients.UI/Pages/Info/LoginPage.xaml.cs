using System;
using System.Collections.Generic;

using Xamarin.Forms;
using CWITC.Clients.Portable;
using System.Text.RegularExpressions;

namespace CWITC.Clients.UI
{
	public partial class LoginPage : ContentPage
	{
		const string emailRegex = @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
			@"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$";

		LoginViewModel vm;
		public LoginPage()
		{
			InitializeComponent();
			BindingContext = vm = new LoginViewModel(Navigation);
		}
	}
}
