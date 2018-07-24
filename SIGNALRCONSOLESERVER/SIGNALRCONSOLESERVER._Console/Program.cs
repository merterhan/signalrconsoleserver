using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.Owin.Hosting;
using Newtonsoft.Json;
using SIGNALRCONSOLESERVER._Console.Model;
using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;
using TableDependency;
using TableDependency.EventArgs;
using TableDependency.SqlClient;

namespace SIGNALRCONSOLESERVER._Console
{
    class Program
    {
        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("Local IP Address Not Found!");
        }
        public static string GetLocalFQDN()
        {
            var props = IPGlobalProperties.GetIPGlobalProperties();
            return props.HostName + (string.IsNullOrWhiteSpace(props.DomainName) ? "" : "." + props.DomainName);
        }
        private static string _con = "data source=localhost;initial catalog=Test;user id=sa;password=123456;MultipleActiveResultSets=True;App=EntityFramework";

        static void Main(string[] args)
        {
            // This will *ONLY* bind to localhost, if you want to bind to all addresses
            // use http://*:8080 to bind to all addresses. 
            // See http://msdn.microsoft.com/library/system.net.httplistener.aspx 
            // for more information.
            string url = "http://*:8081";
            ////when server is remote run the command below on cmd administrator
            ////netsh http add urlacl url=http://*:8081/ user=Everyone

            int startin = 60 - DateTime.Now.Second;
            var t = new System.Threading.Timer(o => sendALot(),
                 null, startin * 1000, 3000);

            using (WebApp.Start(url))
            {
                Console.WriteLine("Server running on {0}", url);
                Console.ReadLine();
            }

            ////SQLTABLEDEPENDENCY
            //using (var dep = new SqlTableDependency<Alarm>(_con, tableName: "Alarm"))
            //{
            //    dep.OnChanged += TableChanged;
            //    dep.Start();

            //    Console.WriteLine("Press a key to exit");
            //    Console.ReadKey();

            //    dep.Stop();
            //}
            ////end SQLTABLEDEPENDENCY
        }

        //public static void TableChanged(object sender, RecordChangedEventArgs<Alarm> e)
        //{
        //    var changedEntity = e.Entity;
        //    //string result = JsonConvert.SerializeObject(changedEntity, Formatting.Indented);
        //    //Console.WriteLine(result);
        //    //Console.ReadLine();
        //}

        static void sendALot()
        {
            Random rnd = new Random();
            var context = GlobalHost.ConnectionManager.GetHubContext<MyHub>();
            context.Clients.All.addMessage(rnd.Next(0, 6));
            Console.WriteLine("Sended at: " + DateTime.Now.ToString());

        }

        public void InsertAlarm(Alarm model)
        {
            using (TestEntities db = new TestEntities())
            {
                try
                {
                    db.Alarm.Add(model);
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    Console.WriteLine("InsertAlarm() error: " + e.Message);
                }
            }
        }
    }

    [HubName("MyHub")]
    public class MyHub : Hub
    {
        public string Send(string message)
        {
            return message;
        }

        public void DoSomething(int? param)
        {
            Clients.All.addMessage("0");
        }

        public override Task OnConnected()
        {
            // Add your own code here.
            // For example: in a chat application, record the association between
            // the current connection ID and user name, and mark the user as online.
            // After the code in this method completes, the client is informed that
            // the connection is established; for example, in a JavaScript client,
            // the start().done callback is executed.

            string clientConnectionId = Context.ConnectionId;
            Console.WriteLine("client connected with " + clientConnectionId + " connectionId");
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            // Add your own code here.
            // For example: in a chat application, mark the user as offline, 
            // delete the association between the current connection id and user name.

            if (stopCalled)
            {
                Console.WriteLine(String.Format("Client {0} explicitly closed the connection.", Context.ConnectionId));
            }
            else
            {
                Console.WriteLine(String.Format("Client {0} timed out .", Context.ConnectionId));
            }
            return base.OnDisconnected(stopCalled);
        }

        public override Task OnReconnected()
        {
            Console.WriteLine(Context.ConnectionId + " reconnected.");
            // Add your own code here.
            // For example: in a chat application, you might have marked the
            // user as offline after a period of inactivity; in that case 
            // mark the user as online again.
            return base.OnReconnected();
        }
    }
}
