using System;
using Newtonsoft.Json;


namespace airteknamespace{

    public class Order
    {   
        public int Id { get; set; }
        public string DepartureCity { get; set; } = "";
        public string destination { get; set; } = "";
        public int? FlightNumber { get; set; }
        public int? Day { get; set; }
    }

    public class Flight
    {
        public int FlightNumber { get; set; }
        public string DepartureCity { get; set; } = "";
        public string ArrivalCity { get; set; } = "";
        public int Day { get; set; }
        public int Capacity = 20;
        private List<Order> Orders = new List<Order>();

        public bool CanAccommodateOrder(Order order)
        {
            return Orders.Count < Capacity && order.destination == ArrivalCity;
        }

        public void AddOrder(Order order)
        {
            order.FlightNumber = FlightNumber;
            order.DepartureCity = DepartureCity;
            order.destination = ArrivalCity;
            order.Day = Day;
            Orders.Add(order);
        }
    }

    public class FlightScheduler
    {
        public List<Flight> LoadDefaultFlightsSchedule(bool loadDefault = true){

            List<Flight> flights = new List<Flight>();

            try{
                if(loadDefault){
                    //Loading default flights
                    //Day 1
                    flights.Add(new Flight { FlightNumber = 1, DepartureCity = "YUL", ArrivalCity = "YYZ", Day = 1});
                    flights.Add(new Flight { FlightNumber = 2, DepartureCity = "YUL", ArrivalCity = "YYC", Day = 1});
                    flights.Add(new Flight { FlightNumber = 3, DepartureCity = "YUL", ArrivalCity = "YVR", Day = 1});

                    //Day 2
                    flights.Add(new Flight { FlightNumber = 4, DepartureCity = "YUL", ArrivalCity = "YYZ", Day = 2});
                    flights.Add(new Flight { FlightNumber = 5, DepartureCity = "YUL", ArrivalCity = "YYC", Day = 2});
                    flights.Add(new Flight { FlightNumber = 6, DepartureCity = "YUL", ArrivalCity = "YVR", Day = 2});

                    Console.WriteLine("Default flights schedule loaded.\n");
                }
                else{
                    Console.WriteLine(" --> Add new flight schedule.\n");
                    Console.WriteLine("Enter number of flights you want to add:");
                    var numOfFlights = Convert.ToInt32(Console.ReadLine());

                    for(int i = 0; i<numOfFlights; i++){
                        Flight newFlight = new Flight
                        {
                            FlightNumber = i + 1,
                        };

                        Console.WriteLine($"For the {i+1} flight:");
                        Console.WriteLine("Enter the flight departure city airport code:");
                        newFlight.DepartureCity = Console.ReadLine();
                        Console.WriteLine("Enter the flight arrival city airport code:");
                        newFlight.ArrivalCity = Console.ReadLine();
                        Console.WriteLine("Enter the day this fight will fly:");
                        newFlight.Day = Convert.ToInt32(Console.ReadLine());
                        Console.WriteLine("\n");

                        flights.Add(newFlight);
                    } 
                    Console.WriteLine("New flights schedule loaded.\n");
                }
            }
            catch(Exception ex){
                Console.WriteLine("Application failed at the inner function LoadDefaultFlightsSchedule() due to following reason. Please restart the application and try again." + ex.Message + " " + ex.StackTrace);
            }
            
            return flights;  

        }
    }

    public class OrderScheduler{

        public List<Order> LoadOrders(string filePath)
        {
            var json = File.ReadAllText(filePath);
            var ordersDict = JsonConvert.DeserializeObject<Dictionary<string, Order>>(json);
            var orders = new List<Order>();

            foreach (var orderEle in ordersDict)
            {
                orderEle.Value.Id = Convert.ToInt32(orderEle.Key.Split("-")[1]);
                orders.Add(orderEle.Value);
            }

            return orders;
        }

        public void ScheduleOrders(List<Order> orders, List<Flight> flights)
        {
            foreach (var order in orders)
            {
                foreach (var flight in flights)
                {
                    if (flight.CanAccommodateOrder(order))
                    {
                        flight.AddOrder(order);
                        break;
                    }
                }
            }
        }
    }

    class AirtekConsole
    {
        static void Main(string[] args)
        {   
            FlightScheduler _flightScheduler = new FlightScheduler();
            OrderScheduler _orderScheduler = new OrderScheduler();

            Console.WriteLine("WELCOME TO SPEEDYAIR.LY CONSOLE!");
            Console.WriteLine("--------------------------------");

            Console.WriteLine(" --> Load default flight? Enter 1(Yes) or 0(No):");
            bool loadDefault = (Console.ReadLine() == "1")? true: false;
            List<Flight> flightsLoaded = _flightScheduler.LoadDefaultFlightsSchedule(loadDefault);

            Console.WriteLine("Current flight schedule is:");
            Console.WriteLine("---------------------------");
            foreach (var flight in flightsLoaded)
            {
                Console.WriteLine($"Flight: {flight.FlightNumber}, departure: {flight.DepartureCity.ToString().ToUpper()}, arrival: {flight.ArrivalCity.ToString().ToUpper()}, day: {flight.Day}");
            }
            Console.WriteLine("\n");

            // Load orders from the JSON file
            string jsonFilePath = "files/coding-assigment-orders.json";
            var ordersLoaded = _orderScheduler.LoadOrders(jsonFilePath);


            // Assign orders to flights
            _orderScheduler.ScheduleOrders(ordersLoaded, flightsLoaded);

            Console.WriteLine("Orders scheduled based on the flight schedule:");
            Console.WriteLine("----------------------------------------------\n");
            
            // Output the result
            foreach (var order in ordersLoaded)
            {
                if (order.FlightNumber == null)
                {
                    Console.WriteLine($"order: {order.Id}, flightNumber: not scheduled");
                }
                else
                {
                    Console.WriteLine($"order: {order.Id}, flightNumber: {order.FlightNumber}, departure: {order.DepartureCity}, arrival: {order.destination}, day: {order.Day}");
                }
            }

        }
    }

}
