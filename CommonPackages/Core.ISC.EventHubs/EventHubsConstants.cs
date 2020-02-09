using System;
using System.Collections.Generic;
using System.Text;

namespace Core.ISC.EventHubs
{
    public static class EventHubsValueConstants
    {
        public const string eventHubName = "vca-dev-westus-pv-eh";
        public const string storageAccountName = "storageaccms";
        public const string storageAccountKey = "U/OyI1ZAYmbGav2F0J3Lj2lTvyErb9KhqmbqkCagDk+HnHQlMMDjacGjVBuVVZkG0QBR6Ti4Kt2BRocgLFf0Zg==";
        public static string eventProcessorHostName = Guid.NewGuid().ToString();
        public const string consumerGroupName = "notificationconsumergroup";
        public const string storageContainerName = "mscontainer";
        public const string eventHandlerMethodName = "Handle";
        public const string connectionString = "Endpoint=sb://vca-dev-westus-pv-ehns.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessPolicy;SharedAccessKey=ICkIWmanZ6WRAhDm5emThX1yNgV/djZiY8CIBqu/r+Y=;EntityPath=vca-dev-westus-pv-eh";
    }
    public static class EventHubsKeyConstants
    {
        public const string connectionString = "EventHubs:connectionString";
        public const string consumerGroupName = "EventHubs:consumerGroupName";
        public const string eventHubsName = "EventHubs:eventHubName";
        public const string storageAccountName = "EventHubs:storageAccountName";
        public const string storageConnString = "EventHubs:storageConnectionString";
        public const string storageAccountKey = "EventHubs:storageAccountKey";
        public const string storageContainerName = "EventHubs:storageContainerName";
    }
}
