using System;

namespace TelegramBot.Declarations
{
    public interface IBot
    {
        public static void GetInstanse()
        {
            throw new NotImplementedException($"{nameof(IBot)} must implement singleton pattern.");
        }
        
        public static void CreateInstanse()
        {
            throw new NotImplementedException($"{nameof(IBot)} must implement singleton pattern.");
        }
        
        public void Start();
        public void Stop();
    }
}