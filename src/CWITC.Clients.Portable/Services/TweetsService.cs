using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CWITC.Clients.Portable.Services
{
    public interface ITweetsService
    {
        Task<IEnumerable<Tweet>> GetTweets();
    }
}
