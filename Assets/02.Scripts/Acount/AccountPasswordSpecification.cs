using System.Text.RegularExpressions;

public class AccountPasswordSpecification
{
    private const int MinLength = 8;
    private const int MaxLength = 20;

    // 비밀번호: 영문, 숫자를 각각 1자 이상 포함하고, 공백 없이 8~20자
    // 예) abc12345, Pass1234word
    private static readonly Regex PasswordRegex = new Regex(
        @"^(?=.*[A-Za-z])(?=.*\d)\S{" + MinLength + "," + MaxLength + "}$");

    public string ErrorMessage => $"비밀번호는 영문과 숫자를 포함한 공백 없는 {MinLength}~{MaxLength}자여야 합니다.";

    public bool IsSatisfiedBy(string password)
    {
        return !string.IsNullOrEmpty(password) && PasswordRegex.IsMatch(password);
    }
}
