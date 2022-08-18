using System.ComponentModel.DataAnnotations;

namespace PUSH_SERVER_WEB.Models
{
    public class LoginForm
    {
        [Required(ErrorMessage = "로그인 필드가 채워져야 합니다")]
        public string? Identifier { get; set; }
        [Required(ErrorMessage = "비밀번호 필드가 채워져야 합니다")]
        public string? Password { get; set; }
    }
}