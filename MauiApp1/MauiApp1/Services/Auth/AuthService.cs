﻿using Identity_Application.Contracts.User.Commands.ChangePassword;
using Identity_Application.Contracts.User.Commands.LoginUser;
using Identity_Application.Contracts.User.Commands.RegisterUser;
using Identity_Application.Contracts.User.Queries.CheckUserDetails;
using Identity_Application.Contracts.User.Responses;
using MauiApp1.Identity;
using MauiApp1.Services.Profile;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Json;

namespace MauiApp1.Services.Auth
{
    public class AuthService(IdentityHttpClient identityHttpClient,IUserProfileService userProfileService)
      : IAuthService
    {
        public async Task<HttpResponseMessage> ChangePassword(ChangePasswordUserCommand command)
        {

            var result = await identityHttpClient.PutAsJsonAsync("Auth/ChangePassword", command);
            return result;
        }



        public async Task<string> LoginUserAsync(LoginUserCommand command, bool newRegister = false)
        {
            string errorMessage = null;
            try
            {
                var result = await identityHttpClient.PostAsJsonAsync("Auth/login", command);
                if (result.IsSuccessStatusCode)
                {
                    var responseBody = await result.Content.ReadAsStringAsync();
                    var authResponse = JsonConvert.DeserializeObject<LoginPage.AuthResponse>(responseBody);
                    return authResponse.AccessToken;
                }
                if (result.StatusCode == HttpStatusCode.Unauthorized)
                {
                    errorMessage = (await result.Content.ReadFromJsonAsync<CustomProblemDetails>()).Detail;
                }
            }
            catch (HttpRequestException ex)
            {
                errorMessage = "Server is not responding, please try later";
            }
            catch (Exception e)
            {
                errorMessage = "An error occurred";
            }

            return errorMessage;
        }

        public async Task<string> RegisterUserAsync(RegisterUserCommand command)
        {
            try
            {
                command.PhoneNumber = command.PhoneNumber.Trim();
                if (command.PhoneNumber.Length == 9) command.PhoneNumber = "+992" + command.PhoneNumber.Trim();
                else if (command.PhoneNumber.Length == 12 && command.PhoneNumber[0] != '+') command.PhoneNumber = "+" + command.PhoneNumber;

                var result = await identityHttpClient.PostAsJsonAsync("Auth/register", command);
                if (result.IsSuccessStatusCode)
                {
                    await LoginUserAsync(new LoginUserCommand()
                    {
                        Password = command.Password,
                        Username = command.Username
                    });

                    return "";
                }
                if (result.StatusCode is not (HttpStatusCode.Unauthorized or HttpStatusCode.BadRequest))
                    return "server error, please try again later";

                var response = await result.Content.ReadFromJsonAsync<CustomProblemDetails>();
                return response.Detail;
            }
            catch (HttpRequestException ex)
            {
                return "Server is not responding, please try later";
            }
            catch (Exception e)
            {
                return "An error occurred";
            }
        }

        public async Task<HttpResponseMessage> CheckUserName(string userName)
        {
            var result = await identityHttpClient.GetAsync($"User/CheckUserName/{userName}");
            return result;
        }

        public async Task<HttpResponseMessage> CheckUserDetails(CheckUserDetailsQuery query)
        {
            var result = await identityHttpClient.GetAsync($"User/CheckUserDetails/{query.UserName}/{query.PhoneNumber}/{query.Email}");
            return result;
        }


    }
}
