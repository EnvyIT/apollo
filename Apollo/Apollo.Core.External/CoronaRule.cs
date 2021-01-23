using System;
using System.Collections.Generic;
using System.Linq;
using Apollo.Core.Dto;
using Apollo.Core.Validation;
using Apollo.Util;
using Apollo.Util.Logger;

namespace Apollo.Core.External
{
    public class CoronaRule : ISeatValidation
    {
        private static readonly IApolloLogger<CoronaRule> Logger =
            LoggerFactory.CreateLogger<CoronaRule>();

        public bool IsValid(IEnumerable<SeatDto> seatData, IEnumerable<SeatDto> desiredSeats)
        {
            var seatDataList = seatData.ToList();
            var desiredSeatsList = desiredSeats.ToList();

            ValidationHelper.ValidateEnumerableIsNotNullOrEmpty(Logger.Here(), seatDataList, nameof(seatData));
            ValidationHelper.ValidateEnumerableIsNotNullOrEmpty(Logger.Here(), desiredSeatsList, nameof(desiredSeatsList));

            var seatLayout = GetSeatLayout(seatDataList, desiredSeatsList);
            return desiredSeatsList.All(seat => ValidateNeighbors(seatLayout, seat.LayoutRow, seat.LayoutColumn));
        }

        private static bool ValidateNeighbors(SeatState[,] data, int row, int column)
        {
            var radius = 1;
            var rowCount = data.GetLength(0);
            var columnCount = data.GetLength(1);

            for (var rowOffset = -radius; rowOffset <= radius; rowOffset++)
            {
                for (var columnOffset = -radius; columnOffset <= radius; columnOffset++)
                {
                    var currentRow = row + rowOffset;
                    var currentColumn = column + columnOffset;

                    if ((currentRow != row || currentColumn != column) &&
                        currentRow >= 0 && currentRow < rowCount &&
                        currentColumn >= 0 && currentColumn < columnCount &&
                        data[currentRow, currentColumn] == SeatState.Occupied)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private static SeatState[,] GetSeatLayout(List<SeatDto> seatData, List<SeatDto> desiredSeats)
        {
            var maxColumn = Math.Max(seatData.Max(s => s.LayoutColumn), desiredSeats.Max(s => s.LayoutColumn)) + 1;
            var maxRow = Math.Max(seatData.Max(s => s.LayoutColumn), desiredSeats.Max(s => s.LayoutColumn)) + 1;

            var layout = new SeatState[maxRow, maxColumn];
            foreach (var seat in seatData)
            {
                layout[seat.LayoutRow, seat.LayoutColumn] = seat.State;
            }

            return layout;
        }
    }
}