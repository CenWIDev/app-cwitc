using System;
using System.Threading.Tasks;
using CWITC.DataObjects;

namespace CWITC.Shared.DataStore.Firebase
{
    public abstract class ReadonlyStore<T> : BaseStore<T> where T: IBaseDataObject
    {
        public override Task<bool> RemoveAsync(T item)
        {
            throw new NotSupportedException();
        }

        public override Task<bool> UpdateAsync(T item)
        {
            throw new NotSupportedException();
        }

        public override Task<bool> InsertAsync(T item)
        {
            throw new NotSupportedException();
        }
    }

}
