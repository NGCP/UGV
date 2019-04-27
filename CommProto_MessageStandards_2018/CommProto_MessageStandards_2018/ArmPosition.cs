/*
 ArmPosition
 

 Copyright (C) 2016-2017  Northrup Grumman Collaboration Project.
 This program is free software: you can redistribute it and/or modify
 it under the terms of the GNU General Public License as published by
 the Free Software Foundation, either version 3 of the License, or
 (at your option) any later version.
  
 This program is distributed in the hope that it will be useful,
 but WITHOUT ANY WARRANTY; without even the implied warranty of
 MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 GNU General Public License for more details.
  
 You should have received a copy of the GNU General Public License
 along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Comnet;
using Comnet.Serialization;

namespace NGCP
{
  /**
   * ArmPosition Packet Template. 
   */
  public class ArmPosition : ABSPacket
  {
    public ArmPosition(Int32 position1,
      Int32 position2,
      Int32 position3,
      Int32 position4)
      : base("ArmPosition")
    {
      this.position1 = position1;
      this.position2 = position2;
      this.position3 = position3;
      this.position4 = position4;
    }

    public override void Pack(ObjectStream obj)
    {
      obj.Input(position1);
      obj.Input(position2);
      obj.Input(position3);
      obj.Input(position4);
    }

    public override void Unpack(ObjectStream obj)
    {
      position4 = obj.OutputInt32();
      position3 = obj.OutputInt32();
      position2 = obj.OutputInt32();
      position1 = obj.OutputInt32();
    }
	
    public override ABSPacket Create()
    {
      return new ArmPosition(0, 0, 0, 0);
    }

    #region Data
    public Int32 position1 { get; set; }
    public Int32 position2 { get; set; }
    public Int32 position3 { get; set; }
    public Int32 position4 { get; set; }
    #endregion
  }
}

