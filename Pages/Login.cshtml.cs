using IntegrationProject.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IntegrationProject.Pages
{
    public class LoginModel : PageModel
    {

        private readonly IHttpContextAccessor _accessor;
        private readonly UserService _userService;
        public LoginModel(IHttpContextAccessor accessor, UserService userService)
        {
            _accessor = accessor;
            _userService = userService;
        }
        [BindProperty]
        public UserLogin UserLogin { get; set; }

        public string ErrorMessage;
        public async Task<IActionResult> OnGetAsync()
        {
            if (_accessor.HttpContext.User.Identity.IsAuthenticated)
            {
                return Redirect("/");
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            ErrorMessage = await _userService.UserLoginAsync(UserLogin);

            return Redirect("/");
        }
    }
}