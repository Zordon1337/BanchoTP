
using BanchoTroller.Structs;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Globalization;


public class Server
{
    public class Writer : BinaryWriter
    {
        public Writer(Stream s) : base(s) { }
        public override void Write(string value)
        {
            if (value != null)
            {
                if (value.Length == 0)
                {
                    this.Write(new byte[] { 0x00 });
                    return;
                }
            }

            try
            {
                this.Write((byte)11);
                base.Write(value);
            }
            catch
            {

            }
        }
    }
    public static string ReadStringFromStream(TcpClient client)
    {
        byte[] buffer = new byte[4096];
        var stream = client.GetStream();

        if (stream.CanRead)
        {
            string receivedData = null;

            
            var tsk = Task.Run(() =>
            {
                while (true)
                {
                    
                    if (stream.DataAvailable)
                    {
                        int bytesReceived = stream.Read(buffer, 0, buffer.Length);
                        receivedData += Encoding.UTF8.GetString(buffer, 0, bytesReceived);
                    }
                    else
                    {
                        
                        break;
                    }
                }
            });

            Task.WaitAll(tsk);

            return receivedData;
        }

        return null;
    }

    
    public static void Annouce(TcpClient user, string message)
    {
        MemoryStream ms = new MemoryStream();
        Writer writer = new Writer(ms);
        writer.Write(message);
        writer.Flush();
        SendPacket(25,ms,user);
    }
    public static void Main(string[] args)
    {
        TcpListener srv = new TcpListener(13381);

        srv.Start();
        while(true)
        {
            var client = srv.AcceptTcpClient();
            MemoryStream ms = new MemoryStream();
            Writer bw = new Writer(ms);
            
            try
            {
                string data = ReadStringFromStream(client);
                string[] lines = new string[] { };
                if (data != null)
                {
                     lines = data.Split('\n');
                }
                if (lines.Length >= 2)
                {
                    string username = lines[0]?.Trim();
                    string password = lines[1]?.Trim();
                    string version = lines[2].Split('|')[0];
                    Console.WriteLine($"{username} {password}");
                    string reply = new WebClient().DownloadString($"http://osu.zndev.xyz:8080/web/osu-login.php?username={username}&password={password}");
                    if (reply == "1")
                    {
                        Console.WriteLine("correct");
                        bw.Write(1);
                        bw.Flush();
                        SendPacket(5, ms, client);
                        ms = new MemoryStream();
                        
                        try
                        {
                            string userdata = new WebClient().DownloadString("http://osu.zndev.xyz:8080/web/osu-statoth.php?u=" + username);
                            string[] lines2 = userdata.Split('|');
                            long totalscore = long.Parse(lines2[0]);
                            
                            float accuracy = Single.Parse(lines2[1], CultureInfo.InvariantCulture);
                            int rank = int.Parse(lines2[4]);
                            string pfp = lines2[5];
                            Console.WriteLine($"{username} just logged with {version}");
                            bStatusUpdate su = new bStatusUpdate(bStatus.Idle, false, "", "", 0, Mods.None, PlayModes.OsuStandard);
                            Writer bw2 = new Writer(ms);
                            Int32 id = 1;
                            bw2.Write(id); // id
                            bw2.Write((byte)Completeness.Statistics); // completeness
                            su.WriteToStream(bw2); // status
                            bw2.Write((long)totalscore); // Ranked score
                            bw2.Write((float)accuracy); // Accuracy
                            bw2.Write(0); // Playcount
                            bw2.Write((long)totalscore); // Total Score
                            bw2.Write(rank); // Rank
                            SendPacket(12, ms, client);
                            bw.Write("#osu");
                            bw.Flush();
                            SendPacket(64, ms, client);
                            new Thread(() => PlayerThread(client, username, version, id)).Start();
                            
                        } catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                        
                        
                    }
                    else
                    {
                        Console.WriteLine("not correct");
                        bw.Write(-1);
                        bw.Flush();
                        SendPacket(5, ms, client);
                    }
                    
                }
                else
                {
                    Console.WriteLine("Blyaaaat");
                }
            } catch
            {

            }

            
           
            
        }
    }
    
    public static void PlayerThread(TcpClient client, string username, string version, int userid)
    {
        long oldscore = 0;
        try
        {
            Annouce(client, "Welcome to BanchoTroller \nosu2007srv proxy for \nclients using TCP");
        } catch(Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        new Thread(() =>
        {
            while (true)
            {
                SendEmptyPacket(8, client);
            }
        });
        while (true)
        {
            if(client.Connected)
            {
                var stream = client.GetStream();
                MemoryStream ms = new MemoryStream();
                Writer w2 = new Writer(ms);
                BinaryReader br = new BinaryReader(stream);
                try
                {
                    switch (br.ReadInt16())
                    {

                        case (int)RequestType.Osu_RequestStatusUpdate:
                            {
                                string userdata = new WebClient().DownloadString("http://osu.zndev.xyz:8080/web/osu-statoth.php?u=" + username);
                                string[] lines2 = userdata.Split('|');
                                long totalscore = long.Parse(lines2[0]);
                                float accuracy = Single.Parse(lines2[1], CultureInfo.InvariantCulture);
                                int rank = int.Parse(lines2[4]);
                                string pfp = lines2[5];
                                bStatusUpdate su = new bStatusUpdate(bStatus.Idle, false, "", "", 0, Mods.None, PlayModes.OsuStandard);
                                w2.Write(userid);
                                w2.Write((byte)Completeness.Statistics);
                                su.WriteToStream(w2);
                                w2.Write((long)totalscore);
                                w2.Write((float)accuracy);
                                w2.Write(0);
                                w2.Write((long)totalscore);
                                w2.Write(rank);
                                SendPacket(12, ms, client);
                                Console.WriteLine("Got Packet");
                                break;
                            }
                    }
                } catch { }
                
            } else
            {
                //return;
            }
        }
    }
    public static void SendPacket(int packet, MemoryStream ms, TcpClient client)
    {
        try
        {
            Writer bw = new Writer(client.GetStream());
            bw.Write((ushort)packet);
            bw.Write(false);
            bw.Write((uint)ms.Length);
            bw.Write(ms.ToArray());
            bw.Flush();
        } catch { }
        
    }
    public static void SendEmptyPacket(int packet, TcpClient c )
    {
        try
        {
            Writer bw = new Writer(c.GetStream());
            bw.Write((ushort)packet);
            bw.Write(false);
            bw.Write(0);
            bw.Flush();
        } catch { }
    }
}