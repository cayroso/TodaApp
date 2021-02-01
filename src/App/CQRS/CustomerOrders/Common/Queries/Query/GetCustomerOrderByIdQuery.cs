using System;
using System.Collections.Generic;
using System.Text;
using Common.Extensions;
using Data.Enums;

namespace App.CQRS.Orders.Common.Queries.Query
{
    public sealed class GetCustomerOrderByIdQuery : AbstractQuery<GetCustomerOrderByIdQuery.CustomerOrder>
    {
        public GetCustomerOrderByIdQuery(string correlationId, string tenantId, string userId, string roleId, string orderId)
            : base(correlationId, tenantId, userId)
        {
            RoleId = roleId;
            OrderId = orderId;
        }
        public string RoleId { get; }
        public string OrderId { get; }

        public class CustomerOrder
        {
            public string CustomerOrderId { get; set; }

            public string Customer { get; set; }
            public string PhoneNumber { get; set; }

            public string Address { get; set; }


            public string Number { get; set; }

            //public EnumOrderStatus OrderStatus { get; set; }
            //public string OrderStatusText => OrderStatus.ToString();

            //public EnumPaymentMethod PaymentMethod { get; set; }
            //public string PaymentMethodText => PaymentMethod.ToString();

            public ShippingSetting ShippingSetting { get; set; }


            DateTime _orderDateTime;
            public DateTime OrderDateTime
            {
                get => _orderDateTime;
                set => _orderDateTime = value.Truncate().AsUtc();
            }

            DateTime _deliveryDateTime;
            public DateTime DeliveryDateTime
            {
                get => _deliveryDateTime;
                set => _deliveryDateTime = value.Truncate().AsUtc();
            }

            DateTime _expectedMinDeliveryDateTime;
            public DateTime ExpectedMinDeliveryDateTime
            {
                get => _expectedMinDeliveryDateTime;
                set => _expectedMinDeliveryDateTime = value.Truncate().AsUtc();
            }

            DateTime _expectedMaxDeliveryDateTime;
            public DateTime ExpectedMaxDeliveryDateTime
            {
                get => _expectedMaxDeliveryDateTime;
                set => _expectedMaxDeliveryDateTime = value.Truncate().AsUtc();
            }

            public double GrossPrice { get; set; }
            public double DeliveryFee { get; set; }
            public double NetPrice { get; set; }
            public double AmountPaid { get; set; }

            public bool FullyPaid => AmountPaid >= NetPrice;

            public List<Orderline> Lines { get; set; } = new List<Orderline>();
            public List<OrderNote> OrderNotes { get; set; } = new List<OrderNote>();
            public List<OrderPayment> OrderPayments { get; set; } = new List<OrderPayment>();
            public List<History> Histories { get; set; } = new List<History>();
            public List<UserTask> UserTasks { get; set; } = new List<UserTask>();
            public string Token { get; set; }

        }
        public class ShippingSetting
        {
            public string Name { get; set; }
            public uint MinDelayInHours { get; set; }
            public uint MaxDelayInHours { get; set; }

            public double MinimumOrderValue { get; set; }
            public double FlatRate { get; set; }
            public double PricePercentage { get; set; }

            //public EnumShipmentType ShipmentType { get; set; }
            //public string EnumShipmentTypeText => ShipmentType.ToString();
        }

        public class Orderline
        {
            public string ProductName { get; set; }
            public double ProductPrice { get; set; }

            public string LineNumber { get; set; }
            public double Quantity { get; set; }
            public double ExtendedPrice { get; set; }

            public List<Bottle> Bottles { get; set; } = new List<Bottle>();
        }

        public class Bottle
        {
            public string BottleId { get; set; }
            public string Name { get; set; }
            public string TrackingNumber { get; set; }
            public bool IsReturned { get; set; }
        }

        public class History
        {
            public string OrderStatus { get; set; }

            DateTime _historyDateTime;
            public DateTime HistoryDateTime
            {
                get => _historyDateTime;
                set => _historyDateTime = value.Truncate().AsUtc();
            }

            public string User { get; set; }
            public string Note { get; set; }
        }

        public class UserTask
        {
            public string UserTaskId { get; set; }
            public EnumTaskStatus Status { get; set; }
            public string StatusText => Status.ToString();

            public string RoleId { get; set; }

            public string UserId { get; set; }
            public string UserFirstLastName { get; set; }

            public string Title { get; set; }
            //public string Description { get; set; }

            DateTime _dateAssigned = DateTime.MaxValue;
            public DateTime DateAssigned
            {
                get => _dateAssigned;
                set => _dateAssigned = value.AsUtc();
            }

            DateTime _dateCompleted = DateTime.MaxValue;
            public DateTime DateCompleted
            {
                get => _dateCompleted;
                set => _dateCompleted = value.AsUtc();
            }
            public string Token { get; set; }
        }

        public class OrderNote
        {
            public string User { get; set; }

            public string Note { get; set; }
            public bool SystemGenerated { get; set; }

            DateTime _dateCreated;
            public DateTime DateCreated
            {
                get => _dateCreated;
                set => _dateCreated = value.AsUtc();
            }
        }

        public class OrderPayment
        {
            public string User { get; set; }
            public double AmountDue { get; set; }
            public double AmountPaid { get; set; }

            public string Note { get; set; }

            DateTime _dateCreated;
            public DateTime DateCreated
            {
                get => _dateCreated;
                set => _dateCreated = value.AsUtc();
            }
        }
    }
}
