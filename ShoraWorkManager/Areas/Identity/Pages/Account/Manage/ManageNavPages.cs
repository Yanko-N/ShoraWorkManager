// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace  ShoraWorkManager.Areas.Identity.Pages.Account.Manage
{
   /// <summary>
   /// Class auto-gerated via IdentityScaffolding
   /// </summary>
    public static class ManageNavPages
    {
        public static string Index => "Index";
        public static string Email => "Email";
        public static string ChangePassword => "ChangePassword";

        public static string IndexPageControl => "IndexPageControl";
        public static string CreateAuthorizationToken => "CreateAuthorizationToken";
        public static string Account => "Manage Accounts";

        public static string IndexNavClass(ViewContext viewContext) => PageNavClass(viewContext, Index);
        public static string EmailNavClass(ViewContext viewContext) => PageNavClass(viewContext, Email);
        public static string IndexPageControlNavClass(ViewContext viewContext) => PageNavClass(viewContext, IndexPageControl);
        public static string AccountControlNavClass(ViewContext viewContext) => PageNavClass(viewContext, Account);

        public static string ChangePasswordNavClass(ViewContext viewContext) => PageNavClass(viewContext, ChangePassword);
        public static string CreateCreateAuthorizationNavClass(ViewContext viewContext) => PageNavClass(viewContext, CreateAuthorizationToken);

        public static string PageNavClass(ViewContext viewContext, string page)
        {
            var activePage = viewContext.ViewData["ActivePage"] as string
                ?? System.IO.Path.GetFileNameWithoutExtension(viewContext.ActionDescriptor.DisplayName);
            return string.Equals(activePage, page, StringComparison.OrdinalIgnoreCase) ? "active" : null;
        }
    }
}
