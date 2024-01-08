using Microsoft.Extensions.Options;
using SendGrid.Helpers.Mail;
using SendGrid;
using Ticketing.BAL.Contracts;
using Ticketing.BAL.Options;
using Polly;
using Polly.Retry;
using Ticketing.DAL.Domains;
using Ticketing.DAL.Repositories;
using Microsoft.Extensions.DependencyInjection;
using static Ticketing.DAL.Enums.Statuses;
using log4net;

namespace Ticketing.BAL.Services
{
    public class EmailService : IEmailService
    {
        private readonly IOptions<EmailSettings> _optionsEmailSettings;
        private readonly IOptions<RetryPolicySettings> _optionsRetryPolicySettings;
        private readonly AsyncRetryPolicy<Response> _retryPolicy;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILog _logger;

        public EmailService(IOptions<EmailSettings> optionsEmailSettings,
            IOptions<RetryPolicySettings> optionsRetryPolicySettings,
            IServiceScopeFactory serviceScopeFactory,
            ILog logger)
        {
            _optionsEmailSettings = optionsEmailSettings;
            _optionsRetryPolicySettings = optionsRetryPolicySettings;
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;

            _retryPolicy = RetryPolicy(_optionsRetryPolicySettings.Value.RetryCount, _optionsRetryPolicySettings.Value.SleepDurationSec);
        }

        public async Task SendEmailAsync(EmailNotification emailNotification)
        {
            _logger.Info($"EmailService Start Send Email.");
            EmailNotification savedEmailNotification = await SaveNotificationAsync(emailNotification);

            var fromEmail = _optionsEmailSettings.Value.SenderEmail;
            var fromName = _optionsEmailSettings.Value.SenderName;
            var apiKey = _optionsEmailSettings.Value.ApiKey;
            var sendGridClient = new SendGridClient(apiKey);
            var from = new EmailAddress(fromEmail, fromName);
            var to = new EmailAddress(emailNotification.Email, emailNotification.Name);
            var subject = "Email Notification";

            var body = $"This is Email Notification order {emailNotification.OrderId} amount {emailNotification.OrderAmount} summary {emailNotification.OrderSummary}";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, body, "");

            savedEmailNotification.EmailRequestStatus = EmailRequestStatus.InProgress;
            await UpdateNotificationAsync(savedEmailNotification);

            var response = await _retryPolicy.ExecuteAsync(async () => await sendGridClient.SendEmailAsync(msg));

            if (response.IsSuccessStatusCode)
            {
                _logger.Info($"EmailService Success.");
                savedEmailNotification.EmailRequestStatus = EmailRequestStatus.Success;
                await UpdateNotificationAsync(savedEmailNotification);
            }
            else
            {
                _logger.Info($"EmailService Fail.");
                savedEmailNotification.EmailRequestStatus = EmailRequestStatus.Fail;
                await UpdateNotificationAsync(savedEmailNotification);
            }
        }

        private async Task<EmailNotification> SaveNotificationAsync(EmailNotification emailNotification)
        {
            using var scope = _serviceScopeFactory.CreateScope();
                  var myScopedService = scope.ServiceProvider.GetService<Repository<EmailNotification>>();
                  return await myScopedService.CreateAsync(emailNotification);
        }

        private async Task UpdateNotificationAsync(EmailNotification emailNotification)
        {
            using var scope = _serviceScopeFactory.CreateScope();
                  var myScopedService = scope.ServiceProvider.GetService<Repository<EmailNotification>>();
            await myScopedService.UpdateAsync(emailNotification);
        }

        private AsyncRetryPolicy<Response> RetryPolicy(int retryCount, double waitMinutes)
        {
            return Policy
                       .Handle<Exception>()
                       .OrResult<SendGrid.Response>(r =>
                       {
                           var statusCode = (int)r.StatusCode;
                           return statusCode != 200;
                       })
                       .WaitAndRetryAsync(retryCount: retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(waitMinutes, retryAttempt)),
                        (ex, timeSpan, retryAttempt) =>
                       {
                           _logger.Error($"EmailService Error {ex}.");
                       });
        }
    }
}
