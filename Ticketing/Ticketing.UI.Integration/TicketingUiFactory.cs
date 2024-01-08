using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Ticketing.DAL.Domain;
using Ticketing.DAL.Domains;
using Ticketing.DAL.Repositories;
using static Ticketing.DAL.Enums.Statuses;

namespace Ticketing.UI.Integration
{
    public class TicketingUiFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var dbContextDescriptor = services.SingleOrDefault(
                    d => d.ServiceType ==
                        typeof(DbContextOptions<ApplicationContext>));

                services.Remove(dbContextDescriptor);

                var dbConnectionDescriptor = services.SingleOrDefault(
                    d => d.ServiceType ==
                        typeof(DbConnection));

                services.Remove(dbConnectionDescriptor);

                services.AddDbContext<ApplicationContext>(opts => opts.UseSqlServer("Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=TicketDB_TestV2;Integrated Security=True;"));

                var sp = services.BuildServiceProvider();
                using (var scope = sp.CreateScope())
                using (var appContext = scope.ServiceProvider.GetRequiredService<ApplicationContext>())
                {
                    try
                    {
                        appContext.Database.EnsureDeleted();
                        appContext.Database.EnsureCreated();
                        appContext.EnsureSeedData();
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
            });
        }
    }

    public static class Seed
    {
        public static void EnsureSeedData(this ApplicationContext context)
        {
            if (!context.Database.GetPendingMigrations().Any())
            {
                if (!context.Venues.Any())
                {
                    context.Venues.Add(new Venue { Name = "Venue1", Version = BitConverter.GetBytes(DateTime.Now.Millisecond) });
                    context.Venues.Add(new Venue { Name = "Venue2", Version = BitConverter.GetBytes(DateTime.Now.Millisecond) });
                    context.Venues.Add(new Venue { Name = "Venue3", Version = BitConverter.GetBytes(DateTime.Now.Millisecond) });
                    context.SaveChanges();
                }

                if (!context.Events.Any())
                {
                    context.Events.Add(new Event { Name = "Event1", EventDate = DateTime.Now.AddDays(-3), Version = BitConverter.GetBytes(DateTime.Now.Millisecond) });
                    context.Events.Add(new Event { Name = "Event2", EventDate = DateTime.Now, Version = BitConverter.GetBytes(DateTime.Now.Millisecond) });
                    context.Events.Add(new Event { Name = "Event3", EventDate = DateTime.Now.AddDays(+3), Version = BitConverter.GetBytes(DateTime.Now.Millisecond) });
                    context.SaveChanges();
                }

                if (!context.Carts.Any())
                {
                    context.Carts.Add(new Cart { Id = new Guid("F9168C5E-CEB2-4faa-B6BF-329BF39FA1E4"), Version = BitConverter.GetBytes(DateTime.Now.Millisecond) });
                    context.SaveChanges();
                }

                if (!context.Orders.Any())
                {
                    context.Orders.Add(new Order { Name = "Order1", CartId = new Guid("F9168C5E-CEB2-4faa-B6BF-329BF39FA1E4"), Version = BitConverter.GetBytes(DateTime.Now.Millisecond) });
                    context.SaveChanges();
                }

                if (!context.PriceTypes.Any())
                {
                    context.PriceTypes.Add(new PriceType { Name = "Adult", Version = BitConverter.GetBytes(DateTime.Now.Millisecond) });
                    context.PriceTypes.Add(new PriceType { Name = "Child", Version = BitConverter.GetBytes(DateTime.Now.Millisecond) });
                    context.PriceTypes.Add(new PriceType { Name = "VIP", Version = BitConverter.GetBytes(DateTime.Now.Millisecond) });
                    context.PriceTypes.Add(new PriceType { Name = "Admission", Version = BitConverter.GetBytes(DateTime.Now.Millisecond) });
                    context.SaveChanges();
                }

                if (!context.SeatStatuses.Any())
                {
                    context.SeatStatuses.Add(new SeatStatus { Id = SeatState.Available, Name = "Available", Version = BitConverter.GetBytes(DateTime.Now.Millisecond) });
                    context.SeatStatuses.Add(new SeatStatus { Id = SeatState.Booked, Name = "Booked", Version = BitConverter.GetBytes(DateTime.Now.Millisecond) });
                    context.SeatStatuses.Add(new SeatStatus { Id = SeatState.Sold, Name = "Sold", Version = BitConverter.GetBytes(DateTime.Now.Millisecond) });
                    context.SaveChanges();
                }

                if (!context.PaymentStatuses.Any())
                {
                    context.PaymentStatuses.Add(new PaymentStatus { Id = PaymentState.NoPayment, Name = "No payment", Version = BitConverter.GetBytes(DateTime.Now.Millisecond) });
                    context.PaymentStatuses.Add(new PaymentStatus { Id = PaymentState.PartPayment, Name = "Part payment", Version = BitConverter.GetBytes(DateTime.Now.Millisecond) });
                    context.PaymentStatuses.Add(new PaymentStatus { Id = PaymentState.FullPayment, Name = "Full payment", Version = BitConverter.GetBytes(DateTime.Now.Millisecond) });
                    context.PaymentStatuses.Add(new PaymentStatus { Id = PaymentState.PaymentFailed, Name = "Payment Failed", Version = BitConverter.GetBytes(DateTime.Now.Millisecond) });
                    context.SaveChanges();
                }

                if (!context.Payments.Any())
                {
                    context.Payments.Add(new Payment { Amount = 0, PaymentStatusId = PaymentState.NoPayment, CartId = new Guid("F9168C5E-CEB2-4faa-B6BF-329BF39FA1E4"), Version = BitConverter.GetBytes(DateTime.Now.Millisecond) });
                    context.SaveChanges();
                }

                if (!context.Sections.Any())
                {
                    context.Sections.Add(new Section { Name = "Section1", VenueId = 1, PriceTypeId = 1, Version = BitConverter.GetBytes(DateTime.Now.Millisecond) });
                    context.Sections.Add(new Section { Name = "Section2", VenueId = 1, PriceTypeId = 2, Version = BitConverter.GetBytes(DateTime.Now.Millisecond) });
                    context.Sections.Add(new Section { Name = "Section3", VenueId = 1, PriceTypeId = 3, Version = BitConverter.GetBytes(DateTime.Now.Millisecond) });

                    context.SaveChanges();
                }

                if (!context.Seats.Any())
                {
                    context.Seats.Add(new Seat { SectionId = 1, RowNumber = 1, SeatNumber = 1, SeatStatusState = SeatState.Available, Version = BitConverter.GetBytes(DateTime.Now.Millisecond) });
                    context.Seats.Add(new Seat { SectionId = 1, RowNumber = 1, SeatNumber = 2, SeatStatusState = SeatState.Available, Version = BitConverter.GetBytes(DateTime.Now.Millisecond) });
                    context.Seats.Add(new Seat { SectionId = 1, RowNumber = 1, SeatNumber = 3, SeatStatusState = SeatState.Available, Version = BitConverter.GetBytes(DateTime.Now.Millisecond) });

                    context.SaveChanges();
                }

                if (!context.ShoppingCarts.Any())
                {
                    context.ShoppingCarts.Add(new ShoppingCart
                    {
                        EventId = 1,
                        SeatId = 1,
                        PriceTypeId = 1,
                        Price = 1,
                        CartId = new Guid("F9168C5E-CEB2-4faa-B6BF-329BF39FA1E4"),
                        Version = BitConverter.GetBytes(DateTime.Now.Millisecond)
                    });
                    context.SaveChanges();
                }
            }
        }
    }
}
