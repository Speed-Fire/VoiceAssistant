using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoiceAssistant.Core.Interfaces
{
    public interface IUrgentNotifier
    {
        /// <summary>
        /// Push message to the error queue.
        /// </summary>
        /// <param name="message">Message.</param>
        /// <param name="duration">Show duration in milliseconds.</param>
        /// <param name="exception">Exception.</param>
        void NotifyError(string message, int duration = 3000, Exception? exception = null);

        void NotifyWarning(string message, int duration = 3000);

        void NotifyInfo(string message, int duration = 3000);
    }
}
