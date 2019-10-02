using System;

using MvvmHelpers;

namespace CWITC.DataObjects
{

	public interface IBaseDataObject
	{
		string Id { get; set; }
	}

	public abstract class BaseDataObject : ObservableObject, IBaseDataObject
	{
		public virtual string Id { get; set; }
	}
}
