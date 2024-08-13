namespace MultiFactorAuthentication.Interfaces
{
    public interface ISmsService
    {
        /// <summary>
        ///     Send a sms message.
        /// </summary>
        /// <param name="to">A phone number.</param>
        /// <param name="message">The message.</param>
        void Send(string to, string message);
    }
}