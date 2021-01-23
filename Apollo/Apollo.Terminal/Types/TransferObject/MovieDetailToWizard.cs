using System.Collections.Generic;
using Apollo.Core.Dto;

namespace Apollo.Terminal.Types.TransferObject
{
    public class MovieDetailToWizard
    {
        public ScheduleDto SelectedSchedule { get; }

        public IList<ScheduleDto> Schedules { get; }

        public MovieDetailToWizard(ScheduleDto selectedSchedule, IList<ScheduleDto> schedules)
        {
            SelectedSchedule = selectedSchedule;
            Schedules = schedules;
        }
    }
}