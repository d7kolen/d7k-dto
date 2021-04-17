using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace d7k.Dto.Tests
{
	public class SaveLotDtoTemp
	{
		public int? LotId { get; set; }
		public DateTimeOffset? FixDate { get; set; }
		public DateTimeOffset? PlannedPaymentDate { get; set; }
		public DateTimeOffset? FactPaymentDate { get; set; }
		public DateTimeOffset? PaymentReceiveDate { get; set; }
		public DateTimeOffset? PlannedDeliveryDate { get; set; }
		public DateTimeOffset? FactDeliveryDate { get; set; }
		public DateTimeOffset? PickUpDate { get; set; }

		public bool? WithNds { get; set; }
		public decimal? BaseExchangeHousePriceRubGram { get; set; }
		public decimal? ExchangeHousePriceRubGram { get; set; }
		public decimal? WeightGram { get; set; }
		public decimal? PriceRubGram { get; set; }
		public decimal? Percent { get; set; }
		public decimal? Sum { get; set; }
		public decimal NotNullableSum { get; set; }
		public string Comment { get; set; }
		public byte[] Timestamp { get; set; }
		public List<byte> List { get; set; }

		public int? ProductTypeId { get; set; }
		public int? ExchangeHouseId { get; set; }
		public int? SupplierId { get; set; }
		public int? DeliveryMethodId { get; set; }
		public int? PriceCalcId { get; set; }
		public int UserId { get; set; }
	}
}
