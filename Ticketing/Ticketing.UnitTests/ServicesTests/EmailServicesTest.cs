using log4net;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using Ticketing.BAL.Options;
using Ticketing.BAL.Services;
using static Ticketing.DAL.Enums.Statuses;
using Ticketing.DAL.Domain;
using Ticketing.DAL.Domains;
using Ticketing.DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using Ticketing.UnitTests.Helpers;

namespace Ticketing.UnitTests.ServicesTests
{
    public class EmailServicesTest
    {
        [Fact]
        public async Task SendEmailAsync_Success()
        {
            var emailNotification = new EmailNotification
            {
                Email = "testc@gmail.com",
                Name = "customerName",
                Version = BitConverter.GetBytes(DateTime.Now.Millisecond),
                EmailRequestStatus = EmailRequestStatus.None,
                OrderAmount = 1,
                OrderSummary = 1,
                EmailTimeStamp = DateTime.Now,
                OrderId = 1
            };

            var moqLog = new Mock<ILog>();
            var moqLogObject = new Mock<ILog>().Object;

            var mockEmailNotificationSet = MockDbSet.BuildAsync(new List<EmailNotification>());

            var mockContext = new Mock<ApplicationContext>();
            mockContext.Setup<Microsoft.EntityFrameworkCore.DbSet<EmailNotification>>(c => c.EmailNotifications).Returns(mockEmailNotificationSet.Object);

            var _mockEmailNotificationRepository = new Mock<Repository<EmailNotification>>(mockContext.Object, moqLogObject);

            _mockEmailNotificationRepository.Setup(c => c.CreateAsync(emailNotification)).Returns(Task.FromResult(emailNotification));

            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider
                .Setup(x => x.GetService(typeof(Repository<EmailNotification>)))
                .Returns(_mockEmailNotificationRepository.Object);

            var serviceScopeMock = new Mock<IServiceScope>();
            serviceScopeMock.SetupGet<IServiceProvider>(s => s.ServiceProvider)
                .Returns(serviceProvider.Object);

            var serviceScopeFactoryMock = new Mock<IServiceScopeFactory>();
            serviceScopeFactoryMock.Setup(s => s.CreateScope())
                .Returns(serviceScopeMock.Object);

            var emailSettingsOptions = Options.Create(new EmailSettings { ApiKey = "Value1", SenderEmail = "Value2", SenderName = "Value3" });
            var retryPolicySettingsOptions = Options.Create(new RetryPolicySettings { RetryCount = 1, SleepDurationSec = 1});

            var emailService = new EmailService(emailSettingsOptions, retryPolicySettingsOptions, serviceScopeFactoryMock.Object, moqLogObject);

            await emailService.SendEmailAsync(emailNotification);

            _mockEmailNotificationRepository.Verify((p) => p.UpdateAsync(It.IsAny<EmailNotification>()), Times.AtLeast(2), ".Update EmailNotification is fail");
        }
    }
}
