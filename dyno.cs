using System;
using System.IO;
using System.Collections.Generic;
using System.Net;
using System.Linq;

public class Solution
{
    public static void Main()
    {
        var webRequest = WebRequest.Create(@"https://gist.githubusercontent.com/ovdirek/6f555c2d810ac692f847b95e06ea4388/raw/c4ae1a69a23dca9dc96a5484dc4e491708d83ecb/dyno.log");
        string[] okunanVeri;
        using (var response = webRequest.GetResponse())
        using (var content = response.GetResponseStream())
        using (var reader = new StreamReader(content))
        {
            var strContent = reader.ReadToEnd();
            okunanVeri = strContent.Split("\n");
        }

        List<string> dyno = new List<string>();
        List<string> url = new List<string>();
        List<int> connectTime = new List<int>();
        List<int> serviceTime = new List<int>();
        int count_pending_messages = 0, get_messages = 0, get_friends_progress = 0, get_friends_score = 0, user_post = 0, user_get = 0;

        foreach (string item in okunanVeri)//Okunan her bir url için aşağıya giriyorum.
        {
            int indisMethod, indisPath;
            string methodType;
            if (item.IndexOf("method=") != -1 && item.IndexOf("path=") != -1)
            {
                indisMethod = item.IndexOf("method=");
                indisPath = item.IndexOf("path=");
                methodType = item.Substring(indisMethod + 7, indisPath - indisMethod - 8);
                if (methodType == "GET" || methodType == "POST")//Method Check
                {
                    int indisHost;
                    if (item.IndexOf("host=") != -1)
                    {
                        indisHost = item.IndexOf("host=");
                        string urlInner = item.Substring(indisPath + 5, indisHost - indisPath - 6);
                        url.Add(urlInner);//Her bir url listeye ekleniyor.
                        if (urlInner.StartsWith("/api/users/"))//URL Check
                        {
                            int indisDyno, indisConnect, indisService, indisStatus;
                            if (item.IndexOf("dyno=") != -1 && item.IndexOf("connect=") != -1 && item.IndexOf("service=") != -1 && item.IndexOf("status=") != -1)
                            {
                                indisService = item.IndexOf("service=");
                                indisStatus = item.IndexOf("status=");
                                indisDyno = item.IndexOf("dyno=");
                                indisConnect = item.IndexOf("connect=");
                                
                                string dynoInner = item.Substring(indisDyno + 5, indisConnect - indisDyno - 6);

                                dyno.Add(dynoInner);//Şartlara uyan bir url e Dyno listesine ekledim.

                                string serviceInner = item.Substring(indisService + 8, indisStatus - indisService - 11);
                                string connectInner = item.Substring(indisConnect + 8, indisService - indisConnect - 11);

                                connectTime.Add(Convert.ToInt32(connectInner)); //Şartlara uyan bir url için connect time listesine ekledim.
                                serviceTime.Add(Convert.ToInt32(serviceInner)); //Şartlara uyan bir url için service time listesine ekledim.

                                string[] bolunmusString = urlInner.Split('/');
                                if (bolunmusString.Length == 5)//User isteğiyle gelen veriler.
                                {
                                    if (bolunmusString[4] == "count_pending_messages")
                                        count_pending_messages++;
                                    else if (bolunmusString[4] == "get_messages")
                                        get_messages++;
                                    else if (bolunmusString[4] == "get_friends_progress")
                                        get_friends_progress++;
                                    else if (bolunmusString[4] == "get_friends_score")
                                        get_friends_score++;
                                }
                                else if (methodType == "GET")//User olmadan gelen veriler
                                    user_get++;
                                else if (methodType == "POST")
                                    user_post++;

                            }
                        }
                    }
                }
            }
        }

        connectTime.Sort(); //OrtaDeğer almak için sıralıyoruz.
        serviceTime.Sort(); //OrtaDeğer almak için sıralıyoruz.
        double averageConnectTime = connectTime.Average();
        double averageServiceTime = serviceTime.Average();

        int ortaDegerConnectTime = 0;
        int ortaDegerServiceTime = 0;

        if (connectTime.Count % 2 == 0) //Çift eleman varsa
            ortaDegerConnectTime = (connectTime[(connectTime.Count - 1) / 2] + connectTime[(connectTime.Count) / 2]) / 2;
        else
            ortaDegerConnectTime = connectTime[connectTime.Count / 2];

        if (serviceTime.Count % 2 == 0) //Çift eleman varsa
            ortaDegerServiceTime = (serviceTime[(serviceTime.Count - 1) / 2] + serviceTime[(serviceTime.Count) / 2]) / 2;
        else
            ortaDegerServiceTime = serviceTime[serviceTime.Count / 2];

        Console.WriteLine("Average Connect Time: " + averageConnectTime.ToString());
        Console.WriteLine("Average Service Time: " + averageServiceTime.ToString());
        Console.WriteLine("Average Connect and Service Time: " + ((averageServiceTime + averageConnectTime) / 2).ToString());
        Console.WriteLine("Median Connect Time: " + ortaDegerConnectTime.ToString());
        Console.WriteLine("Median Service Time: " + ortaDegerServiceTime.ToString());

        Console.WriteLine("User Count Pending Messages: " + count_pending_messages.ToString());
        Console.WriteLine("User Get Messages: " + get_messages.ToString());
        Console.WriteLine("User Get Friends Progress: " + get_friends_progress.ToString());
        Console.WriteLine("User Get Friends Score: " + get_friends_score.ToString());
        Console.WriteLine("Userless Get Count: " + user_get.ToString());
        Console.WriteLine("Userless Post Count: " + user_post.ToString());

        var tekilDeger = dyno.Select(a => a.ToLower()).Distinct();
        var mostDyno = "";
        int countDyno = 0;
        foreach (var item in tekilDeger)
        {
            var deger = dyno.Where(r => r.ToLower() == item);
            if (deger.Count() > countDyno)
            { 
                countDyno = deger.Count();
                mostDyno = deger.FirstOrDefault();
            }
        }

        Console.WriteLine("Most Dyno :" + mostDyno.ToString());
        Console.WriteLine("Most Dyno Count: " + countDyno.ToString());
    }
}
