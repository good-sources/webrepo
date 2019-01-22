namespace AggregationServiceClient
{
    using System;
    using AggregationService.Domain.Models;

    class Program
    {
        static readonly string endpointAddress = ApplicationSettingsManager.Pick("endpointAddress");
        static readonly ServiceConsumer serviceConsumer = new ServiceConsumer(endpointAddress);
        static readonly OutputFormatter formatter = new OutputFormatter();

        static void Main(string[] args)
        {
            Console.WriteLine("************* Welcome to Aggregation Service consuming app *************\n\n");
            Console.WriteLine("— Press (spacebar) to see available options or any other key for exit —\n");

            if (Console.ReadKey(true).Key == ConsoleKey.Spacebar)
            {
                Console.WriteLine("– Type (a) to list collections");
                Console.WriteLine("– Type (b) to add a new collection");
                Console.WriteLine("– Type (c) to add a new source");
                Console.WriteLine("– Type (d) to list contents");
                Console.WriteLine("– Type (q) for exit\n");

                try
                {
                    while (true)
                    {
                        switch (Console.ReadKey(true).KeyChar)
                        {
                            case 'a':
                                Console.WriteLine("Collections list");
                                Console.WriteLine(formatter.FormatCollections(serviceConsumer.GetCollections()));

                                break;
                            case 'b':
                                Console.WriteLine("New collection");
                                Console.Write("Specify collection name: ");

                                if (serviceConsumer.CreateCollection(Console.ReadLine()))
                                    Console.WriteLine("New collection has been added");

                                Console.WriteLine();
                                goto case 'a';
                            case 'c':
                                Console.WriteLine("New source");
                                Console.WriteLine($"Specify – separated by comma – type of source ({formatter.FormatSourceTypesDictionary(serviceConsumer.GetSupportedSourceTypes())}), URI and collection name: ");

                                if (serviceConsumer.CreateSource(Console.ReadLine()))
                                    Console.Write("New source has been added");

                                break;
                            case 'd':
                                Console.WriteLine("Contents list");
                                Console.Write("Specify collection name: ");
                                string collectionName = Console.ReadLine();

                                Console.WriteLine(formatter.FormatContents(serviceConsumer.GetContentsByCollection(collectionName), collectionName));                                
                                break;
                            case 'q':
                                return;
                            default:
                                break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.ReadKey();
                }                
            }
        }
    }
}