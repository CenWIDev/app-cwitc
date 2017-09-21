using System;
using System.Windows.Input;

namespace CWITC.Clients.Portable
{
	public interface ILocationViewModel
	{
		string Title { get; set; }
		string Description { get; }
		double Latitude { get; }
		double Longitude { get; }
		ICommand Command { get; }
	}
}
