using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SmartHome.Models;
using System.Security.Claims;
using System.Security.Cryptography; 
using System.Text;

public class LoginModel : PageModel
{
    private readonly SmartHome.Models.ApplicationContext _context;

    [BindProperty]
    public string Login { get; set; }

    [BindProperty]
    public string Password { get; set; }

    public string ErrorMessage { get; set; }

    public LoginModel ( ApplicationContext context )
    {
        _context = context;
    }

    public void OnGet ( )
    {
    }

    public async Task<IActionResult> OnPostAsync ( )
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.UsrLogin == Login);

        if (user != null)
        {
            string hashedInput = ComputeSha1Hash(Password);

            if (hashedInput.Equals(user.UsrPassword, StringComparison.OrdinalIgnoreCase))
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UsrLogin),
                    new Claim("UserId", user.UsrId.ToString())
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                return RedirectToPage("/Index");
            }
        }

        ErrorMessage = "Nieprawid³owy login lub has³o! [ODMOWA DOSTÊPU]";
        return Page();
    }

    private string ComputeSha1Hash ( string rawData )
    {
        using (SHA1 sha1Hash = SHA1.Create())
        {
            byte[] bytes = sha1Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2"));
            }
            return builder.ToString();
        }
    }
}