﻿namespace API.Services;

/// <summary>
/// A service to handle captcha requests
/// </summary>
public class CaptchaService
{
    private readonly string? _CaptchaSecret = Environment.GetEnvironmentVariable("CAPTCHA_SECRET");

    private readonly HttpClient _HttpClient;

    public CaptchaService(HttpClient captchaClient)
    {
        captchaClient.BaseAddress = new Uri("https://www.google.com/recaptcha/api/siteverify");
        _HttpClient = captchaClient;
    }

    /// <summary>
    /// Validate a captcha submission
    /// </summary>
    /// <param name="captcha">The captcha token</param>
    /// <returns>Whether or not the token was valid</returns>
    /// <exception cref="ApplicationException"></exception>
    public async Task<bool> Validate(string captcha)
    {
        if (_CaptchaSecret is null)
            throw new ApplicationException("Captcha secret env variable not set");

        try
        {
            HttpResponseMessage res = await _HttpClient.PostAsync(
                $"?secret={_CaptchaSecret}&response={captcha}",
                new StringContent(""));

            CaptchaResponse? result = JsonSerializer.Deserialize<CaptchaResponse>(await res.Content.ReadAsStringAsync());

            if (result is null)
                return false;
            else
                return result.Success;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// The expected response from the captcha API
    /// </summary>
    private class CaptchaResponse
    {
        public bool Success { get; set; }
    }
}