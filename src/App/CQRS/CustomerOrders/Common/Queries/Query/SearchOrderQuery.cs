using System;
using System.Collections.Generic;
using System.Text;
using Common.Extensions;
using Data.Common;
using Data.Enums;

namespace App.CQRS.Orders.Common.Queries.Query
{
    //public sealed class SearchOrderQuery : AbstractPagedQuery<SearchOrderQuery.Order>
    //{
    //    public SearchOrderQuery(string correlationId, string tenantId, string userId,
    //        EnumOrderStatus status, DateTime dateStart, DateTime dateEnd,
    //        string criteria, int pageIndex, int pageSize, string sortField, int sortOrder)
    //        : base(correlationId, tenantId, userId, criteria, pageIndex, pageSize, sortField, sortOrder)
    //    {
    //        Status = status;
    //        DateStart = dateStart;
    //        DateEnd = dateEnd;
    //    }

    //    //public EnumOrderStatus Status { get; }
    //    public DateTime DateStart { get; }
    //    public DateTime DateEnd { get; }

    //    public class Order
    //    {
    //        public string CustomerOrderId { get; set; }
    //        public string Number { get; set; }

    //        DateTime _orderDateTime;
    //        public DateTime OrderDateTime
    //        {
    //            get => _orderDateTime;
    //            set => _orderDateTime = value.AsUtc();
    //        }

    //        DateTime _expectedMinDeliveryDateTime;
    //        public DateTime ExpectedMinDeliveryDateTime
    //        {
    //            get => _expectedMinDeliveryDateTime;
    //            set => _expectedMinDeliveryDateTime = value.AsUtc();
    //        }

    //        DateTime _expectedMaxDeliveryDateTime;
    //        public DateTime ExpectedMaxDeliveryDateTime
    //        {
    //            get => _expectedMaxDeliveryDateTime;
    //            set => _expectedMaxDeliveryDateTime = value.AsUtc();
    //        }

    //        public EnumOrderStatus OrderStatus { get; set; }
    //        public string OrderStatusText => OrderStatus.ToString();

    //        public EnumPaymentMethod PaymentMethod { get; set; }
    //        public string PaymentMethodText => PaymentMethod.ToString();

    //        public EnumShipmentType ShipmentType { get; set; }
    //        public string ShipmentTypeText => ShipmentType.ToString();

    //        public double NetPrice { get; set; }
    //        public double AmountPaid { get; set; }

    //        public string RecipientName { get; set; }
    //        public string Phone { get; set; }
    //        public string Address { get; set; }
    //    }
    //}
}
