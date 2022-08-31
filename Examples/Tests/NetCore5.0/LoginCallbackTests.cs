using System.Text;
using Microsoft.AspNetCore.Http;
using Moq;
using Newtonsoft.Json;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Contract;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Contract.Implementation;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Middlewares.Login;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Middlewares.Login.Models;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Models.Request;
using TourmalineCore.AspNetCore.JwtAuthentication.Core.Options;
using TourmalineCore.AspNetCore.JwtAuthentication.Shared.Services;
using Xunit;

namespace Tests.NetCore5._0
{
    public class LoginCallbackTests
    {
        private readonly DefaultHttpContext _httpContext;
        private readonly Mock<Func<LoginModel, Task>> _onLoginExecutingMock;
        private readonly Mock<Func<LoginModel, Task>> _onLoginExecutedMock;
        private readonly LoginService _loginService;

        public LoginCallbackTests()
        {
            _httpContext = new DefaultHttpContext();
            _httpContext.Request.Method = HttpMethods.Post;
            _httpContext.Request.Path = new PathString("/login");

            var requestData = Encoding.Default.GetBytes(JsonConvert.SerializeObject(new LoginRequestModel
                        {
                            Login = "Admin",
                            Password = "Admin",
                        }
                    )
                );

            _httpContext.Request.Body = new MemoryStream(requestData);

            _onLoginExecutingMock = new Mock<Func<LoginModel, Task>>();

            _onLoginExecutingMock.Setup(p => p(It.IsAny<LoginModel>()))
                .Returns(Task.CompletedTask);

            _onLoginExecutedMock = new Mock<Func<LoginModel, Task>>();

            _onLoginExecutedMock.Setup(p => p(It.IsAny<LoginModel>()))
                .Returns(Task.CompletedTask);

            var authenticationOptions = new AuthenticationOptions()
            {
                PrivateSigningKey = "MIIEowIBAAKCAQEAsDwLnM5sbVi326YDsLvMkQLXDKVAaHrJZ/MwkoxF4Hmq4+pu4KojgQyVDtjseXG8UW5wbxW58eXG8V0XgJzsD8zQX2Z1bBawpIeD9sXf/5CFZGif85YFIqS3brqR3ScdGxYHXcwrUMGUCThxe918Q0aNXzdSxGGP2v7ZbtpFhLRyrTXHl4u6k3eyYG7zCkwextnMb9CJuCR7x1ua1V1S0xljAqg5PicFjt0vVSKzPM/Djw7XK84sJXxaet7t4cNtXVJIAyXUMsSli6gg9Cw9CEUSE40iWUR/6wrdUYAchk3vWiBhMmnufwzmFRLKHOH9Fz8buJVSrRfyt7a6S2iN+wIDAQABAoIBAQCvue/KV3p+Pex2tD8RxvDf13kfPtfOVkDlyfQw7HXwsuDXijctBfmJAEbRGzQQlHw2pmyuF3fl4DxTB4Qb1lz8FDniJoQHV0ijhgzrz7rfVffsevajKH/OX3gYjShM4GeBTqHhwWefiqZV21YtMFhrrLniq4N4FeAfeebNRg/zlWEigraxqAWb4cplnxBE3qOBECKXdF/B8uhp743BU/2HLSO5BUdhtPlN3FKoYdyqtrKyNO2z7rC+Gk8tNd+KbMHDUMiOQXzbXkpsXYKAug9iTW+gxZG/bNyzGNrJBFrUYb1fP4iZphbxBJgobNYJBKA565cAX/wI5lFakTBB0YAhAoGBAOk0TyV0dA8WJ6NrWmRUBKsKvkSREhBveW+P3LtA8a1IgQf4K6ohIfcq9w/+nRvTLPIxo67FcqEyzVUu9TOafzIi59w4RBWG/HKOZ5lvIVicbuPyclPVWyC+9bMMgWEJy9wGwE+fGh3AvAA4PXNBcjOqfT0sSF9PBUo5qN11Q/qHAoGBAMF2IL+cXgPiUta4XoMh14ksJiwHtZeMkj+kauU3rctDITSkIGMFp4q0W5UUSG1yPcW/++rMQfuAjCZotdNpbQT+g+KfG44DMT5W7nRgv60S0/6X/OoLIhCue19yLMVzFpai0YEH+s24/XNnwl53K34G1zVMCsZcIuIng8SZVintAoGAJP/1pr2pRFOBin4X418pNnIH6h0SPqVRIRA0N0mAjru4LSmE1ANZvjuE43bEOovwz6Rskegl3cmPpnpC0SMsFypOmzQaKUg3eX16lm95XPPE7EmlNgPd534kwXm0dU72lzxC+t8FZ78SlP5XUZgKpIPiRvhlqymAb1xinHBkjrUCgYAB144YRPTgNJd1U+wSc5AJzlHOuYQRHVWHJZme9RjChrEaPzXPu44M1ArLMJY/9IaCC4HqimdWbbLn6rdQfAB9u66lyb4JbB5b6Zf7o7Avha5fDjNqRxDb981U61Fhz+a3KHW2NM0+iDRhlOtU2u2fFZGXAFJZ8Saj4JxwksUvQQKBgEQ1TAW/INhWSkEW8vGeLnjV+rxOx8EJ9ftVCRaQMlDEDlX0n7BZeQrQ1pBxwL0FSTrUQdD02MsWshrhe0agKsw2Yaxn8gYs1v9HMloS4Q3L2zl8pi7R3yx72RIcdnS4rqGXeO5t8dm305Yz2RHhqtkBmpFBssSEYCY/tUDmsQVU",
            };

            _loginService = new LoginService(
                new TokenManager(authenticationOptions, new DefaultUserClaimsProvider(), new JwtTokenCreator(authenticationOptions)),
                new FakeUserCredentialValidator());
        }

        [Fact]
        public async Task LoginCallbackInvokeWithDefaultLogin()
        {
            RequestDelegate next = d => Task.CompletedTask;

            var loginMiddleware = new LoginMiddleware(next,
                    new LoginEndpointOptions
                    {
                        LoginEndpointRoute = "/login",
                    },
                    _onLoginExecutingMock.Object,
                    _onLoginExecutedMock.Object
                );

            await loginMiddleware.InvokeAsync(_httpContext, _loginService);

            _onLoginExecutingMock.Verify(x => x.Invoke(It.IsAny<LoginModel>()), Times.Once);
            _onLoginExecutedMock.Verify(x => x.Invoke(It.IsAny<LoginModel>()), Times.Once);
        }
    }
}