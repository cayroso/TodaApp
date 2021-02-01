using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.CQRS.Orders.Common.Queries.Query;
using Data.App.DbContext;
using Data.Common;
using Data.Constants;
using Data.Enums;
using Data.Identity.DbContext;

namespace App.CQRS.Orders.Common.Queries.Handler
{
    public sealed class CustomerOrderQueryHandler//:
        //IQueryHandler<GetCustomerOrderByIdQuery, GetCustomerOrderByIdQuery.CustomerOrder>,
        //IQueryHandler<SearchOrderQuery, Paged<SearchOrderQuery.Order>>
    {
        readonly AppDbContext _dbContext;
        public CustomerOrderQueryHandler(AppDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        //async Task<GetCustomerOrderByIdQuery.CustomerOrder> IQueryHandler<GetCustomerOrderByIdQuery, GetCustomerOrderByIdQuery.CustomerOrder>.HandleAsync(GetCustomerOrderByIdQuery query)
        //{
        //    //var includeAllTasks = query.RoleId == ApplicationRoles.Manager.Id || query.RoleId == ApplicationRoles.Assistant.Id;

        //    var sql = from o in _dbContext.Orders
        //                        .IgnoreQueryFilters()
        //                        .AsNoTracking()

        //              where o.OrderId == query.OrderId

        //              select new GetCustomerOrderByIdQuery.CustomerOrder
        //              {
        //                  CustomerOrderId = o.OrderId,
        //                  Number = o.Number,

        //                  Customer = o.DeliveryAddress.RecipientName,
        //                  PhoneNumber = o.DeliveryAddress.PhoneNumber,
        //                  Address = o.DeliveryAddress.Address,


        //                  OrderDateTime = o.OrderDateTime,
        //                  DeliveryDateTime = o.DeliveryDateTime,
        //                  ExpectedMinDeliveryDateTime = o.ExpectedMinDeliveryDateTime,
        //                  ExpectedMaxDeliveryDateTime = o.ExpectedMaxDeliveryDateTime,
        //                  OrderStatus = o.OrderStatus,
        //                  PaymentMethod = o.PaymentMethod,
        //                  ShippingSetting = new GetCustomerOrderByIdQuery.ShippingSetting
        //                  {
        //                      Name = o.ShippingSetting.Name,
        //                      MinDelayInHours = o.ShippingSetting.MinDelayInHours,
        //                      MaxDelayInHours = o.ShippingSetting.MaxDelayInHours,
        //                      FlatRate = o.ShippingSetting.FlatRate,
        //                      PricePercentage = o.ShippingSetting.PricePercentage,
        //                      MinimumOrderValue = o.ShippingSetting.MinimumOrderValue,
        //                      ShipmentType = o.ShippingSetting.ShipmentType
        //                  },

        //                  GrossPrice = o.GrossPrice,
        //                  DeliveryFee = o.DeliveryFee,
        //                  NetPrice = o.NetPrice,
        //                  AmountPaid = o.AmountPaid,
        //                  //DeliveryOption = new GetOrderByIdQuery.DeliveryOption
        //                  //{
        //                  //    Name = o.DeliveryOption.Name,
        //                  //    Amount = o.DeliveryOption.Amount,
        //                  //    MinDelayInHours = o.DeliveryOption.MinDelayInHours,
        //                  //    MaxDelayInHours = o.DeliveryOption.MaxDelayInHours,
        //                  //    Notes = o.DeliveryOption.Notes
        //                  //},
        //                  Lines = o.LineItems.OrderBy(e => e.LineNumber).Select(e => new GetCustomerOrderByIdQuery.Orderline
        //                  {
        //                      ExtendedPrice = e.ExtendedPrice,
        //                      LineNumber = e.LineNumber,
        //                      ProductName = e.Product.Name,
        //                      ProductPrice = e.Product.Prices.First().Price,
        //                      Quantity = e.QuantityOrdered,
        //                      Bottles = e.Bottles.Select(b => new GetCustomerOrderByIdQuery.Bottle
        //                      {
        //                          BottleId = b.BottleId,
        //                          Name = b.Bottle.Name,
        //                          TrackingNumber = b.Bottle.TrackingNumber,
        //                          IsReturned = b.IsReturned
        //                      }).ToList()
        //                  }).ToList(),
        //                  OrderNotes = o.OrderNotes.OrderBy(e => e.DateCreated).Select(e => new GetCustomerOrderByIdQuery.OrderNote
        //                  {
        //                      Note = e.Note,
        //                      DateCreated = e.DateCreated,
        //                      SystemGenerated = e.SystemGenerated,
        //                      User = e.User.FirstLastName
        //                  }).ToList(),
        //                  OrderPayments = o.OrderPayments.OrderBy(e => e.DateCreated).Select(e => new GetCustomerOrderByIdQuery.OrderPayment
        //                  {
        //                      DateCreated = e.DateCreated,
        //                      AmountDue = e.AmountDue,
        //                      AmountPaid = e.AmountPaid,
        //                      Note = e.Note,
        //                      User = e.User.FirstLastName
        //                  }).ToList(),
        //                  Histories = o.StatusHistories.OrderBy(e => e.HistoryDateTime).Select(e => new GetCustomerOrderByIdQuery.History
        //                  {
        //                      HistoryDateTime = e.HistoryDateTime,
        //                      OrderStatus = e.OrderStatus.ToString(),
        //                      User = e.User.FirstLastName,
        //                      Note = e.Note
        //                  }).ToList(),
        //                  //UserTasks = o.UserTasks.Where(e => e.RoleId == query.RoleId || includeAllTasks)
        //                  //                      .OrderBy(e => e.Title).Select(e => new GetOrderByIdQuery.UserTask
        //                  //                      {
        //                  //                          UserTaskId = e.UserTaskId,
        //                  //                          RoleId = e.RoleId,
        //                  //                          UserId = e.UserId,
        //                  //                          UserFirstLastName = string.IsNullOrWhiteSpace(e.UserId) ? string.Empty : e.User.FirstLastName,
        //                  //                          Title = e.Title,
        //                  //                          Status = e.Status,
        //                  //                          DateAssigned = e.DateAssigned,
        //                  //                          DateCompleted = e.DateCompleted,
        //                  //                          Token = e.ConcurrencyToken,
        //                  //                      }).ToList(),
        //                  Token = o.ConcurrencyToken
        //              };

        //    var dto = await sql.FirstOrDefaultAsync();

        //    return dto;
        //}


        //async Task<Paged<SearchOrderQuery.Order>> IQueryHandler<SearchOrderQuery, Paged<SearchOrderQuery.Order>>.HandleAsync(SearchOrderQuery query)
        //{
        //    var sql = from o in _dbContext.Orders.AsNoTracking()

        //              where query.Status == EnumOrderStatus.Unknown || o.OrderStatus == query.Status
        //              where o.OrderDateTime >= query.DateStart && o.OrderDateTime <= query.DateEnd

        //              where string.IsNullOrWhiteSpace(query.Criteria)
        //                    || EF.Functions.Like(o.Number, $"%{query.Criteria}%")
        //              //|| EF.Functions.Like(o.FirstName, $"%{query.Criteria}%")
        //              //|| EF.Functions.Like(o.LastName, $"%{query.Criteria}%")
        //              //|| EF.Functions.Like(o.PhoneNumber, $"%{query.Criteria}%")
        //              //|| EF.Functions.Like(o.Address, $"%{query.Criteria}%")


        //              orderby o.OrderDateTime

        //              select new SearchOrderQuery.Order
        //              {
        //                  CustomerOrderId = o.OrderId,
        //                  Number = o.Number,
        //                  ShipmentType = o.ShippingSetting.ShipmentType,
        //                  NetPrice = o.NetPrice,
        //                  AmountPaid = o.AmountPaid,
        //                  OrderStatus = o.OrderStatus,
        //                  PaymentMethod = o.PaymentMethod,
        //                  OrderDateTime = o.OrderDateTime,
        //                  ExpectedMinDeliveryDateTime = o.ExpectedMinDeliveryDateTime,
        //                  ExpectedMaxDeliveryDateTime = o.ExpectedMaxDeliveryDateTime,

        //                  RecipientName = o.DeliveryAddress.RecipientName,
        //                  Phone = o.DeliveryAddress.PhoneNumber,
        //                  Address = o.DeliveryAddress.Address
        //              };

        //    var pagedItems = await sql.ToPagedItemsAsync(query.PageIndex, query.PageSize);

        //    return pagedItems;
        //}

    }
}
