using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Application.Interfaces
{
    public interface IJwtService
    {
        string GenerateAccessToken(string username, string role);
        string GenerateRefreshToken();
    }
}
