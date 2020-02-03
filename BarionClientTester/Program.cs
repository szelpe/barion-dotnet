using System;
using System.Collections.Generic;

namespace BarionClientTester
{
    class Program
    {
        static void Main(string[] args)
        {
            var flows = new Dictionary<int, Action>
            {
                { 1, ImmediatePayment.Run },
                { 2, ReservationPayment.Run },
                { 3, Refund.Run },
                { 4, FinishReservation.Run },
                { 5, CapturePayment.Run }
            };

            Console.WriteLine("Select flow to run:");
            Console.WriteLine("\t(1) Immediate payment");
            Console.WriteLine("\t(2) Reservation payment");
            Console.WriteLine("\t(3) Start payment then refund");
            Console.WriteLine("\t(4) Start payment then finish reservation");
            Console.WriteLine("\t(5) Start payment then finish captured payment");

            var selectedFlowId = 0;
            while (selectedFlowId == 0)
            {
                var selectedFlowInput = Console.ReadLine();
                if(int.TryParse(selectedFlowInput, out var parsedFlowId))
                {
                    if (flows.ContainsKey(parsedFlowId))
                        selectedFlowId = parsedFlowId;
                }
            }

            flows[selectedFlowId]();

            Console.ResetColor();
            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}
