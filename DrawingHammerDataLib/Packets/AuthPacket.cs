using DrawingHammerDataLib.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrawingHammerDataLib.Packets
{
     [Serializable]
    public class AuthPacket : BasePacket
    {        
        public AuthenticationType AuthType;

        public AuthPacket(AuthenticationType authType, string senderUID, string destionationUID) : base(BasePacketType.Authentication, senderUID, destionationUID)
        {
            AuthType = authType;         
        }        
    }
}
