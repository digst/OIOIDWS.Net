﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digst.OioIdws.Rest.ProviderAuthentication
{
    public interface ITokenProvider
    {
        Task<string> RetrieveTokenAsync(string accessToken);
    }
}
