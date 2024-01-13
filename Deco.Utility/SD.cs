using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deco.Utility
{
    public static class SD
    {
        public const string Role_Customer = "Customer";
        public const string Role_Hotel = "Hotel";
        public const string Role_Admin = "Admin";
        public const string Role_Dealer = "Dealer";

        public const string Status_Pending = "Pending";
		public const string Status_Approved = "Approved";
		public const string Status_Proccessing = "Processing";
		public const string Status_Shiped = "Shiped";
		public const string Status_Canceled = "Canceled";
		public const string Status_Refunded = "Refunded";

        public const string Payment_Pending = "Pending";
        public const string Payment_Approved = "Approved";
		public const string Payment_DelayedPayment = "Delayed Peyment";
		public const string Payment_Rejected = "Rejected";
	}
}
