﻿using Azure.Core;
using Microsoft.Identity.Client;
using Microsoft.Identity.Web;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCoreAzureStorage.FilesProvider.AzureStorageAccess
{
    public class TokenAcquisitionTokenCredential : TokenCredential
    {
        private ITokenAcquisition _tokenAcquisition;

        /// <summary>
        /// Constructor from an ITokenAcquisition service.
        /// </summary>
        /// <param name="tokenAcquisition">Token acquisition.</param>
        public TokenAcquisitionTokenCredential(ITokenAcquisition tokenAcquisition)
        {
            _tokenAcquisition = tokenAcquisition;
        }

        public override AccessToken GetToken(TokenRequestContext requestContext, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public override async ValueTask<AccessToken> GetTokenAsync(TokenRequestContext requestContext, CancellationToken cancellationToken)
        {
            AuthenticationResult result = await _tokenAcquisition
                .GetAuthenticationResultForUserAsync(requestContext.Scopes).ConfigureAwait(false);
            return new AccessToken(result.AccessToken, result.ExpiresOn);
        }
    }
}