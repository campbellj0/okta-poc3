using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OktaSampleApi.Services
{
    // How to pass/verify Open ID token between .net core web app and web api?
    // https://stackoverflow.com/questions/59687103/how-to-pass-verify-open-id-token-between-net-core-web-app-and-web-api
    public interface ICurrentUserService
    {
        public ClaimsPrincipal GetCurrentUser();

        public string GetCurrentUserDisplayName();

        public string GetCurrentUserFullName();

        public string GetCurrentUserId();

        public DateTime? GetCurrentUserDob();

        public string GetCurrentUserGender();

        //public AddressFromClaimsDTO GetCurentUserAddress();

        public bool IsAuthenticated();
    }

    public class CurrentUserService : ICurrentUserService
    {

        private const string FULL_ADDRESS_CLAIM_TYPE = "address";

        private readonly IHttpContextAccessor _context;

        public CurrentUserService(IHttpContextAccessor context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets whether or not the current user context is authenticated.
        /// </summary>
        /// <returns></returns>
        public bool IsAuthenticated()
        {
            return GetCurrentUser().Identity.IsAuthenticated;
        }

        /// <summary>
        /// Gets the current user's address.
        /// TODO: tie this into our address data model... but if addresses live in Okta what does that mean?
        /// </summary>
        /// <returns></returns>
        //public AddressFromClaimsDTO GetCurentUserAddress()
        //{
        //    var addressClaim = GetClaim(FULL_ADDRESS_CLAIM_TYPE);

        //    if (addressClaim != null)
        //    {
        //        //var parseValue = addressClaim.Value.ToString().Replace("{address:", "{\"address\":");
        //        var address = JsonSerializer.Deserialize<AddressFromClaimsDTO>(addressClaim.Value.ToString());
        //        return address;
        //    }
        //    else
        //    {
        //        return new AddressFromClaimsDTO();
        //    }
        //}

        public ClaimsPrincipal GetCurrentUser()
        {
            return _context.HttpContext.User;
        }

        public string GetCurrentUserDisplayName()
        {
            return GetCurrentUser().Identity.Name;
        }

        public string GetCurrentUserFullName()
        {
            throw new NotImplementedException();
        }

        public string GetCurrentUserId()
        {
            throw new NotImplementedException();
        }

        public DateTime? GetCurrentUserDob()
        {
            var claim = GetClaim("birthdate");

            if (claim != null && !string.IsNullOrEmpty(claim.Value))
            {
                return DateTime.Parse(claim.Value);
            }
            else
            {
                return null;
            }
        }

        public string GetCurrentUserGender()
        {
            return GetClaim("gender")?.Value.ToString();
        }


        public Claim GetClaim(string claimType)
        {
            return _context.HttpContext.User.FindFirst(x => x.Type == claimType);
        }

    }
}
