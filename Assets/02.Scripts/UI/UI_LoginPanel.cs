using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 로그인 화면. 로그인 모드와 회원가입 모드를 바꾸고, 입력을 확인해 AccountStorage에 부탁한다.
public class UI_LoginPanel : MonoBehaviour
{
    [Header("입력 칸")]
    [SerializeField] private TMP_InputField _idInput;
    [SerializeField] private TMP_InputField _passwordInput;

    [Header("버튼")]
    [SerializeField] private Button _loginButton; // 로그인 모드: 로그인
    [SerializeField] private Button _registerButton; // 회원가입 모드: 가입하기

    [Header("기타")]
    [SerializeField] private TextMeshProUGUI _messageText; // 안내 글자
    [SerializeField] private string _gameSceneName = "CullingTestScene"; // 로그인하면 불러올 씬

    private AccountIdSpecification _idSpec = new AccountIdSpecification();
    private AccountPasswordSpecification _passwordSpec = new AccountPasswordSpecification();


    private void Start()
    {
        _loginButton.onClick.AddListener(Login);
        _registerButton.onClick.AddListener(Register);
        Debug.LogError("로그인씬");
    }


    // 로그인: 입력을 확인하고, 맞으면 게임 씬을 불러온다.
    private void Login()
    {
        string id = _idInput.text;
        string password = _passwordInput.text;

        if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(password))
        {
            _messageText.text = "아이디와 비밀번호를 입력해 주세요.";
            return;
        }

        if (!AccountStorage.CheckPassword(id, password))
        {
            // 아이디가 없는지 비밀번호가 틀렸는지는 알려 주지 않는다.
            _messageText.text = "아이디 또는 비밀번호가 맞지 않습니다.";
            return;
        }

        SceneLoader.Load(_gameSceneName); // 로딩 화면을 거쳐서 넘어간다.
    }

    // 회원가입: 입력을 확인하고, 문제가 없으면 계정을 저장한 뒤 로그인 모드로 돌아간다.
    private void Register()
    {
        string id = _idInput.text;
        string password = _passwordInput.text;

        if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(password))
        {
            _messageText.text = "아이디와 비밀번호를 입력해 주세요.";
            return;
        }

        if (!_idSpec.IsSatisfiedBy(id))
        {
            _messageText.SetText(_idSpec.ErrorMessage);
            return;
        }

        if (!_passwordSpec.IsSatisfiedBy(password))
        {
            _messageText.SetText(_passwordSpec.ErrorMessage);
            return;
        }

        if (AccountStorage.Exists(id))
        {
            _messageText.text = "이미 있는 아이디입니다.";
            return;
        }

        AccountStorage.Register(id, password);
        _messageText.text = "가입했습니다. 로그인해 주세요.";
    }
}