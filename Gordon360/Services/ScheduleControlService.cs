
using Gordon360.Database.CCT;
using Gordon360.Exceptions;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Gordon360.Services
{
    /// <summary>
    /// Service Class that facilitates data transactions between the ScheduleControlController and the ScheduleControl part of the database model.
    /// </summary>
    public class ScheduleControlService : IScheduleControlService
    {
        private CCTContext _context;

        public ScheduleControlService(CCTContext context)
        {
            _context = context;
        }

        /// <summary>
        /// privacy setting of schedule.
        /// </summary>
        /// <param name="id">id</param>
        /// <param name="value">Y or N</param>
        public async Task UpdateSchedulePrivacy(string id, string value)
        {
            var original = _context.ACCOUNT.FirstOrDefault(x => x.gordon_id == id);

            if (original == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The account was not found." };
            }

            await _context.Procedures.UPDATE_SCHEDULE_PRIVACYAsync(int.Parse(id), value);

        }

        /// <summary>
        /// description of schedule.
        /// </summary>
        /// <param name="id">id</param>
        /// <param name="value">New description</param>
        public async Task UpdateDescription(string id, string value)
        {
            var original = _context.ACCOUNT.FirstOrDefault(x => x.gordon_id == id);

            if (original == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The account was not found." };
            }

            await _context.Procedures.UPDATE_DESCRIPTIONAsync(int.Parse(id), value);
        }


        /// <summary>
        /// Update timestamp of modification in schedule.
        /// </summary>
        /// <param name="id">id</param>
        /// <param name="value">Modified Time</param>
        public async Task UpdateModifiedTimeStamp(string id, DateTime value)
        {
            var original = _context.ACCOUNT.FirstOrDefault(x => x.gordon_id == id);

            if (original == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The account was not found." };
            }

            await _context.Procedures.UPDATE_TIMESTAMPAsync(int.Parse(id), value);
        }
    }
}