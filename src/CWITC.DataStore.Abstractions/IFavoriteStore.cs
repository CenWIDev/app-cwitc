using System;
using System.Threading.Tasks;
using CWITC.DataObjects;
using System.Collections.Generic;

namespace CWITC.DataStore.Abstractions
{
    public interface IFavoriteStore : IBaseStore<Favorite>
    {
        Task<bool> IsFavorite(string sessionId);
    }
}

