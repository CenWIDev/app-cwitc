﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CWITC.Clients.Portable;
using CWITC.DataObjects;
using CWITC.DataStore.Abstractions;
using CWITC.iOS;
using CWITC.iOS.DataStore.Firebase;
using Xamarin.Forms;

[assembly:Dependency(typeof(FavoriteStore))]

namespace CWITC.iOS.DataStore.Firebase
{
    public class FavoriteStore : BaseUserDataStore<Favorite>, IFavoriteStore
    {
        public override string Identifier => "user_favorites";

        public Task DropFavorites()
        {
            base.DropTable();

            return Task.CompletedTask;
        }

        public async Task<bool> IsFavorite(string sessionId)
        {
            var items = (await GetItemsAsync()) ?? new List<Favorite>();
            return items.Any(x => x.SessionId == sessionId);
        }
    }
}
