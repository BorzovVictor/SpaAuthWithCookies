using Microsoft.AspNetCore.Identity;

namespace ApiCookiesAuth.Settings
{
    public class RusIdentityErrorDescriber: IdentityErrorDescriber
    {
        public override IdentityError DuplicateUserName(string userName) { return new IdentityError { Code = nameof(DuplicateUserName), Description = $"Пользователь с именем '{userName}' уже существует." }; }
        public override IdentityError DuplicateEmail(string email) { return new IdentityError { Code = nameof(DuplicateEmail), Description = $"Email '{email}' уже зарегистрирован." }; }
    }
}