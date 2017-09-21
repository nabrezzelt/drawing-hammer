using DrawingHammerDataLib.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace DrawingHammerDataLib.Packets
{
    [Serializable]
    public abstract class BasePacket
    {
        public const string SERVER = "server";
        public const string ALL = "*";      

        public BasePacketType Type;
        public string SenderUID;
        public string DestinationUID;

        protected BasePacket(BasePacketType type, string senderUID, string destinationUID)
        {
            Type = type;
            SenderUID = senderUID;
            DestinationUID = destinationUID;
        }

        public static byte[] Serialize(object packet)
        {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();

            bf.Serialize(ms, packet); //Serialisiert dieses Objekt in einen ByteArray (funktioniert ählich wie JSON)
            byte[] serializedObject = ms.ToArray();
            ms.Close();

            return serializedObject;
        }

        public static object Deserialize(byte[] packetBytes)
        {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream(packetBytes);

            object packet = (BasePacket) bf.Deserialize(ms); //Deserialisiert den ByteArray in ein Objekt (von JSON zu Objekt)

            ms.Close();

            return packet;
        }

        public enum BasePacketType
        {
            Authentication            
        }
    }
}
