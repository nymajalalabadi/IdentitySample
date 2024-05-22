﻿using System.Net.Http;
using System;
using IdentitySample.Authorization.ClaimBasedAuthorization.Utilities.MvcNamesUtilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace IdentitySample.Authorization.ClaimBasedAuthorization.Utilities
{
    public class ClaimBasedAuthorizationUtilities : IClaimBasedAuthorizationUtilities
    {
        private readonly IMvcUtilities _mvcUtilities;

        public ClaimBasedAuthorizationUtilities(IMvcUtilities mvcUtilities)
        {
            _mvcUtilities = mvcUtilities;
        }

        public string GetClaimToAuthorize(HttpContext httpContext)
        {
            var areaName = httpContext.GetRouteValue("area")?.ToString();
            var controllerName = httpContext.GetRouteValue("controller")?.ToString();
            var actionName = httpContext.GetRouteValue("action")?.ToString();

            //var claimToAuthorize = _mvcUtilities.MvcInfoForActionsThatRequireClaimBasedAuthorization
            //    .Where(x => 
            //        x.AreaName == areaName && x.ControllerName == controllerName && x.ActionName == actionName)
            //    .SingleOrDefault();

            _mvcUtilities.MvcInfoForActionsThatRequireClaimBasedAuthorization
                .TryGetValue(new MvcNamesModel(areaName, controllerName, actionName),
                    out var actualValue);

            return actualValue?.ClaimToAuthorize;
        }

    }

}
