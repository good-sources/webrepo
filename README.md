# webrepo
Prototype of a system for data syndication

The solution consists of two projects, namely:
– AggregationService, ASP.NET Web API 2 application which conforms to REST-based principles
– AggregationServiceClient, C# application which is supposed to consume the former

Aggregation service works in a similar fashion as existing data syndication tools avalailable on the internet. Currently it only supports RSS feeds as the source of data. But the architecture of the service is extensible so that the support of new types of data source can be easely added.
The core feature of the service lays in its ability to extensively cache data following the standards of HTTP protocol for cache revalidation and (optionaly) the content negotiation agreements taken for a specific type of data source.
