using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ticketing.DAL.Domain;
using static Ticketing.DAL.Enums.Statuses;

namespace Ticketing.BAL.Model
{
    public class SeatReturnModel
    {
        public int SeatId { get; set; }
        public int SectionId { get; set; }
        public int RowNumber { get; set; }
        public int SeatNumber { get; set; }
        public SeatState SeatStatusState { get; set; }
        public required string NameSeatStatus { get; set; }
        public int PriceTypeId { get; set; }
        public required string NamePriceType { get; set; }
    }
}
