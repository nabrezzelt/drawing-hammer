using DrawingHammerDataLib.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace DrawingHammerDataLib
{
    [Serializable]
    public class Packet
    {
        public PacketType PacketType;
        public byte[] File;
        public List<object> Data;
        public string Uid;
        public string DestinationUid;

        public Packet(PacketType type)
        {
            this.PacketType = type;
            this.Data = new List<object>();
        }

        public Packet(PacketType type, byte[] file)
        {
            this.PacketType = type;
            this.File = file;
            this.Data = new List<object>();
        }

        public Packet(PacketType type, List<object> data)
        {
            this.PacketType = type;
            this.Data = data;
        }

        public Packet(PacketType type, byte[] file, List<object> data)
        {
            this.PacketType = type;
            this.File = file;
            this.Data = data;
        }

        public Packet(PacketType type, string uid)
        {
            this.PacketType = type;
            this.Uid = uid;
            this.Data = new List<object>();
        }       

        public Packet(PacketType type, string uid, List<object> data)
        {
            this.PacketType = type;
            this.Data = data;
            this.Uid = uid;
        }

        public Packet(PacketType type, string uid, string destinationUID, List<object> data)
        {
            this.PacketType = type;
            this.Data = data;
            this.DestinationUid = destinationUID;
            this.Uid = uid;
        }

        /// <summary>
        /// Deserialisiert den gegebenen ByteArray in ein Packet-Objekt (von JSON zurück zu einem Objekt).
        /// </summary>
        /// <param name="packetType">Packet-Objekt als ByteArray</param>
        public Packet(byte[] packetBytes)
        {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream(packetBytes);

            Packet p = (Packet)bf.Deserialize(ms); //Deserialisiert den ByteArray in ein Objekt (von JSON zu Objekt)

            ms.Close();

            this.PacketType = p.PacketType;
            this.Uid = p.Uid;
            this.DestinationUid = p.DestinationUid;
            this.File = p.File;
            this.Data = p.Data;
        }

        /// <summary>
        /// Serialisiert dieses Object in einen ByteArray (ähnlich wie bei PHP wenn ein Objekt zu einem JSON serialisiert wird).
        /// </summary>
        /// <returns>Serialisierter Byte-Array von diesem Objekt</returns>
        public byte[] ConvertToBytes()
        {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();

            bf.Serialize(ms, this); //Serialisiert dieses Objekt in einen ByteArray (funktioniert ählich wie JSON)
            byte[] serializedObject = ms.ToArray();
            ms.Close();

            return serializedObject;
        }

        /// <summary>
        /// Gibt die eigene IPv4 Adresse zurück. Wenn keine Adresse vergeben ist wird 127.0.0.1 zurückgegeben
        /// </summary>
        /// <returns>IPv4-Adresse als string</returns>
        public static string GetThisIPv4Adress()
        {
            IPAddress[] ips = Dns.GetHostAddresses(Dns.GetHostName());

            foreach (IPAddress ip in ips)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }

            return "127.0.0.1";
        }
    }
}
