using System;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using SongSpiration.BLL.DTOs;
using SongSpiration.BLL.Interfaces;

namespace SongSpiration.BLL.Services;

public class EmailSender : IEmailSender
{
    private readonly EmailSettings _emailSettings;

    public EmailSender(IOptions<EmailSettings> emailSettings)
    {
        _emailSettings = emailSettings.Value;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string message)
    {
        try
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(_emailSettings.FromName, _emailSettings.FromEmail));
            emailMessage.To.Add(new MailboxAddress("", toEmail));
            emailMessage.Subject = subject;

            var bodyBuilder = new BodyBuilder { HtmlBody = message };
            emailMessage.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            
            // For dev/testing with services like mailtrap, StartTls is typical.
            // If using a service that doesn't support it or for local testing, 
            // you might need SecureSocketOptions.Auto or None.
            await client.ConnectAsync(_emailSettings.Host, _emailSettings.Port, SecureSocketOptions.Auto);
            
            if (!string.IsNullOrEmpty(_emailSettings.User) && !string.IsNullOrEmpty(_emailSettings.Password))
            {
                await client.AuthenticateAsync(_emailSettings.User, _emailSettings.Password);
            }

            await client.SendAsync(emailMessage);
            await client.DisconnectAsync(true);
        }
        catch (Exception ex)
        {
            // In a real application, you'd log this exception
            Console.WriteLine($"Failed to send email: {ex.Message}");
            throw new InvalidOperationException($"Could not send email: {ex.Message}", ex);
        }
    }
}