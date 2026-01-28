using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace NorbitApi;

public class AuthOptions
{
    public const string ISSUER = "NorbitSSO"; // издатель токена
    public const string AUDIENCE = "DesktopWPF"; // потребитель токена
    const string KEY = "MZC224IO8vNbkb2xOpZa6ERpTFvolrgdGF2zOVqt5ZMI1ivleuse4w7oPqjIF64k";   // ключ для шифрации
    public static SymmetricSecurityKey GetSymmetricSecurityKey() => 
        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(KEY));
}