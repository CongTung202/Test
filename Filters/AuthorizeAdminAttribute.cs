using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace NemeShop.Filters
{
    public class AuthorizeAdminAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var session = context.HttpContext.Session.GetString("UserEmail");
            if (string.IsNullOrEmpty(session))
            {
                // Nếu chưa đăng nhập → chuyển về trang Login của Admin
                context.Result = new RedirectToActionResult("Login", "Account", new { area = "Admin" });
            }
            base.OnActionExecuting(context);
        }
    }
}
